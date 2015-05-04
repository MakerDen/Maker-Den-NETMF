using Microsoft.SPOT.Hardware;

namespace Glovebox.Netduino.Sensors {
    public class SensorMoisture : Generic.SensorOneAnalogWire {

        public SensorMoisture(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base(pin, SampleRateMilliseconds, name, "moisture", "p") {

        }
    }
}
