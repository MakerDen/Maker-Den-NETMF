///Author: Russell Peake
///Email: rdpeake@adam.com.au
///Adapted from andrew.f.stace@gmail.com
using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework.Base;
using Glovebox.MicroFramework.IoT;

namespace Glovebox.Netduino.Actuators
{
    public class RgbLedPwm : RgbLed
    {
        // The PulsePeriodInMicroseconds is the length of the pulse in microseconds
        // a pulse duration is a fraction on the pulse period and determines the brightness level
        // a pulse duration of 0 mean 0 milliseconds of the pulse period the LED will be high, hence the LED will be off
        // a pulse duration of 1000 would mean the pulse is high for 100% of the pulse period, hence LED full brightness
        // a pulse duration of 500 would mean the pulse is high for 50% of the pulse period, hence LED half brightness
        const uint PulsePeriodInMicroseconds = 1000;

        class pwmledState : ledState
        {
            internal enum CmdType
            {
                Fade,
                Blink
            }

            public BlinkRate blinkRate;
            internal CmdType cmd;
            public new PWM led;
            public bool Cancel { get; set; }
            public uint milliseconds = 0;
            public uint endPulseDuration = 1000;
            public uint startPulseDuration = 0;

            public override void Start()
            {

                while (true)
                {
                    blink.WaitOne();
                    running = true;
                    Cancel = false;

                    switch (cmd)
                    {
                        case CmdType.Fade:
                            RunFade();
                            break;
                        case CmdType.Blink:
                            RunBlink();
                            break;
                    }

                    running = false;
                }
            }

            protected void RunFade()
            {
                const uint PulseStep = 50; // milliseconds
                int currentTickCount;
                int range = (int)(endPulseDuration - startPulseDuration);
                uint segments = milliseconds / PulseStep; // so chop total milliseconds in to 10 millisecond segments
                int increment = (int)(range / segments);

                currentTickCount = Environment.TickCount;

                for (int i = (int)startPulseDuration; Environment.TickCount < currentTickCount + milliseconds && !Cancel; i += increment)
                {
                    led.Duration = (uint)i;
                    Thread.Sleep((int)PulseStep);
                }

                led.Duration = endPulseDuration;
            }

            protected void RunBlink()
            {
                int blinkRateMilliseconds = CalculateBlinkRate(blinkRate);
                int currentTickCount = Environment.TickCount;
                uint level = led.Duration;
                bool ledOn = true;

                while (Environment.TickCount < currentTickCount + milliseconds && !Cancel)
                {
                    Thread.Sleep(blinkRateMilliseconds);
                    ledOn = !ledOn;
                    led.Duration = ledOn ? level : 0;
                }
                led.Duration = 0;
            }

            int CalculateBlinkRate(BlinkRate rate)
            {
                int br = 500;
                switch (rate)
                {
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


        public RgbLedPwm(Cpu.PWMChannel red, Cpu.PWMChannel green, Cpu.PWMChannel blue, string name)
            //RgbLed(Cpu.Pin red, Cpu.Pin green, Cpu.Pin blue, string name)
            : base((Cpu.Pin)red, (Cpu.Pin)green, (Cpu.Pin)blue, name, "rgbledpwm")  {
            ls[0] = new pwmledState() { led = new PWM(red, PulsePeriodInMicroseconds, 0, PWM.ScaleFactor.Microseconds, false) };
            ls[1] = new pwmledState() { led = new PWM(green, PulsePeriodInMicroseconds, 0, PWM.ScaleFactor.Microseconds, false) };
            ls[2] = new pwmledState() { led = new PWM(blue, PulsePeriodInMicroseconds, 0, PWM.ScaleFactor.Microseconds, false) };

            ((pwmledState)ls[0]).led.Start();
            ((pwmledState)ls[1]).led.Start();
            ((pwmledState)ls[2]).led.Start();

            }


        #region Blink

        public override void Blink(Led l, int Milliseconds, BlinkRate blinkRate)
        {
            Blink(l, 100, blinkRate, (uint)Milliseconds);
        }
        
        /// <summary>
        /// Blink an LED
        /// </summary>
        /// <param name="l"></param>
        /// <param name="levelPercentage">as a percentage 0 = off, 100 = max on</param>
        /// <param name="blinkRate"></param>
        /// <param name="milliseconds">the time the blink sequence will run for</param>
        public void Blink(Led l, byte levelPercentage, BlinkRate blinkRate, uint milliseconds)
        {
            levelPercentage = (byte)(levelPercentage % 101);

            if (!StartRunThread(l)) { return; }

            ((pwmledState)ls[(int)l]).cmd = pwmledState.CmdType.Blink;
            ((pwmledState)ls[(int)l]).led.Duration = (uint)(levelPercentage * 10);  // scale duration to PulsePeriodInMicroseconds
            ((pwmledState)ls[(int)l]).blinkRate = blinkRate;
            ((pwmledState)ls[(int)l]).milliseconds = milliseconds;


            ls[(int)l].blink.Set();
        }

        #endregion

        #region on/off

        public override void On(Led l)
        {
            if (ls[(int)l].running) { return; }
            ((pwmledState)ls[(int)l]).led.Duration = 100;
        }

        public override void Off(Led l)
        {
            if (ls[(int)l].running) { return; }
            ((pwmledState)ls[(int)l]).led.Duration = 0;
        }

        #endregion

        #region FADE
        /// <summary>
        /// Run an LED fade in out sequence
        /// </summary>
        /// <param name="l"></param>
        /// <param name="startLevelPercentage">start LED brightness level as a percentage, 0 = off, 100 = max brightness</param>
        /// <param name="endLevelPercentage">end LED brightness level as a percentage, 0 = off, 100 = max brightness</param>
        /// <param name="milliseconds"></param>
        public void StartFade(Led l, uint startLevelPercentage, uint endLevelPercentage, uint milliseconds)
        {

            if (!StartRunThread(l)) { return; }

            ((pwmledState)ls[(int)l]).cmd = pwmledState.CmdType.Fade;
            ((pwmledState)ls[(int)l]).milliseconds = milliseconds;
            ((pwmledState)ls[(int)l]).startPulseDuration = (startLevelPercentage % 101) * 10;  // scale duration to PulsePeriodInMicroseconds
            ((pwmledState)ls[(int)l]).endPulseDuration = (endLevelPercentage % 101) * 10;  // scale duration to PulsePeriodInMicroseconds

            ls[(int)l].blink.Set();
        }
        #endregion

        #region colour
        public void SetColour(byte red, byte green, byte blue)
        {
            const double Scaler = PulsePeriodInMicroseconds / 256;  // 256 = max byte type size

            for (int i = 0; i < 3; i++)
            {
                if (ls[i] == null || ((pwmledState)ls[i]).led == null || ls[i].running) { return; }
            }

            ((pwmledState)ls[0]).led.Duration = (uint)(red * Scaler);
            ((pwmledState)ls[1]).led.Duration = (uint)(green * Scaler);
            ((pwmledState)ls[2]).led.Duration = (uint)(blue * Scaler);
        }


        public void SetColour(Led l, byte level)
        {
            level = (byte)(level % 101);

            if (ls[(int)l] == null || ((pwmledState)ls[(int)l]).led == null || ls[(int)l].running) { return; }
            ((pwmledState)ls[(int)l]).led.Duration = (uint)(level * 10);  // scale duration to PulsePeriodInMicroseconds
        }
        #endregion

        #region cancel
        public void Cancel(Led l)
        {
            if (ls[(int)l] == null) { return; }
            ((pwmledState)ls[(int)l]).Cancel = true;
        }

        public void Cancel()
        {
            for (int i = 0; i < 3; i++)
            {
                if (ls[i] == null) { continue; }
                ((pwmledState)ls[i]).Cancel = true;
            }
        }
        #endregion

        #region Util
        protected override void ActuatorCleanup()
        {
            for (int i = 0; i < 3; i++)
            {
                if (((pwmledState)ls[i]).led != null) { ((pwmledState)ls[i]).led.Dispose(); }
            }
        }

        private bool StartRunThread(Led l)
        {
            if (ls[(int)l] == null || ((pwmledState)ls[(int)l]).led == null || ls[(int)l].running) { return false; }

            if (ls[(int)l].ledThread == null)
            {
                ls[(int)l].ledThread = new Thread(new ThreadStart(ls[(int)l].Start));
                ls[(int)l].ledThread.Priority = ThreadPriority.Lowest;
                ls[(int)l].ledThread.Start();
            }
            return true;
        }

        public override void Action(IotAction action)
        {
            if (action.subItem == string.Empty) { return; }
            uint colourIndex = 0;
            string colourName = action.subItem;
            string[] colours = new string[] { "red", "green", "blue" };
            for (colourIndex = 0; colourIndex < colours.Length; colourIndex++)
            {
                if (colourName == colours[colourIndex]) { break; }
            }
            if (colourIndex > 2) { return; }
            switch (action.cmd)
            {
                case "fade":
                    //get params
                    break;
                default:
                    base.Action(action);
                    break;
            }
        }

        #endregion

    }
}
