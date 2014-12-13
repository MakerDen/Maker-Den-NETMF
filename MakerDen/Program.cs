using Coatsy.Netduino.NeoPixel;
using Coatsy.Netduino.NeoPixel.Grid;
using Glovebox.MicroFramework;
using Glovebox.MicroFramework.Sensors;
using Glovebox.Netduino;
using Glovebox.Netduino.Sensors;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace MakerDen {
    public class Program  {
        public static void Main() {
            // main code marker
            using (SensorLight light = new SensorLight(AnalogChannels.ANALOG_PIN_A0, -1, "light01"))
            using (NeoPixelGrid8x8 g = new NeoPixelGrid8x8("display01")) {
                while (true) {
                    for (byte i = 0; i < 26; i++) {
                        g.DrawLetter(i, g.coolPalette[i % g.coolPalette.Length]);
                        g.FrameDraw();
                        Thread.Sleep((int)light.Current * 4);
                    }
                    for (byte i = 0; i < 26; i++) {
                        g.DrawLowercae(i, g.coolPalette[i % g.coolPalette.Length]);
                        g.FrameDraw();
                        Thread.Sleep((int)light.Current * 4);
                    }
                    for (byte i = 0; i < 10; i++) {
                        g.DrawNumber(i, g.coolPalette[i % g.coolPalette.Length]);
                        g.FrameDraw();
                        Thread.Sleep((int)light.Current * 4);
                    }
                    for (byte i = 0; i < 10; i++) {
                        g.DrawSymbols(i, g.coolPalette[i % g.coolPalette.Length]);
                        g.FrameDraw();
                        Thread.Sleep((int)light.Current * 4);
                    }

                    //g.DrawLetter(NeoPixelGrid8x8.Letters.f, Pixel.CoolColours.CoolRed);
                    //g.FrameDraw();
                    //Thread.Sleep(250);
                    //g.DrawLetter(NeoPixelGrid8x8.Letters.r, Pixel.CoolColours.CoolRed);
                    //g.FrameDraw();
                    //Thread.Sleep(250);
                    //g.DrawLetter(NeoPixelGrid8x8.Letters.e, Pixel.CoolColours.CoolRed);
                    //g.FrameDraw();
                    //Thread.Sleep(250);
                    //g.DrawLetter(NeoPixelGrid8x8.Letters.d, Pixel.CoolColours.CoolRed);
                    //g.FrameDraw();
                    //Thread.Sleep(250);
                    //g.DrawLetter(NeoPixelGrid8x8.Letters.d, Pixel.CoolColours.CoolRed);
                    //g.FrameDraw();
                    //Thread.Sleep(250);
                    //g.DrawLetter(NeoPixelGrid8x8.Letters.i, Pixel.CoolColours.CoolRed);
                    //g.FrameDraw();
                    //Thread.Sleep(250);
                    //g.DrawLetter(NeoPixelGrid8x8.Letters.e, Pixel.CoolColours.CoolRed);
                    //g.FrameDraw();
                    //Thread.Sleep(1000);
                    //g.DrawLetter(NeoPixelGrid8x8.Letters.d, Pixel.CoolColours.CoolRed);
                    //g.FrameDraw();
                    //Thread.Sleep(1000);
                    //g.DrawLetter(NeoPixelGrid8x8.Letters.a, Pixel.CoolColours.CoolRed);
                    //g.FrameDraw();
                    //Thread.Sleep(1000);
                    //g.DrawLetter(NeoPixelGrid8x8.Letters.v, Pixel.CoolColours.CoolRed);
                    //g.FrameDraw();
                    //Thread.Sleep(1000);
                    //g.DrawLetter(NeoPixelGrid8x8.Letters.e, Pixel.CoolColours.CoolRed);
                    //g.FrameDraw();
                    //Thread.Sleep(1000);

                }

                //for (int c = 0; c < 32; c++) {
                //    for (ushort i = 0; i < 8; i++) {
                //        g.RowRollRight(i);
                //    }
                //    g.FrameDraw();
                //    Thread.Sleep(100);
                //}
            }


        }
    }
}

