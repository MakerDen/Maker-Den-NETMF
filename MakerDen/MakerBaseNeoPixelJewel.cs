using System;
using Microsoft.SPOT;
using Coatsy.Netduino.NeoPixel.Jewel;
using System.Threading;
using Coatsy.Netduino.NeoPixel;

namespace MakerDen {
    public class MakerBaseNeoPixelJewel : MakerBaseIoT {
        static NeoPixelJewel jewel;
        static Thread neoPixelThread;

        private static void CreateCyclesCollection() {
            jewel.cycles = new DoCycle[] {

            new DoCycle(jewel.Marigold),
            new DoCycle(jewel.Rainbow),
            };
        }

        protected static void StartNeoPixel() {
            jewel = new NeoPixelJewel(7, "jewel");

            CreateCyclesCollection();

            neoPixelThread = new Thread(StartNeoPixelThread);
            neoPixelThread.Priority = ThreadPriority.Lowest;
            neoPixelThread.Start();
        }

        private static void StartNeoPixelThread() {
            while (true) {
                for (int i = 0; i < jewel.cycles.Length; i++) {
                    jewel.ExecuteCycle(jewel.cycles[i]);
                }
            }
        }
    }
}
