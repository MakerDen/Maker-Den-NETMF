using System;
using Microsoft.SPOT;
using System.Threading;

namespace Coatsy.Netduino.NeoPixel
{
    public class NeoPixelStrip : NeoPixelFrameBase
    {

        public NeoPixelStrip(int _pixelCount, string name)
            : base(_pixelCount, name)
        {

        }


        #region Cycles

        public void Themometer()
        {
            PixelColour[] palette = new PixelColour[]
            {
                PixelColour.Red,
                PixelColour.OrangeRed,
                PixelColour.Orange,
                PixelColour.Yellow,
                PixelColour.YellowGreen,
                PixelColour.Green,
                PixelColour.Blue,
                PixelColour.BlueViolet,
                PixelColour.Indigo,
                PixelColour.Violet,
            };

            SetLevel(100, palette, true, 7, 10);
        }

        public void SlowFill()
        {
            SetLevel(80, Pixel.ColourLowPower.HotGreen, true, 3, 20);
        }

        public void Blinky()
        {
            for (int i = 0; i < 4; i++)
            {
                FrameClear();
                FrameDraw();
                Thread.Sleep(250);
                FrameSet(Pixel.Colour.GreenYellow);
                FrameDraw();
                Thread.Sleep(250);
            }
        }

        public void Pulse()
        {
            FrameClear();
            FrameSet(Pixel.Colour.Green, (ushort)0);
            Debug.Print(DateTime.Now.Millisecond.ToString());
            FrameDraw();
            Debug.Print(DateTime.Now.Millisecond.ToString());
            for (ushort i = 0; i < Length; i++)
            {
                FramePixelSwap((ushort)i, (ushort)(i + 1));
                FrameDraw();
                Debug.Print(DateTime.Now.Millisecond.ToString());
                //Thread.Sleep(50);
            }
            // Debug.Print(DateTime.Now.Millisecond.ToString());

            // turn off the last one
            FrameSet(Pixel.Colour.Black, new ushort[] { (ushort)(0) });
            FrameDraw();
        }

        public void Rainbow()
        {
            FrameClear();
            FrameDraw();
            FrameSetBlocks(new Pixel[] 
                { 
                    Pixel.Colour.Red, 
                    Pixel.Colour.Orange, 
                    Pixel.Colour.Yellow, 
                    Pixel.Colour.Green, 
                    Pixel.Colour.Blue, 
                    Pixel.Colour.Indigo, 
                    Pixel.Colour.Violet 
                });
            FrameDraw();
            Thread.Sleep(1000);

        }
        public void ColourChange()
        {
            FrameClear();
            FrameDraw();
            for (int i = 0; i < 4; i++)
            {
                FrameSet(Pixel.Colour.Red);
                FrameDraw();
                Thread.Sleep(250);
                FrameSet(Pixel.Colour.Green);
                FrameDraw();
                Thread.Sleep(250);
                FrameSet(Pixel.Colour.Blue);
                FrameDraw();
                Thread.Sleep(250);

            }
        }

        #endregion

        #region Primitives
        /// <summary>
        /// Lights up a percentage of the strip
        /// </summary>
        /// <param name="percent">percentage of the strip to light up (0-100)</param>
        /// <param name="colour">Colour to light the strip</param>
        /// <param name="stepUp">Should the strip animate to this level</param>
        /// <param name="increment">Animation step size (percentage)</param>
        /// <param name="stepDelay">ms Delay between animation steps</param>
        public void SetLevel(ushort percent, PixelColour colour = PixelColour.Red, bool stepUp = false, ushort increment = 1, int stepDelay = 250)
        {
            SetLevel(percent, getPixel(colour), stepUp, increment, stepDelay);
        }

        /// <summary>
        /// Lights up a percentage of the strip
        /// </summary>
        /// <param name="percent">percentage of the strip to light (0-100)</param>
        /// <param name="colour">colour to use</param>
        /// <param name="stepUp">Should the strip animate to this level</param>
        /// <param name="increment">Animation step size (percentage)</param>
        /// <param name="stepDelay">ms Delay between animation steps</param>
        public void SetLevel(ushort percent, Pixel colour, bool stepUp = false, ushort increment = 1, int stepDelay = 250)
        {
            SetLevel(percent, new Pixel[] { colour }, stepUp, increment, stepDelay);
        }

        /// <summary>
        /// Lights up a percentage of the strip with the colour changing based on the array of pixels passed
        /// </summary>
        /// <param name="percent">percentage of the strip to light (0-100)</param>
        /// <param name="palette">Colours to use. Strip will be evenly divided. If a colour would only appear at a level above percent, it will not be shown</param>
        /// <param name="stepUp">Should the strip animate to this level</param>
        /// <param name="increment">Animation step size (percentage)</param>
        /// <param name="stepDelay">ms Delay between animation steps</param>
        public void SetLevel(ushort percent, PixelColour[] palette, bool stepUp = false, ushort increment = 1, int stepDelay = 250)
        {
            Pixel[] p = new Pixel[palette.Length];
            for (int i = 0; i < palette.Length; i++)
            {
                p[i] = getPixel(palette[i]);
            }

            SetLevel(percent, p, stepUp, increment, stepDelay);
        }

        /// <summary>
        /// Lights up a percentage of the strip with the colour changing based on the array of pixels passed
        /// </summary>
        /// <param name="percent">percentage of the strip to light (0-100)</param>
        /// <param name="palette">Colours to use. Strip will be evenly divided. If a colour would only appear at a level above percent, it will not be shown</param>
        /// <param name="stepUp">Should the strip animate to this level</param>
        /// <param name="increment">Animation step size (percentage)</param>
        /// <param name="stepDelay">ms Delay between animation steps</param>
        public void SetLevel(ushort percent, Pixel[] palette, bool stepUp = false, ushort increment = 1, int stepDelay = 250)
        {
            if (stepUp)
            {
                for (ushort step = 0; step < percent; step += increment)
                {
                    SetLevel(step, palette);
                    Thread.Sleep(stepDelay);
                }

                SetLevel(percent, palette);
            }
            else
            {

                FrameClear();
                if (percent <= 0)
                // Do nothing - already cleared
                { }
                else if (percent >= 100)
                    FrameSetBlocks(palette);
                else
                {
                    int maxPixel = (Length * percent / 100) - 1;
                    int blockSize = Length / palette.Length;
                    int leftovers = Length % palette.Length;
                    int leftoversUsed = 0;
                    int highestPixelSet = -1;

                    int remainingPixels;

                    for (int i = 0; i < palette.Length; i++)
                    {

                        remainingPixels = maxPixel - highestPixelSet;

                        if (remainingPixels <= 0)
                            break;

                        if (remainingPixels > blockSize)
                        {
                            FrameSet(palette[i], (ushort)(highestPixelSet + 1), (ushort)blockSize);
                            highestPixelSet += blockSize;
                            if (highestPixelSet < maxPixel && leftoversUsed < leftovers)
                            {
                                FrameSet(palette[i], (ushort)(highestPixelSet + 1));
                                highestPixelSet++;
                                leftoversUsed++;
                            }
                        }
                        else
                        {
                            FrameSet(palette[i], (ushort)(highestPixelSet + 1), (ushort)remainingPixels);
                            highestPixelSet += remainingPixels;
                        }

                    }

                }
                FrameDraw();
            }
        }

        #endregion

        /// <summary>
        /// Executes the neopixel pixel definition and then checks if there are 
        /// any external actions commands to be executed
        /// </summary>
        /// <param name="doCycle"></param>
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
