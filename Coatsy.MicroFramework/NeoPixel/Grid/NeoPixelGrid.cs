using System;
using Microsoft.SPOT;
using Coatsy.Netduino.NeoPixel;
using System.Threading;
using Glovebox.IoT;

namespace Coatsy.Netduino.NeoPixel.Grid
{

    public class NeoPixelGrid : NeoPixelGridBase
    {
        public NeoPixelGrid(ushort columns, ushort rows, ushort panels, string name)
            : base(columns, rows, panels, name) { }

        Random random = new Random();


        public void Rain()
        {
            FrameSet(PaletteCoolLowPower[1]);
            RowDrawLine(0, Pixel.ColourLowPower.CoolBlue);

            for (int t = 0; t < 50; t++)
            {
                int nr = random.Next();
                ushort col = (ushort)(nr % Columns);

                ColumnRollDown(col);
                PointColour(0, col, PaletteCoolLowPower[random.Next() % PaletteCoolLowPower.Length]);
                for (ushort i = 0; i < Columns; i++)
                {
                    ColumnRollDown(i);
                }

                FrameDraw();
            }

        }


        public void Snow()
        {
            Pixel[] backgrounds = new Pixel[] { new Pixel(1, 0, 0), new Pixel(0, 1, 0), new Pixel(0, 0, 1), new Pixel(1, 1, 0), new Pixel(0, 1, 1), new Pixel(1, 0, 1) };
            int NUMDOTS = 18;
            int count = 0;

            FrameSet(backgrounds[random.Next() % backgrounds.Length]);
            count++;

            for (int i = 0; i < NUMDOTS; i++)
            {

                var v1 = (ushort)(random.Next() % Rows);
                var v2 = (ushort)(random.Next() % Columns);
                PointColour(v1, v2, PaletteCoolLowPower[random.Next() % PaletteCoolLowPower.Length]);
            }

            for (int c = 0; c < 40; c++)
            {
                for (ushort i = 0; i < Rows; i++)
                {
                    ColumnRollDown(i);
                    ColumnRollRight(i);
                }

                FrameDraw();
                Util.Delay(25);
            }
        }

        public void StarSquare()
        {
            for (int i = 0; i < 50; i++)
            {
                DrawBox(0, 0, 8, PaletteCoolLowPower[i % PaletteCoolLowPower.Length]);
                DrawBox(1, 1, 6, PaletteCoolLowPower[(i + 1) % PaletteCoolLowPower.Length]);
                DrawBox(2, 2, 4, PaletteCoolLowPower[(i + 2) % PaletteCoolLowPower.Length]);
                DrawBox(3, 3, 2, PaletteCoolLowPower[(i + 3) % PaletteCoolLowPower.Length]);
                FrameDraw();
            }
        }

        public void Squares()
        {
            ushort[] outterSquare = new ushort[] { 8, 9, 10, 11, 12, 13, 16, 21, 24, 29, 32, 37, 40, 45, 48, 49, 49, 50, 51, 52, 53 };
            ushort[] innerSquare = new ushort[] { 17, 18, 19, 20, 25, 28, 33, 36, 41, 42, 43, 44 };
            FrameSet(Pixel.ColourLowPower.WarmRed, outterSquare);
            FrameSet(Pixel.ColourLowPower.WarmRed, innerSquare);
        }

        public void Flag()
        {
            FrameSet(Pixel.ColourLowPower.CoolGreen);
            RowDrawLine(2, Pixel.ColourLowPower.CoolBlue);
            RowDrawLine(3, Pixel.ColourLowPower.CoolBlue);
            ColumnDrawLine(1, Pixel.ColourLowPower.CoolRed);    
            ColumnDrawLine(2, Pixel.ColourLowPower.CoolRed);

            FrameDraw();

            for (int t = 0; t < 50; t++)
            {
                for (ushort i = 0; i < 8; i++)
                {
                    ColumnRollRight(i);
                }

                for (ushort c = 0; c < Columns; c++)
                {
                    ColumnRollDown(c);
                }

                FrameDraw();
                Util.Delay(50);
            }
        }


        public void Dotty()
        {
            while (true)
            {
                for (ushort r = 0; r < 8; r++)
                {
                    for (ushort c = 0; c < 8; c++)
                    {
                        FrameSet(Pixel.Colour.Black);
                        PointColour(r, c, Pixel.Colour.Purple);
                        FrameDraw();
                    }
                }
            }
        }
    }
}
