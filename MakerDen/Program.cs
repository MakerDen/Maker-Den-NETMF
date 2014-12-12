using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Sensors;
using Glovebox.Netduino;
using Glovebox.Netduino.Sensors;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace MakerDen
{
    public class Program : MakerBaseIoT
    {
        public static void Main()
        {
            // main code marker

            //Replace the "emul" which is the name of the device with a unique 3 to 5 character name
            //use your initials or something similar.  This code will be visible on the IoT Dashboard
            StartNetworkServices("emul", true);

            using (SensorTemp temp = new SensorTemp(Pins.GPIO_PIN_D8, 10000, "temp01"))
            using (SensorLight light = new SensorLight(AnalogChannels.ANALOG_PIN_A0, 1000, "light01"))
            using (rgb = new RgbLed(Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6, "rgb01")) {
                temp.OnBeforeMeasurement += OnBeforeMeasure;
                temp.OnAfterMeasurement += OnMeasureCompleted;
                light.OnBeforeMeasurement += OnBeforeMeasure;
                light.OnAfterMeasurement += OnMeasureCompleted;
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}

