using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;

namespace Glovebox.Netduino.Sensors {
    public class SensorLight : SensorBase {
        protected AnalogInput analogPin;

        protected override double Current { get { return (int)(analogPin.Read() * 100); } }

        public SensorLight(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base(SensorType.Light, ValuesPerSample.One, SampleRateMilliseconds, name) {

                analogPin = new AnalogInput(pin, -1);
                StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = (int)(analogPin.Read() * 100);
        }

        protected override string GeoLocation() {
            return Utilities.RandomPostcode();
        }

        protected override void SensorCleanup() {
            if (analogPin != null) { analogPin.Dispose(); }
        }
    }
}
