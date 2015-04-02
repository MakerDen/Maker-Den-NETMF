using System;
using Microsoft.SPOT;
using System.Threading;

namespace Coatsy.Netduino.NeoPixel.Jewel
{
    public class NeoPixelJewel : NeoPixelFrameBase
    {
        private ushort centrePixelPos = 0;
        public NeoPixelJewel(int ledCount, string name)
            : base(ledCount, name)
        {

        }

        #region Primitives
        public void Flower(Pixel petals, Pixel centre)
        {
            FrameSet(petals);
            FrameSet(centre, centrePixelPos);
            FrameDraw();
        }

        public void Flower(Pixel[] petals, Pixel centre)
        {
            FrameSet(petals);
            FrameSet(centre, centrePixelPos);
            FrameDraw();
        }

        public void SetCentre(Pixel colour)
        {
            FrameSet(colour, centrePixelPos);
            FrameDraw();
        }

        public void SetPetal(Pixel colour, ushort petalPos)
        {
            FrameSet(colour, (ushort)(petalPos + 1));
        }

        #endregion
        #region Cycles
        public void Marigold()
        {
            Flower(Pixel.ColourLowPower.HotYellow, Pixel.ColourLowPower.WarmYellow);
            Thread.Sleep(1000);
        }

        public void Rainbow()
        {
            Pixel[] pallette = new Pixel[]
            {
                Pixel.ColourLowPower.HotRed,
                Pixel.ColourLowPower.HotOrange,
                Pixel.ColourLowPower.HotYellow,
                Pixel.ColourLowPower.HotGreen,
                Pixel.ColourLowPower.HotBlue,
                Pixel.ColourLowPower.HotPurple,
            };

            Flower(pallette, Pixel.ColourLowPower.WarmYellow);

            Thread.Sleep(1000);
        }

        public void LightItUp()
        {
            Flower(Pixel.ColourLowPower.HotGreen, Pixel.ColourLowPower.WarmGreen);
            Blink(1000, 5);
        }
        #endregion

        public void ExecuteCycle(DoCycle doCycle)
        {
            try
            {
                doCycle();
                Thread.Sleep(50);
            }
            catch { ActuatorErrorCount++; }
        }
    }
}
