using Microsoft.SPOT.Net.NetworkInformation;
using Microsoft.SPOT.Time;
using System;
using System.Net;
using System.Text;
using System.Threading;

namespace Glovebox.IoT {
    public static class Util {

        const string ntpServer = "au.pool.ntp.org";
        private readonly static string[] postcodes = new string[] { "3000", "6000", "2011" };
        private static Random rnd = new Random(Environment.TickCount);
        const int networkSettleTime = 1000;

        public static int RandomNumber(int Range) {
            return rnd.Next(Range);
        }

        public static bool SetTime(bool connected) {
#if MF_FRAMEWORK_VERSION_V4_3
            if (!connected) { return false; }
            try {
                //network settle time
                Util.Delay(networkSettleTime);
                var ntpAddress = GetTimeServiceAddress(ntpServer);
                if (ntpAddress == null) { return false; }

                TimeService.UpdateNow(ntpAddress, 200);

                return true;
            }
            catch { return false; }
#else
            return true;
#endif
        }

#if MF_FRAMEWORK_VERSION_V4_3
        private static byte[] GetTimeServiceAddress(string TimeServerAddress) {
            try {
                IPAddress[] address = Dns.GetHostEntry(TimeServerAddress).AddressList;
                if (address != null && address.Length > 0) {
                    return address[0].GetAddressBytes();
                }
                return null;
            }
            catch { return null; }
        }
#endif

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

        /// <summary>
        /// convert string to byte array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string str) {
            return new UTF8Encoding().GetBytes(str);
        }

        static public ServiceManager StartNetworkServices(string deviceName, bool connected, string deviceGuid) {
            ConfigurationManager.DeviceName = deviceName;
            ConfigurationManager.UniqueDeviceIdentifier = deviceGuid;
            if (!connected) { return null; }

            Util.SetTime(connected);
            return new ServiceManager(ConfigurationManager.Broker, connected);
        }

        public static string GetUniqueDeviceId() {
#if MF_FRAMEWORK_VERSION_V4_3
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces()) {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
                    return MacToString(nic.PhysicalAddress);
                }
            }
#endif
            return string.Empty;
        }

        private static string MacToString(byte[] macAddress) {
            string result = string.Empty;
            foreach (var part in macAddress) {
                result += part.ToString("X") + "-";
            }
            return result.Substring(0, result.Length - 1);
        }

        public static void Delay(int milliseconds) {
#if MF_FRAMEWORK_VERSION_V4_3
            Thread.Sleep(milliseconds);
#else
            Task.Delay(milliseconds).Wait();
#endif
        }
    }
}
