using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;

namespace Glovebox.Netduino.Sensors {
    public class SensorLight : SensorBase {
        private AnalogInput analogPin;

        public override double Current { get { return (int)(analogPin.Read() * 100); } }

        /// <summary>
        /// Create and start a light senor
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="SampleRateMilliseconds">How often to measure in milliseconds or -1 to disable auto timed sensor readings</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public SensorLight(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base("light", "p", ValuesPerSample.One, SampleRateMilliseconds, name) {

                analogPin = new AnalogInput(pin, -1);
                StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = (int)(analogPin.Read() * 100);
        }

        protected override string GeoLocation() {
            return Utilities.RandomPostcode();
        }

        protected override void SensorCleanup() {
            if (analogPin != null) { analogPin.Dispose(); }
        }
    }
}
