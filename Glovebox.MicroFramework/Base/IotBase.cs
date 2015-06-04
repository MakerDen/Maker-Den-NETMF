using Glovebox.IoT.Command;
using System;

namespace Glovebox.IoT.Base {
    public abstract class IotBase : IDisposable {
        protected abstract void CleanUp();

        public enum IotType { Sensor, Actuator };
        public IotType ThisIotType { get; set; }

        protected readonly string deviceName = ConfigurationManager.DeviceId;
        private readonly string name;
        public string Name { get { return name; } }

        protected readonly string type;
        public string Type { get { return type; } }

        public uint TotalActionCount { get; private set; }
        protected int TotalSensorMeasurements;


        protected readonly uint id;

        public IotBase(string name, string type) {
            this.name = name == null ? "unknownName" : name.ToLower();
            this.type = type == null ? "unknownType" : type.ToLower();
            this.id = IotActionManager.AddItem(this);
        }

        public void IncrementActionCount() {
            TotalActionCount++;
        }

        void IDisposable.Dispose() {
            IotActionManager.RemoveItem(id);
            CleanUp();
        }

        public virtual void Action(IotAction action) { }
    }
}
