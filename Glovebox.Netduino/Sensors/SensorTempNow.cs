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

        public override double Current { get { return ds.ConvertAndReadTemperature(); } }

        /// <summary>
        /// Create and start a temperature senor.  Non cached version
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="SampleRateMilliseconds">How often to measure in milliseconds or -1 to disable auto timed sensor readings</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public SensorTempNow(Cpu.Pin pin, int SampleRateMilliseconds, string name)
            : base("temp", "c", ValuesPerSample.One, SampleRateMilliseconds, name) {

                ds = new DS18B20(pin);
                StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = ds.ConvertAndReadTemperature();
        }

        protected override string GeoLocation() {
            return Utilities.RandomPostcode();
        }

        protected override void SensorCleanup() {
            ds.Dispose();
        }
    }
}
