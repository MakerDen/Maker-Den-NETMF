using System;
using Microsoft.SPOT;
using System.Threading;
using Microsoft.SPOT.Hardware;
using GTM = Gadgeteer.Modules;
using Glovebox.MicroFramework;


namespace test {
    public class MakerBase : Gadgeteer.Program {

        public ServiceManager mm;
        private bool NetworkConnected;

        protected Gadgeteer.Modules.GHIElectronics.EthernetENC28 ethernetENC28 = new GTM.GHIElectronics.EthernetENC28(3);
        protected Gadgeteer.Modules.GHIElectronics.RelayX1 relayX1 = new GTM.GHIElectronics.RelayX1(6);
        protected Gadgeteer.Modules.GHIElectronics.CharacterDisplay characterDisplay = new GTM.GHIElectronics.CharacterDisplay(9);
        protected Gadgeteer.Modules.GHIElectronics.LED7R led7R = new GTM.GHIElectronics.LED7R(13);


        protected void Message(string msg) {
            characterDisplay.Clear();
            characterDisplay.Print(msg);
        }

        protected void Message(string msg, ushort row) {
            if (row > 1) { return; }
            characterDisplay.SetCursorPosition(row, 0);
            characterDisplay.Print(msg);
        }

        public void Initialise(string deviceName, bool connected) {
            deviceName = deviceName.Length > 5 ? deviceName.Substring(0, 5) : deviceName;
            deviceName = deviceName.Length == 0 ? "emul" : deviceName;
            NetworkConnected = connected;
            ConfigurationManager.DeviceName = deviceName;

            if (!connected) { return; }

            ethernetENC28.UseThisNetworkInterface();

            while (!ethernetENC28.IsNetworkConnected) {
                Thread.Sleep(1000);
            }

            Utilities.SetTime(connected);
            mm = new ServiceManager(ConfigurationManager.Broker, connected);
            mm.OnMessageReceived += mm_OnMqqtReceived;
        }

        void mm_OnMqqtReceived(object sender, EventArgs e) {
            var msg = ((ServiceManager.MessageMessageEventArgs)e).message;
            string message = Utilities.BytesToString(msg).ToLower();

            switch (message) {
                case "on":
                    relayX1.TurnOn();
                    break;
                case "off":
                    relayX1.TurnOff();
                    break;
                default:
                    break;
            }
        }

        public uint OnBeforeMeasure(object sender, EventArgs e) {
            uint id = ((SensorBase.SensorIdEventArgs)e).id;
            led7R.TurnAllLedsOff();
            led7R.SetLed((int)id % 6, true);
            return 0;
        }

        public uint OnMeasureCompleted(object sender, EventArgs e) {
            var i = (SensorBase.SensorItemEventArgs)e;
            Message("Msg Id: " + i.msgId.ToString(), 0);
            Message(DateTime.Now.ToString(), 1);

            if (!NetworkConnected) {
                Debug.Print(i.ToString());
                return 0;
            }

            if (mm == null) { return 0; }

            return mm.Publish(i.topic, i.ToJson());
        }
    }
}
