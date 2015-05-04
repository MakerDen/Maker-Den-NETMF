using Microsoft.SPOT.Hardware;

namespace Glovebox.Netduino.Sensors {
    public class SensorLight : Generic.SensorOneAnalogWire {

        public SensorLight(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base(pin, SampleRateMilliseconds, name, "light", "p") {

        }
    }
}
