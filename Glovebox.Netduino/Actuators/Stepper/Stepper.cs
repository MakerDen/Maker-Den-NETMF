using Glovebox.MicroFramework.Base;
using Microsoft.SPOT.Hardware;
using System.Threading;

//#acknowledgement: http://blog.codeblack.nl/post/Netduino-Getting-Started-with-steppermotors.aspx
namespace Glovebox.Netduino.Actuators {
    public class Stepper : ActuatorBase {
        #region Private fields

        private uint usPerStep = 0;

        private int currentStep;

        private OutputPort portCoil1A;
        private OutputPort portCoil1B;
        private OutputPort portCoil2A;
        private OutputPort portCoil2B;

        #endregion

        #region Properties

        /// <summary>
        /// Steps per revolution
        /// </summary>
        public readonly uint StepsPerRevolution;

        #endregion

        #region Constructors

        public Stepper(Cpu.Pin pinCoil1A, Cpu.Pin pinCoil1B, Cpu.Pin pinCoil2A, Cpu.Pin pinCoil2B, uint stepsPerRevolution, string name)
            : base(name, "stepper") {
            // remember the steps per revolution
            this.StepsPerRevolution = stepsPerRevolution;

            // reset the step-counter
            this.currentStep = 0;

            // determine correct pins for the coils; turn off all motor pins
            this.portCoil1A = new OutputPort(pinCoil1A, false);

            Thread.Sleep(500);
            this.portCoil1B = new OutputPort(pinCoil1B, false);

            Thread.Sleep(500);
            this.portCoil2A = new OutputPort(pinCoil2A, false);

            Thread.Sleep(500);

            this.portCoil2B = new OutputPort(pinCoil2B, false);



            // set the maximum speed
            SetSpeed(120);
        }
        protected override void ActuatorCleanup() {
            this.portCoil1A.Dispose();
            this.portCoil1B.Dispose();
            this.portCoil2A.Dispose();
            this.portCoil2B.Dispose();
        }
        public override void Action(Glovebox.MicroFramework.IoT.IotAction action) {
            switch (action.cmd.ToLower()) {
                case "forward":
                    Step(this.StepsPerRevolution, MotorDirection.Forward);
                    break;
                case "reverse":
                    Step(this.StepsPerRevolution, MotorDirection.Reverse);
                    break;
                case "release":
                    Step(this.StepsPerRevolution, MotorDirection.Release);
                    break;
            }
        }

        public Stepper(uint stepsPerRevolution, string name)
            : this(Cpu.Pin.GPIO_Pin0, Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin2, Cpu.Pin.GPIO_Pin3, stepsPerRevolution, name) {
        }

        #endregion

        #region Methods

        public void Step(uint steps, MotorDirection direction, StepType style = StepType.Single) {
            uint usPerStep = this.usPerStep;

            if (style == StepType.Interleave) {
                // corrections for interleave; interleave contains twice the steps (both single & double), so wait only halve the time
                usPerStep /= 2;
            }

            int waitTime = (int)usPerStep / 1000;
            if (waitTime <= 0) {
                waitTime = 1;
            }

            while (steps-- > 0) {
                // execute a single step
                OneStep(direction, style);

                // wait for a single step (convert us to ms)
                Thread.Sleep(waitTime);
            }
        }

        public void SetSpeed(uint speed) {
            // maximize the speed at 120
            // a higher speed will cause a wait-time of 0 ms for each step, which will not allow a step to take place
            // for interleave a 0 ms wait-time works, but it will not run faster then 120
            if (speed > 120) {
                speed = 120;
            }

            // convert rpm to us per step
            usPerStep = 24000000 / (StepsPerRevolution * speed);
        }

        #endregion

        #region Protected methods

        protected int OneStep(MotorDirection direction, StepType stepStyle = StepType.Single) {
            switch (stepStyle) {
                // single coil activation only uses the even stages (0, 2, 4, 6)
                case StepType.Single:
                    if (this.currentStep % 2 != 0)  // we're at an odd step
                    {
                        // add or retract 1, depending on the direction, to get back into the next/previous even stage
                        this.currentStep += direction == MotorDirection.Forward ? 1 : -1;
                    }
                    else {
                        // add or retract 2, depending on the direction, to advance to the next/previous stage
                        this.currentStep += direction == MotorDirection.Forward ? 2 : -2;
                    }
                    break;

                // double coil activation only uses the odd stages (1, 3, 5, 7)
                case StepType.Double:
                    if (this.currentStep % 2 == 0)  // we're at an even step
                    {
                        // add or retract 1, depending on the direction, to get back into the next/previous odd stage
                        this.currentStep += direction == MotorDirection.Forward ? 1 : -1;
                    }
                    else {
                        // add or retract 2, depending on the direction, to advance to the next/previous stage
                        this.currentStep += direction == MotorDirection.Forward ? 2 : -2;
                    }
                    break;

                // interleave coil activation uses all stages (0, 1, 2, 3, 4, 5, 6, 7)
                case StepType.Interleave:
                    // add or retract 1, depending on the direction, to advance to the next/previous stage
                    this.currentStep += direction == MotorDirection.Forward ? 1 : -1;
                    break;
            }

            // ensure we stay within the 0-7 range
            this.currentStep += 8;
            this.currentStep %= 8;

            switch (this.currentStep) {
                case 0:                         // energize coil 1A
                    this.portCoil1B.Write(false);
                    this.portCoil2A.Write(false);
                    this.portCoil2B.Write(false);
                    this.portCoil1A.Write(true);
                    break;
                case 1:                         // energize coil 1A + 2A
                    this.portCoil1A.Write(true);
                    this.portCoil1B.Write(false);
                    this.portCoil2A.Write(true);
                    this.portCoil2B.Write(false);
                    break;
                case 2:                         // energize coil 2A
                    this.portCoil1A.Write(false);
                    this.portCoil1B.Write(false);
                    this.portCoil2B.Write(false);
                    this.portCoil2A.Write(true);
                    break;
                case 3:                         // energize coil 1B + 2A
                    this.portCoil1A.Write(false);
                    this.portCoil1B.Write(true);
                    this.portCoil2A.Write(true);
                    this.portCoil2B.Write(false);
                    break;
                case 4:                         // energize coil 1B
                    this.portCoil1A.Write(false);
                    this.portCoil2A.Write(false);
                    this.portCoil2B.Write(false);
                    this.portCoil1B.Write(true);
                    break;
                case 5:                         // energize coil 1B + 2B
                    this.portCoil1A.Write(false);
                    this.portCoil1B.Write(true);
                    this.portCoil2A.Write(false);
                    this.portCoil2B.Write(true);
                    break;
                case 6:                         // energize coil 2B
                    this.portCoil1A.Write(false);
                    this.portCoil1B.Write(false);
                    this.portCoil2A.Write(false);
                    this.portCoil2B.Write(true);
                    break;
                case 7:                         // energize coil 1A + 2B
                    this.portCoil1A.Write(true);
                    this.portCoil1B.Write(false);
                    this.portCoil2A.Write(false);
                    this.portCoil2B.Write(true);
                    break;
            }

            return this.currentStep;
        }

        #endregion
    }
}


