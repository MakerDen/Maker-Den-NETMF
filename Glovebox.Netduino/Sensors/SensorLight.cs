using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;

namespace Glovebox.Netduino.Sensors {
    public class SensorLight : Generic.SensorOneAnalogWire {
        private AnalogInput analogPin;

        public SensorLight(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base(pin, SampleRateMilliseconds, name, "light", "p")
        {

                analogPin = new AnalogInput(pin, -1);
                StartMeasuring();
        }

    }
}
