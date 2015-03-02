using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;


// www.dfrobot.com relay module V3.1
namespace Glovebox.Netduino.Actuators {
    public class Relay : ActuatorBase {
        public enum Actions {
            On,
            Off
        }

        private OutputPort relay;

        /// <summary>
        /// Create a relay control
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public Relay(Cpu.Pin pin, string name)
            : base(name, "relay") {
            relay = new OutputPort(pin, false);
        }

        protected override void ActuatorCleanup() {
            relay.Dispose();
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
            relay.Write(true);
        }

        public void TurnOff() {
            relay.Write(false);
        }
    }
}
