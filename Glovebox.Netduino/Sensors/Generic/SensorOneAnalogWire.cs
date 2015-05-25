using Glovebox.IoT;
using Glovebox.IoT.Base;
using Microsoft.SPOT.Hardware;

namespace Glovebox.Netduino.Sensors.Generic {
    public class SensorOneAnalogWire : SensorBase {
        protected AnalogInput analogPin;

        public override double Current { get { return (int)(analogPin.Read() * 100); } }

        /// <summary>
        /// Create and start a generic 1 analog wire sensor
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="SampleRateMilliseconds">How often to measure in milliseconds or -1 to disable auto timed sensor readings</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public SensorOneAnalogWire(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name, string sensorType, string sensorUnit)
            : base(sensorType, sensorUnit, ValuesPerSample.One, SampleRateMilliseconds, name) {

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
