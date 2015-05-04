using Microsoft.SPOT.Hardware;

namespace Glovebox.Netduino.Sensors {
    public class SensorWaterLevel : Generic.SensorOneAnalogWire {

        public SensorWaterLevel(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base(pin, SampleRateMilliseconds, name, "waterlevel", "p") {
        }
    }
}
