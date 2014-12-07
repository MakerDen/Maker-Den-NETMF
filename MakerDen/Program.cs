using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Sensors;
using Glovebox.Netduino;
using Glovebox.Netduino.Sensors;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace MakerDen
{
    public class Program : MakerBaseNeoPixelGrid
    {
        public static void Main()
        {
            // main code marker

            PowerManagment.SetPeripheralState(Peripheral.Ethernet, Switch.Off);
            PowerManagment.SetPeripheralState(Peripheral.SDCard, Switch.Off);
            PowerManagment.SetPeripheralState(Peripheral.PowerLED, Switch.Off);

            StartNeoPixel();

            StartNetworkServices("dng", false);

            using (SensorTemp temp = new SensorTemp(Pins.GPIO_PIN_D8, 10000, "temp01"))
            using (SensorLight light = new SensorLight(AnalogChannels.ANALOG_PIN_A0, 1000, "light01"))
            using (SensorMemory mem = new SensorMemory(5000, "mem01"))
            using (SensorError error = new SensorError(5000, "error01"))
            using (rgb = new RgbLed(Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6, "rgb01")) {

                temp.OnBeforeMeasurement += OnBeforeMeasure;
                temp.OnAfterMeasurement += OnMeasureCompleted;

                light.OnBeforeMeasurement += OnBeforeMeasure;
                light.OnAfterMeasurement += OnMeasureCompleted;

                mem.OnBeforeMeasurement += OnBeforeMeasure;
                mem.OnAfterMeasurement += OnMeasureCompleted;

                error.OnAfterMeasurement += OnMeasureCompleted;

                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}

