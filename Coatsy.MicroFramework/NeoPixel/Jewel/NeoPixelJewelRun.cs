using System;
using Microsoft.SPOT;
using System.Threading;
using Glovebox.MicroFramework.IoT;

namespace Coatsy.Netduino.NeoPixel.Jewel {
    public class NeoPixelJewelRun : NeoPixelJewel {
        Thread neoPixelThread;

        public NeoPixelJewelRun(string name)
            : base(7, name) {

            CreateCyclesCollection();

            neoPixelThread = new Thread(StartNeoPixelThread);
            neoPixelThread.Priority = ThreadPriority.Lowest;
            neoPixelThread.Start();
        }

        private void CreateCyclesCollection() {
            cycles = new DoCycle[] {

            new DoCycle(Marigold),
            new DoCycle(Rainbow),
            };
        }


        #region Cycles
        public void Marigold() {
            for (int i = 0; i < 4; i++) {
                FlowerSet(Pixel.ColourLowPower.WarmGreen, Pixel.ColourLowPower.WarmRed);
                FrameDraw();
                Thread.Sleep(500);
                FlowerSet(Pixel.ColourLowPower.WarmRed, Pixel.ColourLowPower.WarmGreen);
                FrameDraw();
                Thread.Sleep(500);
            }
        }

        public void Rainbow() {
            FlowerSet(PaletteWarmLowPower, Pixel.ColourLowPower.WarmPurple);
            FrameDraw();
            Blink(500, 2);
        }

        public void XboxLightItUp() {
            while (true) {
                FlowerSet(Pixel.ColourLowPower.HotGreen, Pixel.ColourLowPower.WarmGreen);
                FrameDraw();
                Blink(1000, 5);
            }
        }
        #endregion


        private void StartNeoPixelThread() {
            while (true) {
                for (int i = 0; i < cycles.Length; i++) {
                    ExecuteCycle(cycles[i]);
                }
            }
        }

        public void ExecuteCycle(DoCycle doCycle) {
            try {
                doCycle();

                var a = GetNextAction();
                while (a != null) {
                    DoAction(a);
                    a = GetNextAction();
                }
            }
            catch { ActuatorErrorCount++; }
        }

        private void DoAction(IotAction a) {
            switch (a.cmd) {
                case "start":
                    if (a.parameters == "xbox") {
                        XboxLightItUp();
                    }
                    break;
            }
        }
    }
}
