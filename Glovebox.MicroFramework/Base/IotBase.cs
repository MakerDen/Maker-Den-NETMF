using System;
using Microsoft.SPOT;
using System.Threading;
using Glovebox.MicroFramework.IoT;

namespace Glovebox.MicroFramework.Base {
    public abstract class IotBase: IDisposable {
        protected abstract void CleanUp();

        public enum IotType { Sensor, Actuator };
        public IotType ThisIotType { get; set; }

        protected readonly string deviceName = ConfigurationManager.DeviceName;
        private readonly string name;
        public string Name {get { return name; }}

        protected readonly string type;
        public string Type { get { return type; } }

        public uint TotalActionCount { get; private set; }
        protected int TotalSensorMeasurements;


        protected readonly uint id;

        public IotBase(string name, string type) {
            this.name = name == null ? "unknown" : name.ToLower();
            this.type = type == null ? "unknown" : type.ToLower();
            this.id = IotList.AddItem(this);
        }

        public void IncrementActionCount(){
            TotalActionCount++;
        }

        void IDisposable.Dispose() {
            IotList.RemoveItem(id);
            CleanUp();
        }

        public virtual void Action(IotAction action){}
    }
}
