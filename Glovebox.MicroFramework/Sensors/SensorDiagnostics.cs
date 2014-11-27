using System;
using Microsoft.SPOT;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;
using Glovebox.MicroFramework.IoT;

namespace Glovebox.MicroFramework.Sensors {
    public class SensorDiagnostics : SensorBase {

        public SensorDiagnostics(int SampleRateMilliseconds, string name)
            : base(SensorType.Diagnostic, ValuesPerSample.Four, SampleRateMilliseconds, name) {

                StartMeasuring();
        }

        public SensorDiagnostics(int SampleRateMilliseconds)
            : base(SensorType.Diagnostic, ValuesPerSample.One, SampleRateMilliseconds, null) {

            StartMeasuring();
        }


        protected override void Measure(double[] value) {
            value[0] = SensorErrorCount;
            value[1] = IotList.ActionErrorCount;
            value[2] = TotalSensorMeasurements;
            value[3] = IotList.TotalActions;
            value[4] = Debug.GC(false);
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
