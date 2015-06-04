using Microsoft.SPOT.Net.NetworkInformation;
using Microsoft.SPOT.Time;
using System.Threading;
using System.Net;


namespace Glovebox.IoT {
    public static partial class Util {
        const int networkSettleTime = 1000;

        public static bool SetTime(bool connected) {

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

        public static string GetUniqueDeviceId() {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces()) {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
                    return MacToString(nic.PhysicalAddress);
                }
            }
            return string.Empty;
        }

        public static string GetIPAddress() {
            string localIP = "?";

            return localIP;
        }

        public static void Delay(int milliseconds) {
            Thread.Sleep(milliseconds);
        }
    }
}
