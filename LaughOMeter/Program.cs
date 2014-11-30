using System;
using Microsoft.SPOT;
using Coatsy.Netduino.NeoPixel;
using System.Threading;
using Glovebox.Netduino.Sensors;
using Microsoft.SPOT.Hardware;

namespace LaughOMeter
{
    public class Program
    {
        public static NeoPixelStrip strip;
        private static AnalogInput analogPin;
        const double minVal = 0d;
        const double maxVal = 0.1d;

        private static PixelColour[] palette = new PixelColour[]
            {
                PixelColour.Red,
                PixelColour.OrangeRed,
                PixelColour.Orange,
                PixelColour.Yellow,
                PixelColour.YellowGreen,
                PixelColour.Green,
                PixelColour.Blue,
                PixelColour.BlueViolet,
                PixelColour.Indigo,
                PixelColour.Violet,
            };

        public static void Main()
        {
            strip = new NeoPixelStrip(50, "LaughOMeter");
            analogPin = new AnalogInput(Cpu.AnalogChannel.ANALOG_3);

            while (true)
            {
                try
                {
                    Debug.Print(analogPin.Read().ToString());
                    strip.SetLevel(((ushort)(analogPin.Read() * 100 / maxVal)), palette);
                }
                catch { }
                Thread.Sleep(500);
            }
        }
    }
}
