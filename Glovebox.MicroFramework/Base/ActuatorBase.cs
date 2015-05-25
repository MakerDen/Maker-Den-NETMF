using Glovebox.IoT.Command;

namespace Glovebox.IoT.Base {
    public abstract class ActuatorBase : IotBase {
        protected abstract void ActuatorCleanup();
        public abstract override void Action(IotAction action);
        protected uint ActuatorErrorCount;

        //static readonly string[] actuatorType = new string[] { "rgbled", "relay", "piezo", "neopixel", "led", "servopwm", "rgbledpwm" };
        //public enum ActuatorType { RgbLed, Relay, Piezo, NeoPixel, Led, ServoPwm, RgbLedPwm }


        public ActuatorBase(string name, string type)
            : base(name, type) {
            this.ThisIotType = IotType.Actuator;
        }

        protected override void CleanUp() {
            ActuatorCleanup();
        }
    }
}
