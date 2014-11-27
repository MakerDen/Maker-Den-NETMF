using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

/// This code is provided as-is, without any warrenty, so use it at your own risk.
/// You can freely use and modify this code.
namespace Coatsy.Netduino.NeoPixel
{

    /// <summary>
    /// Class representing an SPI-driven NeoPixel system
    /// </summary>
    public class NeoPixelSPI
    {
        private byte[] bitZero = new byte[] { (byte)128 };
        private byte[] bitOne = new byte[] { (byte)252 };
        private byte[] bitSilence = new byte[] { 0 };

        private SPI spi;

        /// <summary>
        /// Creates a new NeoPixelSPI with a given chip-select pin, using a given SPI module
        /// </summary>
        /// <param name="chipSelectPin">chip-select pin</param>
        /// <param name="spiModule">SPI module</param>
        public NeoPixelSPI(Cpu.Pin chipSelectPin, SPI.SPI_module spiModule)
        {
            uint speed = 6666; // 6.666 MHz
            SPI.Configuration spiConfig = new SPI.Configuration(chipSelectPin, false, 0, 0, false, true, speed, spiModule);
            this.spi = new SPI(spiConfig);
        }

        /// <summary>
        /// Creates a new NeoPixelSPI with a given SPI configuration<br />
        /// Please note, that the speed should be 6666 (6.666 MHz) otherwise the NeoPixel might not work properly
        /// </summary>
        /// <param name="spiConfiguration"></param>
        public NeoPixelSPI(SPI.Configuration spiConfiguration)
        {
            this.spi = new SPI(spiConfiguration);
        }

        /// <summary>
        /// Shows one pixel, assuming there is only one
        /// </summary>
        /// <param name="pixel">pixel to show</param>
        public void ShowOnePixel(Pixel pixel)
        {
            if (pixel == null)
            {
                return;
            }

            byte[] data = pixel.ToTransferBytes(bitZero, bitOne);
            if ((data == null) || (data.Length == 0))
            {
                return;
            }
            this.spi.Write(data);
            this.SendFinish();
        }

        /// <summary>
        /// Shows the given pixels, assuming they are in the correct order
        /// </summary>
        /// <param name="pixels"></param>
        public void ShowPixels(Pixel[] pixels)
        {
            this.ShowPixels(pixels, 0, pixels.Length);
        }

        /// <summary>
        /// Shows the given pixels, assuming they are in the correct order
        /// </summary>
        /// <param name="pixels">array of pixels</param>
        /// <param name="start">index to start</param>
        /// <param name="count">number of pixels to send</param>
        public void ShowPixels(Pixel[] pixels, int start, int count)
        {
            if ((pixels == null) || (pixels.Length == 0))
            {
                return;
            }
            if (start < 0)
            {
                start = 0;
            }
            if (start + count > pixels.Length)
            {
                count = pixels.Length - start;
            }
            int bitLenPart = 24 * bitZero.Length;
            byte[] data = new byte[pixels.Length * bitLenPart];
            int pos = 0;
            byte[] partData = null;
            Pixel onePixel = null;
            for (int i = start; i < start + count; i++)
            {
                onePixel = pixels[i];
                partData = onePixel.ToTransferBytes(bitZero, bitOne);
                if (partData == null)
                {
                    break;
                }
                Array.Copy(partData, 0, data, pos, partData.Length);
                pos = pos + bitLenPart;
                partData = null;
            }
            this.spi.Write(data);
            this.SendFinish();
        }

        /// <summary>
        /// Sends a pseudo-finish sequence to the wire indicating the end of transmisson
        /// </summary>
        private void SendFinish()
        {            
            // send "low" and wait
            this.spi.Write(bitSilence);
            Thread.Sleep(1);
        }

    }
}
