using System;
using Microsoft.SPOT;

namespace Coatsy.Netduino.NeoPixel.Grid {
    public class NeoPixelGrid8x8 : NeoPixelGrid {

        byte[] a = new byte[8] { 0, 60, 66, 66, 126, 66, 66, 0 };
        ulong[] aphabetMask = new ulong[] { 
            0x002222223e22221c, //a
            0x001e22221e22221e, //b
            0x001c22020202221c, //c
            0x001e22222222221e, //d
            0x003e02021e02023e, //e
            0x000202021e02023e, //f
            0x001c22223a02221c, //g
            0x002222223e222222, //h
            0x003e08080808083e, //i
            0x000608080808083e, //j
            0x0022120a060a1222, //k
            0x003e020202020202, //l
            0x00222222222a3622, //m
            0x002222322a262222, //n
            0x001c22222222221c, //o
            0x000202021e22221e, //p
            0x002c122a2222221c, //q
            0x0022120a1e22221e, //r
            0x001c22100804221c, //s
            0x000808080808083e, //t
            0x001c222222222222, //u
            0x0008142222222222, //v
            0x0022362a22222222, //w
            0x0022221408142222, //x
            0x0008080808142222, //y
            0x003e02040810203e, //z
            };


        ulong[] alphabetLowercase = new ulong[] {
            0x0024243C24180000, // a
            0x001C241C241C0000, // b
            0x0018240424180000, // c
            0x001C2424241C0000, // d
            0x003C041C043C0000, // e
            0x0004041C043C0000, // f
            0x0018243404380000, // g
            0x0024243C24240000, // h
            0x0038101010380000, // i
            0x0018242020200000, // j
            0x0024140C14240000, // k
            0x003C040404040000, // l
            0x0022222A36220000, // m
            0x0022322A26220000, // n
            0x0018242424180000, // o
            0x0004041C241C0000, // p
            0x4038242424180000, // q
            0x0024241C241C0000, // r
            0x001C201804380000, // s
            0x00080808081C0000, // t
            0x0018242424240000, // u
            0x0008142222220000, // v
            0x0022362A22220000, // w
            0x0022140814220000, // x
            0x0008080814220000, // y
            0x003C0408103C0000, // z
        };

        ulong[] numberMask = new ulong[] {
            0x001c22262a32221c, //0
            0x003e080808080a0c, //1
            0x003e04081020221c, //2
            0x001c22201820221c, //3
            0x0008083e0a020202, //4
            0x001c2220201e023e, //5
            0x001c22221e02023c, //6
            0x000202040810203e, //7
            0x001c22221c22221c, //8
            0x002020203c22221c, //9
        };

        ulong[] symbolMask = new ulong[] {
            0x000808083E080808, // +
            0x000000003E000000, // -
            0x00412A142A142A41, // *
            0x0001020408102040, // /
            0x0000003E003E0000, // =
            0x00000C0C000C0C00, // :
            0x00000C0C00000000, // .
            0x00020C0C00000000, // ,
            0x00143E543E153E14, // $
            0x0000081C3E7F7F36, // heart        
        };

        public enum Letters {
            a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z
        }

        public NeoPixelGrid8x8(string name)
            : base(8, 8, name) {
        }

        public void DrawLetter(Letters l, Pixel colour) {
            DrawLetter((byte)l, colour);
        }

        public void DrawLetter(byte letter, Pixel colour) {
            ulong mask = 1;
            ushort pos = 0;
            while (pos < Length) {
                if ((aphabetMask[letter % aphabetMask.Length] & mask) == 0) {
                    FrameSet(Pixel.Colour.Black, pos);
                }
                else {
                    FrameSet(colour, pos);
                }
                pos++;
                mask = mask << 1;
            }
        }

        public void DrawNumber(byte number, Pixel colour) {
            ulong mask = 1;
            ushort pos = 0;
            while (pos < Length) {
                if ((numberMask[number % numberMask.Length] & mask) == 0) {
                    FrameSet(Pixel.Colour.Black, pos);
                }
                else {
                    FrameSet(colour, pos);
                }
                pos++;
                mask += mask;
            }
        }

        public void DrawLowercae(byte number, Pixel colour) {
            ulong mask = 1;
            ushort pos = 0;
            while (pos < Length) {
                if ((alphabetLowercase[number % alphabetLowercase.Length] & mask) == 0) {
                    FrameSet(Pixel.Colour.Black, pos);
                }
                else {
                    FrameSet(colour, pos);
                }
                pos++;
                mask += mask;
            }
        }

        public void DrawSymbols(byte number, Pixel colour) {
            ulong mask = 1;
            ushort pos = 0;
            while (pos < Length) {
                if ((symbolMask[number % symbolMask.Length] & mask) == 0) {
                    FrameSet(Pixel.Colour.Black, pos);
                }
                else {
                    FrameSet(colour, pos);
                }
                pos++;
                mask += mask;
            }
        }
    }
}
