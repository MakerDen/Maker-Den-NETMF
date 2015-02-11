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
        
        public static void Main() {
            // main code marker

            string text = "Hello World!!";
            int currentChar = text.Length - 1;
            int colour = 0;

            NeoPixelGrid8x8 grid = new NeoPixelGrid8x8("grid01", 3);

            while (true) {
                grid.ScrollSymbolInFromLeft(NeoPixelGrid8x8.Symbols.Heart, grid.hotPalette[colour++ % grid.hotPalette.Length], 10);
                grid.ScrollCharacterFromLeft(text.Substring(currentChar)[0], grid.hotPalette[colour++ % grid.hotPalette.Length], 10);
                currentChar--;
                if (currentChar < 0) { currentChar = text.Length - 1; }
            }

            grid.ScrollStringInFromLeft("hello world", new Pixel[] { Pixel.CoolColours.HotRed, Pixel.CoolColours.HotGreen, Pixel.CoolColours.HotBlue }, 10);

            grid.ScrollBitmapInFromLeft((ulong)NeoPixelGrid8x8.Symbols.Heart, Pixel.CoolColours.HotRed, 10);


            grid.DrawSymbol(NeoPixelGrid8x8.Symbols.Heart, grid.warmPalette[0], 1);
            grid.FrameDraw();

            for (int i = 0; i < 300; i++) {
                grid.RowFrameRight();
                grid.FrameDraw();
            }
        }
    }
}

