using System;
using Microsoft.SPOT;
using Coatsy.Netduino.NeoPixel;
using System.Threading;

namespace Coatsy.MicroFramework.NeoPixel {

    /// <summary>
    /// NeoPixel Grid Privatives, builds on Frame Primatives
    /// </summary>
    public class NeoPixelGridBase : NeoPixelFrameBase {
        public ushort Columns { get; private set; }
        public ushort Rows { get; private set; }


        public NeoPixelGridBase(ushort columns, ushort rows, string name)
            : base(columns * rows, name) {
            this.Columns = columns;
            this.Rows = rows;

            FrameClear();

        }

        public ushort PointPostion(ushort row, ushort column) {
            if (row >= Rows || column >= Columns) { return 0; }
            ushort result = (ushort)(row * Columns + column);
            return result;
        }

        public void PointColour(ushort row, ushort column, Pixel pixel) {
            if (row > Rows || column > Columns) { return; }

            int pixelNumber = row * Columns + column;
            Frame[pixelNumber] = pixel;
        }

        public void RowRollRight(ushort rowIndex) {
            rowIndex = (ushort)(rowIndex % Rows);

            Pixel temp = Frame[rowIndex * Columns + Columns - 1];
            for (int col = rowIndex * Columns + Columns - 1, count = 0; count < Columns - 1; col--, count++) {
                Frame[col] = Frame[col - 1];
            }

            Frame[rowIndex * Columns] = temp;
        }

        public void ColumnRollDown(ushort columnIndex) {
            int current;
            columnIndex = (ushort)(columnIndex % Columns);
            Pixel temp = Frame[Columns * (Rows - 1) + columnIndex];

            for (int row = Rows - 2; row >= 0; row--) {
                current = row * Columns + columnIndex;

                Frame[current + Rows] = Frame[current];
            }
            Frame[columnIndex] = temp;
        }

        public void RowDrawLine(ushort rowIndex, Pixel pixel) {
            for (int i = rowIndex * Columns; i < rowIndex * Columns + Columns; i++) {
                Frame[i] = pixel;
            }
        }

        public void RowDrawLine(ushort rowIndex, Pixel[] pixel) {
            for (int i = 0; i < rowIndex * Columns + Columns; i++) {
                Frame[i] = pixel[i % pixel.Length];
            }
        }

        public void ColumnDrawLine(ushort columnIndex, Pixel pixel) {
            for (int r = 0; r < Rows; r++) {
                Frame[columnIndex + (r * Columns)] = pixel;
            }
        }

        public void ColumnDrawLine(ushort columnIndex, Pixel[] pixel) {
            for (int r = 0; r < Rows; r++) {
                Frame[columnIndex + (r * Columns)] = pixel[r % pixel.Length];
            }
        }

        public void DrawBox(ushort startRow, ushort startColumn, ushort width, Pixel pixel) {
            if (width + startRow > Rows || width + startColumn > Columns) { return; }

            FrameSet(pixel, (ushort)(startRow * Columns + startColumn), width);

            int startPos = startRow * Columns + ((width - 1) * Columns + startColumn);

            FrameSet(pixel, (ushort)startPos, width);

            // draw sides of boxes
            for (ushort r = (ushort)(startRow + 1); r < startRow + width - 1; r++) {
                PointColour(r, startColumn, pixel);
                PointColour(r, (ushort)(width - 1 + startColumn), pixel);
            }
        }
    }
}
