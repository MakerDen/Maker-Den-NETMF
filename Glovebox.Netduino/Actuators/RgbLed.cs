using Glovebox.MicroFramework.Base;
using Glovebox.MicroFramework.IoT;
using Microsoft.SPOT.Hardware;
using System;
using System.Threading;

namespace Glovebox.Netduino.Actuators {
    public class RgbLed : ActuatorBase {

        internal class ledState {
            public int blinkMilliseconds = 0;
            public int blinkRateMilliseconds;
            private bool ledOn = false;
            public OutputPort led;
            public Thread ledThread;
            public AutoResetEvent blink = new AutoResetEvent(false);
            public bool running { get; protected set; }

            public virtual void Start() {
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

        internal ledState[] ls = new ledState[3];


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

        /// <summary>
        /// Create a RGB LED Control.  Supports On/off and blink manager
        /// </summary>
        /// <param name="red">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="green">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="blue">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public RgbLed(Cpu.Pin red, Cpu.Pin green, Cpu.Pin blue, string name)
            : base(name, "rgbled") {
            Cpu.Pin[] ledPins = new Cpu.Pin[] { red, green, blue };
            for (int i = 0; i < 3; i++) {
                ls[i] = new ledState();
                ls[i].led = new OutputPort(ledPins[i], false);
            }
        }

        internal RgbLed(Cpu.Pin red, Cpu.Pin green, Cpu.Pin blue, string name, string type) : base(name, type) { }

        public virtual void Blink(Led l, int Milliseconds, BlinkRate blinkRate) {
            //lazy start thread ondemand
            if (ls[(int)l].ledThread == null) {
                ls[(int)l].ledThread = new Thread(new ThreadStart(ls[(int)l].Start));
                ls[(int)l].ledThread.Start();
            }

            ls[(int)l].blinkMilliseconds = Milliseconds;
            ls[(int)l].blinkRateMilliseconds = CalculateBlinkRate(blinkRate);

            ls[(int)l].blink.Set();
        }

        public virtual void On(Led l) {
            if (ls[(int)l].running) { return; }
            ls[(int)l].led.Write(true);
        }

        public virtual void Off(Led l) {
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
