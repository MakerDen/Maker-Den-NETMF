using Glovebox.MicroFramework.Base;
using GTM = Gadgeteer.Modules;


namespace Glovebox.Gadgeteer.Sensors {
    public class SensorTemp : SensorBase {

        private double lastTempReading;
        private double lastHumidityReading;


        public SensorTemp(GTM.GHIElectronics.TempHumidity tempHumidity, int sampleRateMilliseconds, string name)
            : base("temp", "c", ValuesPerSample.One, sampleRateMilliseconds, name) {

            tempHumidity.MeasurementComplete += tempHumidity_MeasurementComplete;

//            tempHumidity.RequestSingleMeasurement();

            tempHumidity.MeasurementInterval = 5000;
            tempHumidity.StartTakingMeasurements();

            StartMeasuring();
        }

        void tempHumidity_MeasurementComplete(GTM.GHIElectronics.TempHumidity sender, GTM.GHIElectronics.TempHumidity.MeasurementCompleteEventArgs e) {
            lastTempReading = e.Temperature;
            lastHumidityReading = e.RelativeHumidity;
        }

        protected override void Measure(double[] value) {
            value[0] = lastTempReading;
            //value[1] = lastHumidityReading;
        }

        protected override string GeoLocation() {
            return string.Empty;
        }

        public override double Current {
            get { return lastTempReading; }
        }

        protected override void SensorCleanup() {
        }
    }
}
