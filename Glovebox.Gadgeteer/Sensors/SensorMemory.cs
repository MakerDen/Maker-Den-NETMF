using Glovebox.MicroFramework.Base;
using Microsoft.SPOT;
using System;

namespace Glovebox.Gadgeteer.Sensors
{
    public class SensorMemory : SensorBase {

        public SensorMemory(int SampleRateMilliseconds, string name)
            : base("mem", "kb", ValuesPerSample.One, SampleRateMilliseconds, name) {

            StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = Debug.GC(false);
        }

        protected override string GeoLocation() {
            return string.Empty;
        }



        protected override void SensorCleanup()
        {
            throw new NotImplementedException();
        }

        public override double Current
        {
            get { return Debug.GC(false); }
        }
    }
}