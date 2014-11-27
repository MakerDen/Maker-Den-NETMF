
using Coatsy.Netduino.NeoPixel;
using Coatsy.Netduino.NeoPixel.Grid;
using System.Threading;

namespace MakerDen {
    public class MakerBaseNeoPixelGrid : MakerBaseIoT {
        static NeoPixelGrid grid;
        static Thread neoPixelThread;

        private static void CreateCyclesCollection() {

            grid.cycles = new DoCycle[] {

            new DoCycle(grid.Snow),
            new DoCycle(grid.StarSquare),
            new DoCycle(grid.Rain),
            new DoCycle(grid.Flag),
            };
        }

        protected static void StartNeoPixel() {
            grid = new NeoPixelGrid(8, 8, "neopixelgrid01");

            CreateCyclesCollection();

            neoPixelThread = new Thread(StartNeoPixelThread);
            neoPixelThread.Priority = ThreadPriority.Lowest;
            neoPixelThread.Start();
        }

        // run the neopixel is seperate thread
        private static void StartNeoPixelThread() {
            while (true) {
                for (int i = 0; i < grid.cycles.Length; i++) {
                    grid.ExecuteCycle(grid.cycles[i]);
                }
            }
        }
    }
}
