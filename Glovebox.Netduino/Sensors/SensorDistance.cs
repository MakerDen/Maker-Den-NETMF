using Glovebox.IoT;
using Glovebox.IoT.Base;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace Glovebox.Netduino.Sensors {
    public class SensorDistance : SensorBase {
        Drivers.HCSR04 sensor;
        public override double Current { get { return (sensor.Ping()); } }

        public delegate uint SensorEventHandler(object sender, EventArgs e);
        public event SensorEventHandler OnAfterMeasurement;
        public event SensorEventHandler OnBeforeMeasurement;


        /// <summary>
        /// Create and start a light senor
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="SampleRateMilliseconds">How often to measure in milliseconds or -1 to disable auto timed sensor readings</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public SensorDistance(Cpu.Pin echo, Cpu.Pin trigger, int SampleRateMilliseconds, string name)
            : base("ultrasonic", "p", ValuesPerSample.One, SampleRateMilliseconds, name) {
            sensor = new Drivers.HCSR04(echo, trigger);
            StartMeasuring();
        }

        protected override void Measure(double[] value) {

            Util.Delay(200);

            value[0] = (sensor.Ping() * 100);
        }

        protected override string GeoLocation() {
            return Util.RandomPostcode();
        }

        protected override void SensorCleanup() {
            if (sensor != null) { sensor.Dispose(); }
        }
    }
}
