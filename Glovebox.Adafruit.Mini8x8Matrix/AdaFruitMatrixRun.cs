using Glovebox.Adafruit.Mini8x8Matrix;
using Glovebox.IoT;
using Microsoft.SPOT.Hardware;
using System;
using System.Threading;

namespace Glovebox.Adafruit.Mini8x8Matrix {
    public class AdaFruitMatrixRun : Adafruit8x8Matrix {

        Thread matrixThread;

        public AdaFruitMatrixRun(string name)
            : base(new Ht16K33I2cConnection(new I2CDevice(new I2CDevice.Configuration(0x70, 200))), name) {

            FrameSetBrightness(2);
            FrameSetBlinkRate(BlinkRate.Off);

            CreateCyclesCollection();

            matrixThread = new Thread(StartMiniMatrixThread);
            matrixThread.Priority = ThreadPriority.Lowest;
            matrixThread.Start();

        }

        private void StartMiniMatrixThread() {
            while (true) {
                for (int i = 0; i < cycles.Length; i++) {
                    ExecuteCycle(cycles[i]);
                }
            }
        }

        private void CreateCyclesCollection() {
            cycles = new DoCycle[] {

            new DoCycle(IPAddress),
            new DoCycle(Welcome),
            new DoCycle(AlphaNumeric),
            new DoCycle(Hearts),
            new DoCycle(FollowMe),
            };
        }

        public void IPAddress() {
            ScrollStringInFromRight(Util.GetIPAddress(), 100);
        }

        public void Welcome() {
            ScrollStringInFromRight("Welcome", 100);
        }

        public void AlphaNumeric() {
            for (int i = 0; i < fontSimple.Length; i++) {
                DrawBitmap(fontSimple[i]);
                FrameDraw();
                Util.Delay(100);
            }
        }

        public void Hearts() {
            DrawSymbol(Symbols.Heart);
            FrameDraw();
            Util.Delay(50);

            for (int i = 0; i < 2; i++) {
                for (ushort c = 0; c < Columns; c++) {
                    FrameRollRight();
                    FrameDraw();
                    Util.Delay(50);
                }
            }

            for (int c = 0; c < 2; c++) {
                for (int i = 0; i < Rows; i++) {
                    RowRollUp();
                    FrameDraw();
                    Util.Delay(50);
                }
            }

            for (int i = 0; i < 2; i++) {

                for (ushort c = 0; c < Columns; c++) {
                    FrameRollLeft();
                    FrameDraw();
                    Util.Delay(50);
                }
            }

            for (int c = 0; c < 2; c++) {
                for (int i = 0; i < Rows; i++) {
                    RowRollDown();
                    FrameDraw();
                    Util.Delay(50);
                }
            }

            for (int j = 0; j < 2; j++) {
                for (int i = 0; i < Rows; i++) {
                    ColumnRollLeft(0);
                    ColumnRollRight(1);
                    ColumnRollLeft(2);
                    ColumnRollRight(3);
                    ColumnRollLeft(4);
                    ColumnRollRight(5);
                    ColumnRollLeft(6);
                    ColumnRollRight(7);
                    FrameDraw();
                    Util.Delay(100);
                }
                Util.Delay(500);
            }
        }

        public void FollowMe() {
            for (int j = 0; j < 2; j++) {
                for (int i = 0; i < 64; i++) {
                    FrameSet(i, true);
                    FrameSet((63 - i), true);
                    FrameDraw();
                    Util.Delay(15);
                    FrameSet(i, false);
                    FrameSet((63 - i), false);
                    FrameDraw();
                    Util.Delay(15);
                }
            }
        }
    }
}

