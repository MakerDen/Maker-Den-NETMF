#region References

using System;
using System.Globalization;
using Microsoft.SPOT.Hardware;

#endregion

namespace Glovebox.Adafruit.Mini8x8Matrix.Driver {

    public class Ht16K33I2cConnection {

        #region Fields

        I2CDevice.I2CWriteTransaction st;
        private readonly I2CDevice matrix8x8;
        static int timeOut = 10;

        #endregion

        /// <summary>
        /// Create an HT16K33 I2C Driver for use by Adafruit Mini LED 8x8 Matrix 
        /// </summary>
        /// <param name="matrix8x8">pass in a new I2CDevice(new I2CDevice.Configuration(0x70, 200)) object</param>
        public Ht16K33I2cConnection(I2CDevice matrix8x8) {

          //  matrix8x8 = new I2CDevice(new I2CDevice.Configuration(0x70, 200));
            this.matrix8x8 = matrix8x8;
            FrameInit();
        }

        #region Ht16K33 I2C Control Methods


        public void Write(byte[] frame) {
            st = I2CDevice.CreateWriteTransaction(frame);
            matrix8x8.Execute(new I2CDevice.I2CTransaction[] { st }, timeOut);
        }

        public void FrameSetBlinkRate(byte br) {
            Write(new byte[] { (byte)(0x80 | 0x01 | (byte)br), 0x00 });
        }

        public void FrameSetBrightness(byte level) {
            if (level > 15) { level = 15; }
            Write(new byte[] { (byte)(0xE0 | level), 0x00 });
        }

        private void FrameInit() {
            Write(new byte[] { 0x21, 0x00 });
            Write(new byte[] { 0xA0, 0x00 });
        }

        #endregion

    }
}