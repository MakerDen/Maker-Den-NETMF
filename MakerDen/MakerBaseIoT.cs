using System;
using Microsoft.SPOT;
using System.Threading;
using Microsoft.SPOT.Hardware;
using Glovebox.MicroFramework;
using Glovebox.Netduino;
using Glovebox.MicroFramework.Base;
using System.Runtime.CompilerServices;

namespace MakerDen {
    public class MakerBaseIoT {
        static public ServiceManager sm;
        static public RgbLed rgb;

       
        protected static void StartNetworkServices(string deviceName, bool connected, string uniqueDeviceIdentifier = "") {
            sm = Utilities.StartNetworkServices(deviceName, connected, uniqueDeviceIdentifier);
        }

        /// <summary>
        /// Blink an LED before measuring sensor
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        static public uint OnBeforeMeasure(object sender, EventArgs e) {
            uint id = ((SensorBase.SensorIdEventArgs)e).id;
            if (rgb == null) { return 0; }
            rgb.On((RgbLed.Led)(id % 3));
            Thread.Sleep(20);
            rgb.Off((RgbLed.Led)(id % 3));
            //   rgb.Blink((RgbLed.Led)(id % 3), 50, RgbLed.BlinkRate.VeryFast);
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
