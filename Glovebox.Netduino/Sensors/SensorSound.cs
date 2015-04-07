using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Base;

namespace Glovebox.Netduino.Sensors {
    public class SensorSound : SensorBase {
        private AnalogInput analogPin;

        // https://www.inkling.com/read/arduino-cookbook-michael-margolis-2nd/chapter-6/recipe-6-7
        // https://randomskk.net/projects/lightstrip/code.html

        const int numberOfSamples = 32;
        const int averagedOver = 8;
        const int midpoint = 512;
        int runningAverage = 0;          //the running average of calculated values
        int sample;
        InputPort digitalPin;
        public override double Current { get { return (int)SampleSound(); } }

        /// <summary>
        /// Create and start a sound senor
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.AnalogChannels namespace</param>
        /// <param name="SampleRateMilliseconds">How often to measure in milliseconds or -1 to disable auto timed sensor readings</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public SensorSound(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base("sound", "d", ValuesPerSample.One, SampleRateMilliseconds, name) {
                digitalPin = new InputPort(Cpu.Pin.GPIO_Pin0,false,Port.ResistorMode.Disabled);
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
                if(digitalPin !=null){
                   // Debug.Print(digitalPin.ToString());
                  //  Debug.Print(digitalPin.Read().ToString());
                }
                var sound = analogPin.Read();
//Debug.Print(sound.ToString());
             //   sample = (int)(20.0 * System.Math.Log10(sound *1024));
                sample = (int)(sound * 1024) -midpoint;

                // get absolute value bit shifting a 32 bit int
                int mask = sample >> 31;
                sample = (mask + sample) ^ mask;

                sumOfSamples += sample;
            }

            averageReading = sumOfSamples / numberOfSamples;     //calculate running average
            //return averageReading;
            runningAverage = (((averagedOver - 1) * runningAverage) + averageReading) / averagedOver;

            return runningAverage;
        }

        protected override void SensorCleanup() {
            if (analogPin != null) { analogPin.Dispose(); }
        }
    }
}
