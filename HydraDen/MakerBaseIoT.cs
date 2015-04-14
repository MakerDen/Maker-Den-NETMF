using Glovebox.Gadgeteer.Actuators;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;
using Microsoft.SPOT;
using System.Runtime.CompilerServices;
using System.Threading;
using GTM = Gadgeteer.Modules;


namespace HydraDen {

    public class MakerBaseIoT {

        static public ServiceManager sm;
        static public Led7R led;
        static public Relay relay;

        public static void StartNetworkServices(GTM.GHIElectronics.EthernetENC28 ethernetENC28, string deviceName, bool connected, string uniqueDeviceIdentifier = "") {
            ethernetENC28.UseThisNetworkInterface();
            while (!ethernetENC28.IsNetworkConnected) {
                Thread.Sleep(1000);
            }

            sm = Utilities.StartNetworkServices(deviceName, connected, uniqueDeviceIdentifier);
        }

        /// <summary>
        /// Blink an LED before measuring sensor
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        static public uint OnBeforeMeasure(object sender, EventArgs e) {
            uint id = ((SensorBase.SensorIdEventArgs)e).id;

            if (led != null) {
                led.AllOn();
             //   relay.TurnOn();
                Thread.Sleep(50);
                led.AllOff();
              //  relay.TurnOff();
            }

            return 0;
        }

        static public uint OnMeasureCompleted(object sender, EventArgs e) {
            if (sm == null) {
                Debug.Print(((SensorBase.SensorItemEventArgs)e).ToString());
                return 0;
            }

            // sensor mesurement completed, now publish to MQTT Service running on Azure
            var json = ((SensorBase.SensorItemEventArgs)e).ToJson();
            var topic = ((SensorBase.SensorItemEventArgs)e).topic;

            return sm.Publish(topic, json);
        }
    }
}
