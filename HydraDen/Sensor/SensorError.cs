using System;
using Microsoft.SPOT;
using Glovebox.MicroFramework;

namespace test {
    public class SensorError : SensorBase {

        public SensorError(int SampleRateMilliseconds)
            : base(SensorType.Error, ValuesPerSample.One, SampleRateMilliseconds) {

            StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = errorCount;
        }

        protected override string GeoLocation() {
            return string.Empty;
        }

        protected override double Current {
            get { return errorCount; }
        }
    }
}
