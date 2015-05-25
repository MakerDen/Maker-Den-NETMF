using Glovebox.IoT.Base;
using Microsoft.SPOT;

namespace Glovebox.IoT.Sensors {
    public class SensorMemory : SensorBase {


        public SensorMemory(int SampleRateMilliseconds, string name)
            : base("mem", "b", ValuesPerSample.One, SampleRateMilliseconds, name) {

            StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = Debug.GC(false);
        }

        protected override string GeoLocation() {
            return string.Empty;
        }

        public override double Current {
            get { return Debug.GC(false); }
        }

        protected override void SensorCleanup() {
        }
    }
}
