// andrew.f.stace@gmail.com
using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework.Base;

namespace Glovebox.Netduino {
    public class RgbLedPwm : IDisposable {
        // The PulsePeriodInMicroseconds is the length of the pulse in microseconds
        // a pulse duration is a fraction on the pulse period and determines the brightness level
        // a pulse duration of 0 mean 0 milliseconds of the pulse period the LED will be high, hence the LED will be off
        // a pulse duration of 1000 would mean the pulse is high for 100% of the pulse period, hence LED full brightness
        // a pulse duration of 500 would mean the pulse is high for 50% of the pulse period, hence LED half brightness
        const uint PulsePeriodInMicroseconds = 1000; 
        

        public enum Led {
            Red,
            Green,
            Blue
        }

        class ledState {
            internal enum CmdType {
                Fade,
                Blink
            }

            public BlinkRate blinkRate;
            internal CmdType cmd;
            public PWM led;
            public Thread ledThread;
            public AutoResetEvent sequence = new AutoResetEvent(false);
            public bool Running { get; private set; }
            public bool Cancel { get; set; }
            public uint milliseconds = 0;
            public uint endPulseDuration = 1000;
            public uint startPulseDuration = 0;

            public void Start() {

                while (true) {
                    sequence.WaitOne();
                    Running = true;
                    Cancel = false;

                    switch (cmd) {
                        case CmdType.Fade:
                            RunFade();
                            break;
                        case CmdType.Blink:
                            RunBlink();
                            break;
                    }

                    Running = false;
                }
            }

            private void RunFade() {
                const uint PulseStep = 50; // milliseconds
                int currentTickCount;
                int range = (int)(endPulseDuration - startPulseDuration);
                uint segments = milliseconds / PulseStep; // so chop total milliseconds in to 10 millisecond segments
                int increment = (int)(range / segments);

                currentTickCount = Environment.TickCount;

                for (int i = (int)startPulseDuration; Environment.TickCount < currentTickCount + milliseconds && !Cancel; i += increment) {
                    led.Duration = (uint)i;
                    Thread.Sleep((int)PulseStep);
                }

                led.Duration = endPulseDuration;
            }

            private void RunBlink() {
                int blinkRateMilliseconds = CalculateBlinkRate(blinkRate);
                int currentTickCount = Environment.TickCount;
                uint level = led.Duration;
                bool ledOn = true;

                while (Environment.TickCount < currentTickCount + milliseconds && !Cancel) {
                    Thread.Sleep(blinkRateMilliseconds);
                    ledOn = !ledOn;
                    led.Duration = ledOn ? level : 0;
                }
                led.Duration = 0;
            }

            int CalculateBlinkRate(BlinkRate rate) {
                int br = 500;
                switch (rate) {
                    case BlinkRate.VerySlow:
                        br = 1000;
                        break;
                    case BlinkRate.Slow:
                        br = 500;
                        break;
                    case BlinkRate.Medium:
                        br = 250;
                        break;
                    case BlinkRate.Fast:
                        br = 75;
                        break;
                    case BlinkRate.VeryFast:
                        br = 25;
                        break;
                }
                return br;
            }
        }

        ledState[] ls = new ledState[3];

        public enum BlinkRate {
            VerySlow,
            Slow,
            Medium,
            Fast,
            VeryFast
        }

        public RgbLedPwm(Cpu.PWMChannel red, Cpu.PWMChannel green, Cpu.PWMChannel blue, string name) {

            ls[0] = new ledState() { led = new PWM(red, PulsePeriodInMicroseconds, 0, PWM.ScaleFactor.Microseconds, false) };
            ls[1] = new ledState() { led = new PWM(green, PulsePeriodInMicroseconds, 0, PWM.ScaleFactor.Microseconds, false) };
            ls[2] = new ledState() { led = new PWM(blue, PulsePeriodInMicroseconds, 0, PWM.ScaleFactor.Microseconds, false) };

            ls[0].led.Start();
            ls[1].led.Start();
            ls[2].led.Start();

        }

        /// <summary>
        /// Run an LED fade in out sequence
        /// </summary>
        /// <param name="l"></param>
        /// <param name="startLevelPercentage">start LED brightness level as a percentage, 0 = off, 100 = max brightness</param>
        /// <param name="endLevelPercentage">end LED brightness level as a percentage, 0 = off, 100 = max brightness</param>
        /// <param name="milliseconds"></param>
        public void StartFade(Led l, uint startLevelPercentage, uint endLevelPercentage, uint milliseconds) {

            if (!StartRunThread(l)) { return; }    

            ls[(int)l].cmd = ledState.CmdType.Fade;
            ls[(int)l].milliseconds = milliseconds;
            ls[(int)l].startPulseDuration = (startLevelPercentage % 101) * 10;  // scale duration to PulsePeriodInMicroseconds
            ls[(int)l].endPulseDuration = (endLevelPercentage % 101) * 10;  // scale duration to PulsePeriodInMicroseconds

            ls[(int)l].sequence.Set();
        }

        /// <summary>
        /// Blink an LED
        /// </summary>
        /// <param name="l"></param>
        /// <param name="levelPercentage">as a percentage 0 = off, 100 = max on</param>
        /// <param name="blinkRate"></param>
        /// <param name="milliseconds">the time the blink sequence will run for</param>
        public void StartBlink(Led l, byte levelPercentage, BlinkRate blinkRate, uint milliseconds) {
            levelPercentage = (byte)(levelPercentage % 101);

            if (!StartRunThread(l)) { return; }

            ls[(int)l].cmd = ledState.CmdType.Blink;
            ls[(int)l].led.Duration = (uint)(levelPercentage * 10);  // scale duration to PulsePeriodInMicroseconds
            ls[(int)l].blinkRate = blinkRate;
            ls[(int)l].milliseconds = milliseconds;


            ls[(int)l].sequence.Set();
        }

        public void SetColour(byte red, byte green, byte blue) {
            const double Scaler = PulsePeriodInMicroseconds / 256;  // 256 = max byte type size

            for (int i = 0; i < 3; i++) {
                if (ls[i] == null || ls[i].led == null || ls[i].Running) { return; }
            }

            ls[0].led.Duration = (uint)(red * Scaler);
            ls[1].led.Duration = (uint)(green * Scaler);
            ls[2].led.Duration = (uint)(blue * Scaler);
        }


        public void SetColour(Led l, byte level) {
            level = (byte)(level % 101);

            if (ls[(int)l] == null || ls[(int)l].led == null || ls[(int)l].Running) { return; }
            ls[(int)l].led.Duration = (uint)(level * 10);  // scale duration to PulsePeriodInMicroseconds
        }

      

        private bool StartRunThread(Led l) {
            if (ls[(int)l] == null || ls[(int)l].led == null || ls[(int)l].Running) { return false; }

            if (ls[(int)l].ledThread == null) {
                ls[(int)l].ledThread = new Thread(new ThreadStart(ls[(int)l].Start));
                ls[(int)l].ledThread.Priority = ThreadPriority.Lowest;
                ls[(int)l].ledThread.Start();
            }
            return true;
        }

        public void Cancel(Led l) {
            if (ls[(int)l] == null) { return; }
            ls[(int)l].Cancel = true;
        }

        public void Cancel() {
            for (int i = 0; i < 3; i++) {
                if (ls[i] == null) { continue; }
                ls[i].Cancel = true;
            }
        }

        void IDisposable.Dispose() {
            for (int i = 0; i < 3; i++) {
                if (ls[i].led != null) {
                    ls[i].led.Stop();
                    ls[i].led.Dispose();
                }
            }
        }
    }
}
