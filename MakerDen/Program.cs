using Glovebox.Netduino.Actuators;
using Glovebox.Netduino.Sensors;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;
using Glovebox.Adafruit.Mini8x8Matrix;
using Coatsy.Netduino.NeoPixel.Jewel;
using Glovebox.IoT;

namespace MakerDen {
    public class Program : MakerBaseIoT {

        public static void Main() {

            // main code maker
            using (AdaFruitMatrixRun matrix = new AdaFruitMatrixRun("matr"))
            using (NeoPixelJewelRun jewel = new NeoPixelJewelRun("jewel"))
            using (SensorTemp temp = new SensorTemp(Pins.GPIO_PIN_D8, 10000, "temp01"))
            using (SensorLight light = new SensorLight(AnalogChannels.ANALOG_PIN_A0, 1000, "light01"))
            using (rgb = new RgbLed(Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6, "rgb01")) {
                temp.OnBeforeMeasurement += OnBeforeMeasure;
                temp.OnAfterMeasurement += OnMeasureCompleted;
                light.OnBeforeMeasurement += OnBeforeMeasure;
                light.OnAfterMeasurement += OnMeasureCompleted;
                Util.Delay(Timeout.Infinite);
            }
        }
    }
}
