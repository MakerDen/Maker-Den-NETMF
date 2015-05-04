using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;
using System;
using GTM = Gadgeteer.Modules;

namespace Glovebox.Gadgeteer.Sensors
{
    public class SensorLight : SensorBase, IDisposable
    {
        private GTM.GHIElectronics.LightSense lightSense;


        public SensorLight(GTM.GHIElectronics.LightSense lightSense, int sampleRateMilliseconds, string name)
            : base(light", "p", ValuesPerSample.One, sampleRateMilliseconds, name)
        {

            this.lightSense = lightSense;

            this.lightSense = new GTM.GHIElectronics.LightSense(14);
            StartMeasuring();

        }

        protected override void Measure(double[] value)
        {
            value[0] = lightSense.ReadProportion() * 100;
        }

        protected override string GeoLocation()
        {
            return Utilities.RandomPostcode();
        }

        void IDisposable.Dispose()
        {

        }

        public override double Current
        {
            get { return (int)(lightSense.ReadProportion() * 100);  }
        }

        protected override void SensorCleanup()
        {

        }
    }
}
