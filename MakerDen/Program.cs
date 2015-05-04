using Glovebox.Netduino.Actuators;
using Glovebox.Netduino.Sensors;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace MakerDen {
    public class Program : MakerBaseIoT {

        public static void Main() {

            // main code maker

            using (SensorTemp temp = new SensorTemp(Pins.GPIO_PIN_D8, 10000, "temp01"))
            using (SensorLight light = new SensorLight(AnalogChannels.ANALOG_PIN_A0, 1000, "light01"))
            using (rgb = new RgbLed(Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6, "rgb01")) {

                while (true) {
                    Debug.Print(light.ToString());
                    Debug.Print(temp.ToString());

                    rgb.On(RgbLed.Led.Red);
                    Thread.Sleep(500);
                    rgb.Off(RgbLed.Led.Red);
                    Thread.Sleep(500);
                }  // End of while loop
            }

        }
    }
}
