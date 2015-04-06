using Coatsy.Netduino.NeoPixel;
using Glovebox.Adafruit.Mini8x8Matrix;
using System.Threading;

namespace MakerDen {
    public class MakerBaseMiniMatrix : MakerBaseIoT {
        static AdaFruitMatrixRun miniMatrix;
        static Thread miniMatrixlThread;
        static DoCycle[] cycles;

        private static void CreateCyclesCollection() {
            cycles = new DoCycle[] {
            new DoCycle(miniMatrix.HappyBirthday),
            new DoCycle(miniMatrix.AlphaNumeric),
            new DoCycle(miniMatrix.Hearts),
            new DoCycle(miniMatrix.FollowMe),
            };
        }

        private void Birthday() {

        }

        protected static void StartMiniMatrix() {
            miniMatrix = new AdaFruitMatrixRun();

            CreateCyclesCollection();

            miniMatrixlThread = new Thread(StartMiniMatrixThread);
            miniMatrixlThread.Priority = ThreadPriority.Lowest;
            miniMatrixlThread.Start();
        }

        public static void ExecuteCycle(DoCycle doCycle) {
            try {
                doCycle();
            }
            catch { }
        }

        private static void StartMiniMatrixThread() {
            while (true) {
                for (int i = 0; i < cycles.Length; i++) {
                    ExecuteCycle(cycles[i]);
                }
            }
        }
    }
}
