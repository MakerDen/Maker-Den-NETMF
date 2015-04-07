using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

//http://blogs.msdn.com/b/laurelle/archive/2013/05/27/ultrasound-sensor-and-net-microframework-netmf.aspx
namespace Glovebox.Netduino.Drivers {
    public class HCSR04 : IDisposable {
        private OutputPort trigger;
        private InputPort echo;
        private long beginTick;
        private long endTick;
        private long minTicks = 0;  // System latency, subtracted off ticks to find actual sound travel time

        public HCSR04(Cpu.Pin echoPin, Cpu.Pin triggerPin)
        {

            trigger = new OutputPort(triggerPin, false);
            echo = new InterruptPort(echoPin, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeLow);
            echo.OnInterrupt += new NativeEventHandler(trigger_OnInterrupt);
            
            //trigger.OnInterrupt +=trigger_OnInterrupt;
            minTicks = 4000L;

            //echo = new InputPort(echoPin, false, Port.ResistorMode.Disabled);
            //trigger = new OutputPort(triggerPin, false);

        }

        private void trigger_OnInterrupt(uint data1, uint data2, DateTime time)

        {
            endTick = time.Ticks;

        }
    
        public void Dispose() {
            if (trigger != null)
                trigger.Dispose();
            if (echo!= null)
                echo.Dispose();
        }
        public long Ping()
        {
            // Reset Sensor
            trigger.Write(true);
            Thread.Sleep(1);

            // Start Clock
            endTick = 0L;
            beginTick = System.DateTime.Now.Ticks;
            // Trigger Sonic Pulse
            trigger.Write(false);

            // Wait 1/20 second (this could be set as a variable instead of constant)
            Thread.Sleep(50);

            if (endTick > 0L)
            {
                // Calculate Difference
                long elapsed = endTick - beginTick;

                // Subtract out fixed overhead (interrupt lag, etc.)
                elapsed -= minTicks;
                if (elapsed < 0L)
                {
                    elapsed = 0L;
                }

                // Return elapsed ticks
                return elapsed * 10 / 636;
                ;
            }

            // Sonic pulse wasn't detected within 1/20 second
            return -1L;
        }

        
        /*private static class Command {
            public const byte SearchROM = 0xF0;
            public const byte ReadROM = 0x33;
            public const byte MatchROM = 0x55;
            public const byte SkipROM = 0xCC;
            public const byte AlarmSearch = 0xEC;
            public const byte StartTemperatureConversion = 0x44;
            public const byte ReadScratchPad = 0xBE;
            public const byte WriteScratchPad = 0x4E;
            public const byte CopySratchPad = 0x48;
            public const byte RecallEEPROM = 0xB8;
            public const byte ReadPowerSupply = 0xB4;
        }*/
    }
}
