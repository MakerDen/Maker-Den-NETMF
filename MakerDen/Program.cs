using Coatsy.Netduino.NeoPixel;
using Coatsy.Netduino.NeoPixel.Grid;
using Glovebox.MicroFramework.Sensors;
using Glovebox.Netduino.Actuators;
using Glovebox.Netduino.Sensors;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace MakerDen {
    public class Program : MakerBaseIoT  {

        private static NeoPixelGrid8x8 grid = new NeoPixelGrid8x8("grid01", 2);

        public static void Main() {
            // main code marker

            grid.ScrollStringInFromRight("hello world", Pixel.CoolColours.CoolGreen, 100);


            StartNetworkServices("dng", true);

            using (SensorTemp temp = new SensorTemp(Pins.GPIO_PIN_D8, 10000, "temp01"))
            using (SensorLight light = new SensorLight(AnalogChannels.ANALOG_PIN_A0, 1000, "light01"))
            using (SensorError error = new SensorError(2000, "error01"))
            using (SensorMemory mem = new SensorMemory(1000, "mem01"))
            using (SensorSound sound = new SensorSound(AnalogChannels.ANALOG_PIN_A4, 1000, "sound01"))
            using (rgb = new RgbLed(Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6, "rgb01")) {
                temp.OnBeforeMeasurement += OnBeforeMeasure;
                temp.OnAfterMeasurement += OnMeasureCompleted;
                light.OnBeforeMeasurement += OnBeforeMeasure;
                light.OnAfterMeasurement += OnMeasureCompleted;
                sound.OnBeforeMeasurement += OnBeforeMeasure;
                sound.OnAfterMeasurement += OnMeasureCompleted;
                error.OnAfterMeasurement += OnMeasureCompleted;
                mem.OnAfterMeasurement += OnMeasureCompleted;
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}

