using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;


// www.dfrobot.com relay module V3.1
namespace Glovebox.Netduino.Actuators {
    public class Relay : ActuatorBase {
        public enum Actions {
            Start,
            Stop
        }

        public OutputPort relay;

        /// <summary>
        /// Create a relay control
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public Relay(Cpu.Pin pin, string name)
            : base(name, ActuatorType.Relay) {
            relay = new OutputPort(pin, false);
        }

        public void TurnOn() {
            relay.Write(true);
        }

        public void TurnOff() {
            relay.Write(false);
        }

        protected override void ActuatorCleanup() {
            relay.Dispose();
        }

        public void Action(Actions action) {
            switch (action) {
                case Actions.Start:
                    TurnOn();
                    break;
                case Actions.Stop:
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
    }
}
