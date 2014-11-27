using System;
using Microsoft.SPOT;
using GTM = Gadgeteer.Modules;
using Glovebox.MicroFramework;


namespace test {
    class SensorTemp : SensorBase {

        private Gadgeteer.Modules.GHIElectronics.TempHumidity tempHumidity = new GTM.GHIElectronics.TempHumidity(8);
        private double lastTempReading;
        private double lastHumidityReading;

        protected override double Current { get { return lastTempReading; } }

        public SensorTemp(int sampleRateMilliseconds)
            : base(SensorType.Temperature, ValuesPerSample.One, sampleRateMilliseconds) {

            tempHumidity.MeasurementComplete += tempHumidity_MeasurementComplete;
            tempHumidity.RequestSingleMeasurement();
            tempHumidity.MeasurementInterval = 1000 * 60 * 30;  // every 30 minutes
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
    }
}
