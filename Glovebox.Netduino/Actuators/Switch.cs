using Glovebox.IoT.Base;
using Glovebox.IoT.IoT;
using Microsoft.SPOT.Hardware;


// www.dfrobot.com relay module V3.1
namespace Glovebox.Netduino.Actuators {
    public class Switch : ActuatorBase {
        public enum Actions {
            On,
            Off
        }

        private OutputPort switchPin;

        public Switch(Cpu.Pin pin, string name) : this(pin, name, "switch") { }
        public Switch(Cpu.Pin pin, string name, string type)
            : base(name, type) {
            switchPin = new OutputPort(pin, false);
        }

        protected override void ActuatorCleanup() {
            switchPin.Dispose();
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

        public override void Action(IotAction action) {
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
            switchPin.Write(true);
        }

        public void TurnOff() {
            switchPin.Write(false);
        }
    }
}
