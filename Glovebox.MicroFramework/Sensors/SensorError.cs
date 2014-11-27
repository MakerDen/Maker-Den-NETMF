using System;
using Microsoft.SPOT;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;

namespace Glovebox.MicroFramework.Sensors {
    public class SensorError : SensorBase {

        protected override double Current { get { return (int)SensorErrorCount; } }

        public SensorError(int SampleRateMilliseconds, string name)
            : base(SensorType.Error, ValuesPerSample.One, SampleRateMilliseconds, name) {

                StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = SensorErrorCount;
        }

        protected override string GeoLocation() {
            return string.Empty;
        }

        protected override void SensorCleanup() {
        }
    }
}
