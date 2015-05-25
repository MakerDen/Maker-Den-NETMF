using System;
using Microsoft.SPOT;
using System.Threading;

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


     
    }
}
