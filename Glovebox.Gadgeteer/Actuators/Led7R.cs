using Gadgeteer.Modules.GHIElectronics;
using Glovebox.MicroFramework.Base;

namespace Glovebox.Gadgeteer.Actuators {
    public class Led7R : ActuatorBase {

        LED7R led7R;

        public Led7R(LED7R led7R, string name)
            : base(name, "Led7R") {

                this.led7R = led7R;            
        }

        public void AllOn() {
            led7R.TurnAllLedsOn();
        }

        public void AllOff() {
            led7R.TurnAllLedsOff();
        }

        protected override void ActuatorCleanup() {

        }

        public override void Action(MicroFramework.IoT.IotAction action) {

        }
    }
}
