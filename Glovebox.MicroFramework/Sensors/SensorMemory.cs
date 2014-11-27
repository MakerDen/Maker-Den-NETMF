using System;
using Microsoft.SPOT;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;

namespace Glovebox.MicroFramework.Sensors {
    public class SensorMemory : SensorBase {


        public SensorMemory(int SampleRateMilliseconds, string name)
            : base(SensorType.Memory, ValuesPerSample.One, SampleRateMilliseconds, name) {

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

        protected override void SensorCleanup() {
        }
    }
}
