using System;
using System.Text;


namespace Glovebox.IoT {
    public static partial class Util {

        const string ntpServer = "au.pool.ntp.org";
        private readonly static string[] postcodes = new string[] { "3000", "4000", "5000", "6000", "7000" };
        private static Random rnd = new Random(Environment.TickCount);


        static public ServiceManager StartNetworkServices(string deviceId, bool connected, string networkId) {
            ConfigurationManager.DeviceId = deviceId;
            ConfigurationManager.NetworkId = networkId;
            if (!connected) { return null; }

            Util.SetTime(connected);
            return new ServiceManager(ConfigurationManager.Broker, connected);
        }

        public static int RandomNumber(int Range) {
            return rnd.Next(Range);
        }

        public static string RandomPostcode() {
            return postcodes[rnd.Next(postcodes.Length)];
        }

        public static string BytesToString(byte[] Input) {
            char[] Output = new char[Input.Length];
            for (int Counter = 0; Counter < Input.Length; ++Counter) {
                Output[Counter] = (char)Input[Counter];
            }
            return new string(Output);
        }

        public static byte[] StringToByteArray(string str) {
            return new UTF8Encoding().GetBytes(str);
        }

        private static string MacToString(byte[] macAddress) {
            string result = string.Empty;
            foreach (var part in macAddress) {
                result += part.ToString("X") + "-";
            }
            return result.Substring(0, result.Length - 1);
        }
    }
}
