using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework;
using Glovebox.Netduino;
using Glovebox.MicroFramework.Base;
using Glovebox.Netduino.Drivers;


namespace Glovebox.Netduino.Sensors {
    public class SensorTempNow : SensorBase {

        DS18B20 ds = null;

        protected override double Current { get { return ds.ConvertAndReadTemperature(); } }

        public SensorTempNow(Cpu.Pin pin, int SampleRateMilliseconds, string name)
            : base(SensorType.Temperature, ValuesPerSample.One, SampleRateMilliseconds, name) {

                ds = new DS18B20(pin);
                StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = ds.ConvertAndReadTemperature();
        }

        protected override string GeoLocation() {
            return Utilities.RandomPostcode();
        }

        //void IDisposable.Dispose() {
        //    ds.Dispose();
        //}

        protected override void SensorCleanup() {
            ds.Dispose();
        }
    }
}
