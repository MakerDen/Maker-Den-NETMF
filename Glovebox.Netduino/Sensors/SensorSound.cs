//using System;
//using Glovebox.MicroFramework;
//using Glovebox.MicroFramework.Base;
//using Microsoft.SPOT.Hardware;
//using SecretLabs.NETMF.Hardware;
//using SecretLabs.NETMF.Hardware.Netduino;

//namespace Glovebox.Netduino.Sensors {
//    public class SensorSound : SingleSensor {
//        private SecretLabs.NETMF.Hardware.AnalogInput analogPin;

//        // https://www.inkling.com/read/arduino-cookbook-michael-margolis-2nd/chapter-6/recipe-6-7
//        // https://randomskk.net/projects/lightstrip/code.html

//        const int numberOfSamples = 32;
//        const int averagedOver = 8;
//        const int midpoint = 512;
//        int runningAverage = 0;          //the running average of calculated values
//        int sample;
//        InputPort digitalPin;
//        public override double Current { get { return (int)SampleSound(); } }

//        /// <summary>
//        /// Create and start a sound senor
//        /// </summary>
//        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.AnalogChannels namespace</param>
//        /// <param name="SampleRateMilliseconds">How often to measure in milliseconds or -1 to disable auto timed sensor readings</param>
//        /// <param name="name">Unique identifying name for command and control</param>
//        public SensorSound(Cpu.Pin pin, int SampleRateMilliseconds, string name)
//            : base("sound", "d", ValuesPerSample.One, SampleRateMilliseconds, name) {
//                analogPin = new SecretLabs.NETMF.Hardware.AnalogInput(Pins.GPIO_PIN_A0);
            
//                StartMeasuring();
//        }

//        protected override void Measure(double[] value) {
//            value[0] = SampleSound();
//        }

//        protected override string GeoLocation() {
//            return Utilities.RandomPostcode();
//        }

//        private int SampleSound() {
//            int sumOfSamples = 0;
//            int averageReading; //the average of that loop of readings

//            for (int i = 0; i < numberOfSamples; i++) {
//                var sound = analogPin.Read();
//                sumOfSamples += sample;
//            }

//            averageReading = sumOfSamples / numberOfSamples;     //calculate running average
//            //return averageReading;
//            runningAverage = (((averagedOver - 1) * runningAverage) + averageReading) / averagedOver;

//            return averageReading;
//        }

//        protected override void SensorCleanup() {
//            if (analogPin != null) { analogPin.Dispose(); }
//        }
//    }
//}
