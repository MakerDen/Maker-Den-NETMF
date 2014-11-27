using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;

namespace Glovebox.Netduino.Sensors {
    public class SensorSound : SensorBase {
        protected AnalogInput analogPin;

        // https://www.inkling.com/read/arduino-cookbook-michael-margolis-2nd/chapter-6/recipe-6-7
        // https://randomskk.net/projects/lightstrip/code.html

        const int numberOfSamples = 32;
        const int averagedOver = 8;
        const int midpoint = 512;
        int runningAverage = 0;          //the running average of calculated values
        int sample;

        protected override double Current { get { return (int)SampleSound(); } }

        public SensorSound(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base(SensorType.Sound, ValuesPerSample.One, SampleRateMilliseconds, name) {

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

                // get absolute value bit shifting a 32 bit int
                int mask = sample >> 31;
                sample = (mask + sample) ^ mask;

                sumOfSamples += sample;
            }

            averageReading = sumOfSamples / numberOfSamples;     //calculate running average
            runningAverage = (((averagedOver - 1) * runningAverage) + averageReading) / averagedOver;

            return runningAverage;
        }

        //void IDisposable.Dispose() {
        //    if (analogPin != null) { analogPin.Dispose(); }
        //}

        protected override void SensorCleanup() {
            if (analogPin != null) { analogPin.Dispose(); }
        }
    }
}
