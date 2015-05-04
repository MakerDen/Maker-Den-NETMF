using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;

namespace Glovebox.Netduino.Sensors {
    public class SensorMoisture: Generic.SensorOneAnalogWire {
        
        public SensorMoisture(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base(pin, SampleRateMilliseconds, name, "moisture", "p")
        {

        }

        
    }
}
