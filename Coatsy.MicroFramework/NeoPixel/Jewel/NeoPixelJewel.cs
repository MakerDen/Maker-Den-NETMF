using System;
using Microsoft.SPOT;
using System.Threading;
using Glovebox.MicroFramework.IoT;

namespace Coatsy.Netduino.NeoPixel.Jewel {
    public class NeoPixelJewel : NeoPixelFrameBase {
        private ushort centrePixelPos = 0;
        public NeoPixelJewel(int ledCount, string name)
            : base(ledCount, name) {
        }

        #region Primitives
        public void FlowerSet(Pixel petals, Pixel centre) {
            FrameSet(petals);
            FrameSet(centre, centrePixelPos);
        }

        public void FlowerSet(Pixel[] petals, Pixel centre) {
            FrameSet(petals);
            FrameSet(centre, centrePixelPos);
        }

        public void FlowerCentreSet(Pixel colour) {
            FrameSet(colour, centrePixelPos);
        }

        public void FlowerPetalSet(Pixel colour, ushort petalPos) {
            FrameSet(colour, (ushort)(petalPos + 1));
        }

        #endregion
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
                    for (int i = 0; i < cycles.Length; i++) {
                        if (a.parameters == "xbox") {
                            XboxLightItUp();
                        }
                    }
                    break;
            }
        }
    }
}
