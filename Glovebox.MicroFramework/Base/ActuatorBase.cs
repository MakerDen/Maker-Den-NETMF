using System;
using Microsoft.SPOT;
using Glovebox.MicroFramework.IoT;

namespace Glovebox.MicroFramework.Base {
    public abstract class ActuatorBase : IotBase {
        protected abstract void ActuatorCleanup();
        public abstract override void Action(IotAction action);
        protected uint ActuatorErrorCount;

        static readonly string[] actuatorType = new string[] { "rgbled", "relay", "piezo", "NeoPixel" };
        public enum ActuatorType { RgbLed, Relay, Piezo, NeoPixel }


        public ActuatorBase(string name, ActuatorType type)
            : base(name, actuatorType[(int)type]) {
            this.ThisIotType = IotType.Actuator;
        }

        protected override void CleanUp() {
            ActuatorCleanup();
        }
    }
}
