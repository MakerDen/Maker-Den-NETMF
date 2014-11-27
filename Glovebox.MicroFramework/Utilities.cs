using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Time;
using System.Net;
using System.Threading;
using System.Text;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;

namespace Glovebox.MicroFramework {
    public static class Utilities {

        const string ntpServer = "au.pool.ntp.org";
        private readonly static string[] postcodes = new string[] { "3000", "6000", "2011" };
        private static Random rnd = new Random(Environment.TickCount);
        const int networkSettleTime = 1000;

        public static int RandomNumber(int Range) {
            return rnd.Next(Range);
        }

        public static bool SetTime(bool connected) {
            if (!connected) { return false; }
            try {
                //network settle time
                Thread.Sleep(networkSettleTime);
                var ntpAddress = GetTimeServiceAddress(ntpServer);
                if (ntpAddress == null) {return false;}

                TimeService.UpdateNow(ntpAddress, 200);

                return true;
            }
            catch { return false; }
        }

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

        public static string RandomPostcode() {
            return postcodes[rnd.Next(postcodes.Length)];
        }

        public static string BytesToString(byte[] Input)
        {
            char[] Output = new char[Input.Length];
            for (int Counter = 0; Counter < Input.Length; ++Counter)
            {
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

            Utilities.SetTime(connected);
            return new ServiceManager(ConfigurationManager.Broker, connected);
        }

        public static string GetMacAddress() {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces()) {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
                    return MacToString(nic.PhysicalAddress);
                }
            }
            return string.Empty;
        }

        private static string MacToString(byte[] macAddress) {
            string result = string.Empty;
            foreach (var part in macAddress) {
                result += part.ToString("X") + "-";
            }
            return result.Substring(0, result.Length - 1);
        }

        public static int Absolute(int value) {
            // get absolute value bit shifting a 32 bit int
            int mask = value >> 31;
            return (mask + value) ^ mask;
        }
    }
}
