using Glovebox.Netduino.Actuators;
using Glovebox.Netduino.Sensors;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;
}




Imperative Modelusing Glovebox.MicroFramework.Sensors;
using Glovebox.Netduino.Actuators;
using Glovebox.Netduino.Sensors;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace MakerDen {
    public class Program : MakerBaseIoT  {
        public static void Main() {
            // main code marker

            StartNetworkServices("cos", true);

            // sensor timer value -1 disables auto measure
            using (SensorLight light = new SensorLight(AnalogChannels.ANALOG_PIN_A0, 1000, "light01"))
            using (rgb = new RgbLed(Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6, "rgb01"))
            using (LedDigital led = new LedDigital(Pins.GPIO_PIN_D0, "ledRed"))
            
            {

                while (true) {
                    if (light.Current < 60) {
                        led.On();
                    }
                    else {
                        rgb.Off(RgbLed.Led.Red);
                        rgb.On(RgbLed.Led.Green);
                    }
                    Thread.Sleep(100);
                }
            }
        }
    }
}
