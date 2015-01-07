using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework.Base;
using Glovebox.MicroFramework.IoT;


namespace Glovebox.Netduino.Actuators {

    //timings set for the Jaycar Hobby Tech YM-2763, Pulse Width: 800 - 2200µs, Rotation Angle: 120
    //YM-2763 servo cable pins - brown cable: ground, red cable: 5v, yellow/orange cable: control

    public class ServoPwm : ActuatorBase {

        public enum Actions {
            Min, Max
        }


        uint _minPosition = 800;  //microseconds
        uint _maxPosition = 2200;  //microseconds
        uint _maxDegrees = 120;
        uint _range = 0;
        float degreesRatio = 0f;

        Cpu.PWMChannel _pin;
        uint _period = 20000;

        uint _servoPosition = 0;
        object _devicelock = new object();

        PWM _servoMotor;

        /// <summary>
        /// Current position in points of the servo
        /// </summary>
        public uint CurrentPosition { get { return _servoPosition; } }

        /// <summary>
        /// Maxiumum points the servo can move
        /// </summary>
        public uint Range { get { return _range; } }

        /// <summary>
        /// Maxiumum degress the servo move
        /// </summary>
        public uint Degrees { get { return _maxDegrees; } }

        /// <summary>
        /// lock for exclusive control of the servo lock(.DeviceLock) {...}
        /// </summary>
        public object DeviceLock { get { return _devicelock; } set { _devicelock = value; } }


        /// <summary>
        /// Initialise the Servo motor with default Jaycar Hobby Tech YM-2763, //timings set for the Jaycar Hobby Tech YM-2763, Pulse Width: 800 - 2200µs, Rotation Angle: 120
        /// </summary>
        /// <param name="pin">PWM Pin</param>
        public ServoPwm(Cpu.PWMChannel pin, string name)
            : base(name, ActuatorType.ServoPwm) {
            _pin = pin;
            Initialise();
        }

        /// <summary>
        /// Initialise the Servo motor
        /// </summary>
        /// <param name="pin">PWM Pin</param>
        /// <param name="minPulse">minimum pulse duration for servo in microseconds</param>
        /// <param name="maxPulseDuration">maximum pulse duration for servo in microseconds</param>
        /// <param name="maxDegrees">maximum degrees the servo can sweep</param>
        public ServoPwm(Cpu.PWMChannel pin, uint period, uint minPulseDuration, uint maxPulseDuration, uint maxDegrees, string name)
            : base(name, ActuatorType.ServoPwm) {
            _pin = pin;
            _period = period;
            _minPosition = minPulseDuration;
            _maxPosition = maxPulseDuration;
            _maxDegrees = maxDegrees;

            Initialise();

        }

        private void Initialise() {

            _range = _maxPosition - _minPosition;
            degreesRatio = (float)_range / (float)_maxDegrees;

            _servoMotor = new PWM(_pin, _period, _minPosition, PWM.ScaleFactor.Microseconds, false);

            _servoMotor.DutyCycle = 0;
            _servoMotor.Start();

            //give the servo enough time to swing to _minPosition
            Thread.Sleep(250);
        }


        /// <summary>
        /// Position the servo by points
        /// </summary>
        public void Position(uint pos) {
            if (pos > (_range)) { pos = _range; }

            _servoPosition = pos;
            pos += _minPosition;

            _servoMotor.Duration = pos;
        }

        /// <summary>
        /// Position the servo by degrees
        /// </summary>
        public void PositionByDegrees(uint degrees) {
            uint pos = (uint)(degreesRatio * degrees);

            _servoPosition = pos;
            pos += _minPosition;

            _servoMotor.Duration = pos;
        }

        public void Reset() {
            _servoMotor.Duration = _minPosition;
            Thread.Sleep(500);
        }

        protected override void ActuatorCleanup() {
            if (_servoMotor == null) { return; }
            _servoMotor.DutyCycle = 0;
            _servoMotor.Stop();
            _servoMotor.Dispose();
        }

        /// <summary>
        /// Set Servo position, min, max, by points between min and max or by degrees
        /// </summary>
        /// <param name="action"></param>
        public override void Action(MicroFramework.IoT.IotAction action) {
            switch (action.cmd) {
                case "min":
                    Action(Actions.Min);
                    break;
                case "max":
                    Action(Actions.Max);
                    break;
                case "position":
                    ActionSetPosition(action.parameters);
                    break;
                case "degrees":
                    ActionSetDegrees(action.parameters);
                    break;
            }
        }

        private void ActionSetDegrees(string parameters) {
            double pos = 0;
            if (double.TryParse(parameters, out pos)) {
                PositionByDegrees((uint)pos);
            }
        }

        private void ActionSetPosition(string parameters) {
            double pos = 0;
            if (double.TryParse(parameters, out pos)) {
                Position((uint)pos);
            }
        }

        public void Action(Actions action) {
            switch (action) {
                case Actions.Min:
                    Position(0);
                    break;
                case Actions.Max:
                    Position(Range);
                    break;
            }
        }
    }
}
