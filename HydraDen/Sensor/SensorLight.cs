using System;
using Microsoft.SPOT;
using GTM = Gadgeteer.Modules;
using Glovebox.MicroFramework;

namespace test {
    class SensorLight : SensorBase, IDisposable {
        private Gadgeteer.Modules.GHIElectronics.LightSense lightSense;

        protected override double Current { get { return (int)(lightSense.ReadProportion() * 100); } }

        public SensorLight(int sampleRateMilliseconds)
            : base(SensorType.Light, ValuesPerSample.One, sampleRateMilliseconds) {

            this.lightSense = new GTM.GHIElectronics.LightSense(14);
            StartMeasuring();

        }

        protected override void Measure(double[] value) {
            value[0] = lightSense.ReadProportion() * 100;
        }

        protected override string GeoLocation() {
            return Utilities.RandomPostcode();
        }

        void IDisposable.Dispose() {

        }
    }
}
