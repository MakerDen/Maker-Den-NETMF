using Glovebox.MicroFramework.Base;

namespace Glovebox.Gadgeteer.Sensors
{
    public class SensorError : SensorBase {

        public SensorError(int SampleRateMilliseconds, string name)
            : base(name, "n", ValuesPerSample.One, SampleRateMilliseconds, "error01") {

            StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = SensorErrorCount;
        }

        protected override string GeoLocation() {
            return string.Empty;
        }



        public override double Current
        {
            get { return SensorErrorCount; }
        }

        protected override void SensorCleanup()
        {

        }
    }
}
