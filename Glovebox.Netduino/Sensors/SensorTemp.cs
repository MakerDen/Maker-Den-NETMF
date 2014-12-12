using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework;
using Glovebox.Netduino;
using Glovebox.MicroFramework.Base;
using Glovebox.Netduino.Drivers;


namespace Glovebox.Netduino.Sensors {
    public class SensorTemp : SensorBase {
        const uint temperatureCacheSeconds = 60 * 10;

        DS18B20 ds = null;
        DateTime nextTemperatureReading = DateTime.MinValue;
        float lastTemperatureReadingValue = 0.0f;

        public override double Current { get { return GetTemperature(); } }

        /// <summary>
        /// Create and start a temperature senor.  Note temperature is cached for 10 minutes
        /// as temperature chip tends to heat up if read too often
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="SampleRateMilliseconds">How often to measure in milliseconds or -1 to disable auto timed sensor readings</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public SensorTemp(Cpu.Pin pin, int SampleRateMilliseconds, string name)
            : base(SensorType.Temperature, ValuesPerSample.One, SampleRateMilliseconds, name) {

                ds = new DS18B20(pin);
                StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = GetTemperature();
        }

        protected override string GeoLocation() {
            return Utilities.RandomPostcode();
        }

        private double GetTemperature() {
            DateTime now = DateTime.Now;
            // cache temperature 
            if (now > nextTemperatureReading) {
                lastTemperatureReadingValue = ds.ConvertAndReadTemperature();
                nextTemperatureReading = now.AddSeconds(temperatureCacheSeconds);
            }
            return lastTemperatureReadingValue;
        }

        //void IDisposable.Dispose() {
        //    ds.Dispose();
        //}

        protected override void SensorCleanup() {
            ds.Dispose();
        }
    }
}
