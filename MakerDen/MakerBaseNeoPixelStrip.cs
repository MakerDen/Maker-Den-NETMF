using System;
using Microsoft.SPOT;
using Coatsy.Netduino.NeoPixel;
using System.Threading;

namespace MakerDen
{
    public class MakerBaseNeoPixelStrip : MakerBaseIoT
    {
        static NeoPixelStrip strip;
        static Thread neoPixelThread;

        private static void CreateCyclesCollection()
        {

            strip.cycles = new DoCycle[] {

            new DoCycle(strip.Themometer),
            new DoCycle(strip.SlowFill),
            //new DoCycle(strip.Pulse),
            new DoCycle(strip.Blinky),
            new DoCycle(strip.Rainbow),
            new DoCycle(strip.ColourChange),
            };
        }

        protected static void StartNeoPixel()
        {
            strip = new NeoPixelStrip(150, "neopixelstrip01");

            CreateCyclesCollection();

            neoPixelThread = new Thread(StartNeoPixelThread);
            neoPixelThread.Priority = ThreadPriority.Lowest;
            neoPixelThread.Start();
        }

        private static void StartNeoPixelThread()
        {
            while (true)
            {
                for (int i = 0; i < strip.cycles.Length; i++)
                {
                    strip.ExecuteCycle(strip.cycles[i]);
                }
            }
        }
    }
}
