using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Glovebox.IoT;
using Glovebox.IoT.Base;

namespace Glovebox.Netduino.Sensors {

    //Breakout Board for Electret Microphone BOB-09964
    //http://littlebirdelectronics.com.au/products/breakout-board-for-electret-microphone
    public class SensorSound : SensorBase {
        private AnalogInput analogPin;

        // https://www.inkling.com/read/arduino-cookbook-michael-margolis-2nd/chapter-6/recipe-6-7
        // https://randomskk.net/projects/lightstrip/code.html

        const int numberOfSamples = 32;
        const int averagedOver = 8;
        const int midpoint = 512;
        int runningAverage = 0;          //the running average of calculated values
        int sample;

        public override double Current { get { return (int)SampleSound(); } }

        /// <summary>
        /// Create and start a sound senor
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.AnalogChannels namespace</param>
        /// <param name="SampleRateMilliseconds">How often to measure in milliseconds or -1 to disable auto timed sensor readings</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public SensorSound(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base("sound", "d", ValuesPerSample.One, SampleRateMilliseconds, name) {

            analogPin = new AnalogInput(pin, -1);
            StartMeasuring();
        }

        protected override void Measure(double[] value) {
            value[0] = SampleSound();
        }

        protected override string GeoLocation() {
            return Utilities.RandomPostcode();
        }

        private int SampleSound() {
            int sumOfSamples = 0;
            int averageReading; //the average of that loop of readings

            for (int i = 0; i < numberOfSamples; i++) {
                sample = (int)(analogPin.Read() * 1024) - midpoint;

                sample = System.Math.Abs(sample);

                sumOfSamples += sample;
            }

            averageReading = sumOfSamples / numberOfSamples;     //calculate running average
            runningAverage = (((averagedOver - 1) * runningAverage) + averageReading) / averagedOver;

            return runningAverage;
        }

        protected override void SensorCleanup() {
            if (analogPin != null) { analogPin.Dispose(); }
        }
    }
}
