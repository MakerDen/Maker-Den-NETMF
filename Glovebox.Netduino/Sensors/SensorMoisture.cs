using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;

namespace Glovebox.Netduino.Sensors {
    public class SensorMoisture: Generic.SensorOneAnalogWire {
        private AnalogInput analogPin;

        
        /// <summary>
        /// Create and start a light senor
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="SampleRateMilliseconds">How often to measure in milliseconds or -1 to disable auto timed sensor readings</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public SensorMoisture(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base(pin, SampleRateMilliseconds, name, "moisture", "p")
        {

        }

        
    }
}
