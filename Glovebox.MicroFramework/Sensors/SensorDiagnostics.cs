using Glovebox.MicroFramework.Base;
using Glovebox.MicroFramework.IoT;
using Microsoft.SPOT;

namespace Glovebox.MicroFramework.Sensors {
    public class SensorDiagnostics : SensorBase {

        public SensorDiagnostics(int SampleRateMilliseconds, string name)
            : base("diag", "g", ValuesPerSample.Five, SampleRateMilliseconds, name) {

            StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = SensorErrorCount;
            value[1] = IotActionManager.ActionErrorCount;
            value[2] = TotalSensorMeasurements;
            value[3] = IotActionManager.TotalActions;
            value[4] = Debug.GC(false);
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
