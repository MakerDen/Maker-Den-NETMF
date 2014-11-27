using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace Glovebox.Netduino {
    public class LedDigital : IDisposable {

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

        public LedDigital(Cpu.Pin pin) {
            ts.MyTimer = new Timer(new TimerCallback(BlinkTime_Tick), ts, Timeout.Infinite, Timeout.Infinite);
            ts.led = new OutputPort(pin, false);
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

        public void Dispose() {
            ts.led.Dispose();
        }
    }
}
