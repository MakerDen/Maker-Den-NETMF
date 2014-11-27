using System;
using Microsoft.SPOT;
using uPLibrary.Networking.M2Mqtt;
using System.Threading;
using uPLibrary.Networking.M2Mqtt.Messages;
using Microsoft.SPOT.Net.NetworkInformation;
using Microsoft.SPOT.Hardware;
using Glovebox.MicroFramework.IoT;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Text;
using Glovebox.MicroFramework.Json;


namespace Glovebox.MicroFramework
{
    public class ServiceManager
    {
        const int networkSettleTime = 6000;
        const uint MaxRetryCount = 10;
        uint errorCount;
        bool networkChanged = false;
        bool networkAvailable = true;
        bool connected;
        string serviceAddress;
        public MqttClient mqtt = null;
        readonly string clientId;
        uint mqttPrePublishDelay;
        uint mqttPostPublishDelay;
        readonly string uniqueDeviceIdentifier;
        int lastSystemrequest = Environment.TickCount;
        DateTime lastSystemRequestTime = DateTime.Now;

        public ServiceManager(string serviceAddress, bool connected)
        {
            uint retryCount = 0;
            this.serviceAddress = serviceAddress;
            this.connected = connected;
            this.clientId = CreateClientId();
            this.mqttPrePublishDelay = ConfigurationManager.mqttPrePublishDelay;
            this.mqttPostPublishDelay = ConfigurationManager.mqttPostPublishDelay;
            this.uniqueDeviceIdentifier = GetUniqueDeviceIdentifier(ConfigurationManager.UniqueDeviceIdentifier);


            if (!connected) { return; }

            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;

            while (mqtt == null && retryCount < MaxRetryCount) {
                try { StartMqtt(); }
                catch { retryCount++; }
            }
        
        }

        private string CreateClientId()
        {
            DateTime utc = DateTime.UtcNow;
            string cid = utc.Hour.ToString() + utc.Minute.ToString() + utc.Second.ToString() + "-" + Utilities.GetMacAddress();
            return cid.Length > 23 ? cid.Substring(0, 23) : cid;  //23 chars for clientid is mqtt max allowed
        }

        private string GetUniqueDeviceIdentifier(string value)
        {
            string id = string.Empty;

            if (value == null || value == string.Empty)
            {
                id = Utilities.GetMacAddress();
                if (id == null || id == string.Empty)
                {
                    id = Guid.NewGuid().ToString();
                }
            }
            else
            {
                Regex r = new Regex("/");
                id = r.Replace(value, "");
            }
            return id;
        }

        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            networkAvailable = e.IsAvailable;
            networkChanged = true;
        }

        void StartMqtt()
        {
            Debug.GC(true);
            bool networkReset = false;
            if (!connected) { return; }

            // give the network some settle time
            Thread.Sleep(networkSettleTime);

            while (mqtt == null || !mqtt.IsConnected)
            {
                networkReset = false;
                while (!networkAvailable)
                {
                    Thread.Sleep(2000);
                    networkReset = true;
                }
                // the network needs a bit of settle time, dhcp etc
                if (networkReset) { Thread.Sleep(networkSettleTime); }

                mqtt = new MqttClient(serviceAddress);
                if (mqtt != null && networkAvailable) { mqtt.Connect(clientId); }

            }

            mqtt.MqttMsgPublishReceived += mqtt_MqttMsgPublishReceived;
            mqtt.Subscribe(ConfigurationManager.MqqtSubscribe, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        void ReconnectMqtt()
        {
            ResetMqtt();
            StartMqtt();
        }

        void ResetMqtt()
        {
            if (mqtt != null)
            {
                mqtt.MqttMsgPublishReceived -= mqtt_MqttMsgPublishReceived;
                mqtt = null;
            }
        }

        void mqtt_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            DateTime now = DateTime.Now;

            if (now < lastSystemRequestTime.AddSeconds(2) || e.Message.Length > 4096 || e.Topic.Length > 256) { return; }

            lastSystemRequestTime = now;

            string[] result = IotList.Action(DecodeAction(e.Topic, Utilities.BytesToString(e.Message)));
            if (result != null)
            {
                Publish(ConfigurationManager.MqttDeviceAnnounce + ConfigurationManager.DeviceName, SystemConfig(result));
            }
        }

        private byte[] SystemConfig(string[] IotItems)
        {
            JSONWriter jw = new JSONWriter();
            jw.Begin();
            jw.AddProperty("Dev", ConfigurationManager.DeviceName);
            jw.AddProperty("Id", uniqueDeviceIdentifier);
            jw.AddProperty("Items", IotItems);
            jw.End();

            return jw.ToArray();
        }

        private IotAction DecodeAction(string topic, string message)
        {
            string[] topicParts = topic.ToLower().Split('/');

            if (topic.Length < 9) { return null; }

            switch (topic.Substring(0, 9))
            {
                case "gbcmd/all":
                    return ActionParts(topicParts, 2, message);
                case "gbcmd/dev":
                    // check device guid matches requested
                    if (topicParts.Length > 2 && topicParts[2] != string.Empty && topicParts[2] != null && topicParts[2] == uniqueDeviceIdentifier)
                    {
                        return ActionParts(topicParts, 3, message);
                    }
                    else { return null; }
            }
            return null;
        }

        private IotAction ActionParts(string[] topicParts, int startPos, string message)
        {
            IotAction action = new IotAction();
            action.parameters = message;

            for (int i = startPos, p = 0; i < topicParts.Length; i++, p++)
            {
                string part = topicParts[i].Length == 0 ? null : topicParts[i];
                if (part == null) { continue; }
                switch (p)
                {
                    case 0:
                        action.cmd = part;
                        break;
                    case 1:
                        action.item = part;
                        break;
                    case 2:
                        action.subItem = part;
                        break;
                    default:
                        break;
                }
            }
            return action;
        }

        private void PublishData(string topic, byte[] data)
        {
            Thread.Sleep((int)mqttPrePublishDelay);
            try
            {
                ExecutionConstraint.Install(20000, 0);
                while (mqtt == null || !mqtt.IsConnected || networkChanged)
                {
                    networkChanged = false;
                    ReconnectMqtt();
                }
                mqtt.Publish(topic, data);
            }
            catch (ConstraintException)
            {
                networkChanged = true;
                errorCount++;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                networkChanged = true;
                errorCount++;
            }
            finally
            {
                ExecutionConstraint.Install(-1, 0);
                Thread.Sleep((int)mqttPostPublishDelay);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public uint Publish(string topic, byte[] data)
        {
            if (!connected) { return 0; }

            PublishData(topic, data);

            return errorCount;
        }
    }
}
