using Glovebox.Adafruit.Mini8x8Matrix.Driver;
using Microsoft.SPOT.Hardware;
using System;
using System.Threading;

namespace Glovebox.Adafruit.Mini8x8Matrix {
    public class AdaFruitMatrixRun : Adafruit8x8Matrix, IDisposable {
        #region IDisposable implementation

        void IDisposable.Dispose() {

        }

        #endregion

        Thread matrix;

        public AdaFruitMatrixRun()
            : base(new Ht16K33I2cConnection(new I2CDevice(new I2CDevice.Configuration(0x70, 200)))) {

            matrix = new Thread(new ThreadStart(this.RunSequence));
            matrix.Start();
        }

        private void RunSequence() {

            FrameSetBrightness(4);
            FrameSetBlinkRate(BlinkRate.Off);

            while (true) {

                ScrollStringInFromRight("Happy Birthday", 100);
                ScrollSymbolInFromRight(new Symbols[] { Symbols.Heart, Symbols.Heart }, 100);

                for (int i = 0; i < fontSimple.Length; i++) {
                    DrawBitmap(fontSimple[i]);
                    FrameDraw();
                    Thread.Sleep(100);
                }


                DrawSymbol(Symbols.Heart);
                FrameDraw();
                Thread.Sleep(50);

                for (int i = 0; i < 4; i++) {
                    for (ushort c = 0; c < Columns; c++) {
                        FrameRollRight();
                        FrameDraw();
                        Thread.Sleep(50);
                    }
                }

                for (int c = 0; c < 4; c++) {
                    for (int i = 0; i < Rows; i++) {
                        RowRollUp();
                        FrameDraw();
                        Thread.Sleep(50);
                    }
                }

                for (int i = 0; i < 4; i++) {

                    for (ushort c = 0; c < Columns; c++) {
                        FrameRollLeft();
                        FrameDraw();
                        Thread.Sleep(50);
                    }
                }

                for (int c = 0; c < 4; c++) {
                    for (int i = 0; i < Rows; i++) {
                        RowRollDown();
                        FrameDraw();
                        Thread.Sleep(50);
                    }
                }

                for (int j = 0; j < 4; j++) {
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
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                }

                for (int j = 0; j < 5; j++) {
                    for (int i = 0; i < 64; i++) {
                        FrameSet(i, true);
                        FrameSet((63 - i), true);
                        FrameDraw();
                        Thread.Sleep(15);
                        FrameSet(i, false);
                        FrameSet((63 - i), false);
                        FrameDraw();
                        Thread.Sleep(15);
                    }
                }
            }
        }
    }
}

