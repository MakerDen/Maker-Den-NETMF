using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;
using Glovebox.MicroFramework.IoT;

namespace Glovebox.Netduino {
    public class RgbLed : ActuatorBase {

        class ledState {
            public int blinkMilliseconds = 0;
            public int blinkRateMilliseconds;
            private bool ledOn = false;
            public OutputPort led;
            public Thread ledThread;
            public AutoResetEvent blink = new AutoResetEvent(false);
            public bool running = false;

            public void Start() {
                int currentTickCount;
                int endTickCount;
                int blinkRate;

                while (true) {
                    blink.WaitOne();
                    running = true;

                    currentTickCount = Environment.TickCount;
                    endTickCount = currentTickCount + blinkMilliseconds;
                    blinkRate = blinkRateMilliseconds;

                    while (currentTickCount < endTickCount) {
                        led.Write(ledOn = !ledOn);
                        Thread.Sleep(blinkRate);
                        currentTickCount = Environment.TickCount;
                    }
                    led.Write(false);
                    running = false;
                }
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

        public enum Led {
            Red,
            Green,
            Blue
        }

        public RgbLed(Cpu.Pin red, Cpu.Pin green, Cpu.Pin blue, string name)
            : base(name, ActuatorType.RgbLed) {
            Cpu.Pin[] ledPins = new Cpu.Pin[] { red, green, blue };
            for (int i = 0; i < 3; i++) {
                ls[i] = new ledState();
                ls[i].led = new OutputPort(ledPins[i], false);
            }
        }

        public void Blink(Led l, int Milliseconds, BlinkRate blinkRate) {
            //lazy start thread ondemand
            if (ls[(int)l].ledThread == null) {
                ls[(int)l].ledThread = new Thread(new ThreadStart(ls[(int)l].Start));
                ls[(int)l].ledThread.Start();
            }

            ls[(int)l].blinkMilliseconds = Milliseconds;
            ls[(int)l].blinkRateMilliseconds = CalculateBlinkRate(blinkRate);

            ls[(int)l].blink.Set();
        }

        public void On(Led l) {
            if (ls[(int)l].running) { return; }
            ls[(int)l].led.Write(true);
        }

        public void Off(Led l) {
            if (ls[(int)l].running) { return; }
            ls[(int)l].led.Write(false);
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

        protected override void ActuatorCleanup() {
            for (int i = 0; i < 3; i++) {
                if (ls[i].led != null) { ls[i].led.Dispose(); }
            }
        }

        public override void Action(IotAction action) {
            if (action.subItem == string.Empty) { return; }
            uint colourIndex = 0;
            string colourName = action.subItem;
            string[] colours = new string[] { "red", "green", "blue" };
            for (colourIndex = 0; colourIndex < colours.Length; colourIndex++) {
                if (colourName == colours[colourIndex]) { break; }
            }
            if (colourIndex > 2) { return; }
            switch (action.cmd) {
                case "on":
                    On((Led)colourIndex);
                    break;
                case "off":
                    Off((Led)colourIndex);
                    break;
                case "blink":
                    // get rate and duration from action.params
                    break;
                default:
                    break;
            }
        }
    }
}
