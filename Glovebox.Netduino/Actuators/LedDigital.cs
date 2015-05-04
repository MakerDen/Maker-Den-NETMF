using Glovebox.MicroFramework.Base;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace Glovebox.Netduino.Actuators {
    public class LedDigital : ActuatorBase {

        class ledState {
            public uint blinkMilliseconds = 0;
            public int BlinkMillisecondsToDate;
            public bool ledOn = false;
            public OutputPort led;
            public Timer MyTimer;
            public int blinkRateMilliseconds;
            public bool running = false;
        }

        ledState ts = new ledState();

        public enum BlinkRate {
            Slow,
            Medium,
            Fast,
            VeryFast
        }

        /// <summary>
        /// Simnple Led control
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.Pins namespace</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public LedDigital(Cpu.Pin pin, string name)
            : base(name, "led") {
            ts.MyTimer = new Timer(new TimerCallback(BlinkTime_Tick), ts, Timeout.Infinite, Timeout.Infinite);
            ts.led = new OutputPort(pin, false);
        }

        public void On() {
            if (ts.running) { return; }
            ts.led.Write(true);
        }

        public void Off() {
            if (ts.running) { return; }
            ts.led.Write(false);
        }

        public void BlinkOn(uint Milliseconds, BlinkRate blinkRate) {

            if (ts.running) { return; }
            ts.running = true;

            ts.blinkMilliseconds = Milliseconds;
            ts.BlinkMillisecondsToDate = 0;
            ts.blinkRateMilliseconds = CalculateBlinkRate(blinkRate);
            ts.MyTimer.Change(0, ts.blinkRateMilliseconds);
        }

        void BlinkTime_Tick(object state) {
            var ts = (ledState)state;

            ts.led.Write(!ts.ledOn);
            ts.ledOn = !ts.ledOn;

            ts.BlinkMillisecondsToDate += ts.blinkRateMilliseconds;
            if (ts.BlinkMillisecondsToDate >= ts.blinkMilliseconds) {
                // turn off blink
                ts.MyTimer.Change(Timeout.Infinite, Timeout.Infinite);
                ts.led.Write(false);
                ts.running = false;
            }
        }

        int CalculateBlinkRate(BlinkRate rate) {
            int br = 500;
            switch (rate) {
                case BlinkRate.Slow:
                    br = 1000;
                    break;
                case BlinkRate.Medium:
                    br = 500;
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
            ts.led.Dispose();
        }

        public override void Action(MicroFramework.IoT.IotAction action) {
            // no actions implemented
        }
    }
}
