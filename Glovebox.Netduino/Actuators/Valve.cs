using Microsoft.SPOT.Hardware;


// www.dfrobot.com relay module V3.1
namespace Glovebox.Netduino.Actuators {
    public class Valve : Switch {
        public enum Actions {
            On,
            Off
        }

        private OutputPort switchPin;

        /// <summary>
        /// Create a relay control
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public Valve(Cpu.Pin pin, string name)
            : base(pin, name, "valve") {
        }
    }
}
