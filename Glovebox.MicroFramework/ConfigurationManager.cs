using System;
using Microsoft.SPOT;
using System.Text.RegularExpressions;

namespace Glovebox.MicroFramework {
    public static class ConfigurationManager {
        // Best efforts to run the MQTT Broker at gloveboxAE.cloudapp.net 
        // You can install your own instance of Mosquitto MQTT Server from http://mosquitto.org 
        public static string Broker = "gloveboxAE.cloudapp.net";

        public static string UniqueDeviceIdentifier { get; set; }

        private static string _devName = "emul";
        public static string DeviceName {
            get { return _devName; }
            set {
                _devName = value == null || value.Length == 0 ? "emul" : value;
                _devName = _devName.Length > 5 ? _devName.Substring(0, 5) : _devName;
            }
        }

        public static string MqttNameSpace = "gb/";
        public static string[] MqqtSubscribe = new string[] { "gbcmd/#" };
        public static string MqttDeviceAnnounce = "gbdevice/";

        public static uint mqttPrePublishDelay = 250;  // milliseconds delay before mqtt publish
        public static uint mqttPostPublishDelay = 200; // milliseconds delay after mqtt publish
    }
}
