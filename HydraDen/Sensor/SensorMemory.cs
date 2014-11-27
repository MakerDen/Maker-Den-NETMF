using System;
using Microsoft.SPOT;
using Glovebox.MicroFramework;

namespace test {
    class SensorMemory : SensorBase {

        public SensorMemory(int SampleRateMilliseconds)
            : base(SensorType.MemFezHydra, ValuesPerSample.One, SampleRateMilliseconds) {

            StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = Debug.GC(false);
        }

        protected override string GeoLocation() {
            return string.Empty;
        }

        protected override double Current {
            get { return Debug.GC(false); }
        }
    }
}
