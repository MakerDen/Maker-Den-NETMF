using Gadgeteer.Modules.GHIElectronics;
using Glovebox.MicroFramework.Base;


// www.dfrobot.com relay module V3.1
namespace Glovebox.Gadgeteer.Actuators {
    public class Relay : ActuatorBase {

        public enum Actions {
            On,
            Off
        }


        RelayX1 relayX1;

        /// <summary>
        /// Create a relay control
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public Relay(RelayX1 relayX1, string name)
            : base(name, "relay") {

            this.relayX1 = relayX1;
        }

        protected override void ActuatorCleanup() {
        }

        public void Action(Actions action) {
            switch (action) {
                case Actions.On:
                    TurnOn();
                    break;
                case Actions.Off:
                    TurnOff();
                    break;
                default:
                    break;
            }
        }

        public override void Action(MicroFramework.IoT.IotAction action) {
            switch (action.cmd) {
                case "on":
                    TurnOn();
                    break;
                case "off":
                    TurnOff();
                    break;
            }
        }

        public void TurnOn() {
            relayX1.TurnOn();
        }

        public void TurnOff() {
            relayX1.TurnOff();
        }
    }
}
