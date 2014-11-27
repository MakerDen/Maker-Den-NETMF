using System;
using Microsoft.SPOT;
using System.Threading;
using Glovebox.MicroFramework.IoT;
using Coatsy.Netduino;
using Glovebox.Netduino;
using Coatsy.Netduino.NeoPixel;
using System.Collections;
using MakerDen.Properties;
using Coatsy.Netduino.Helpers;
using Coatsy.Netduino.NeoPixel.Ring;
//using Json.NETMF;

namespace MakerDen {
    public class MakerBaseNeoPixelRing : MakerBaseIoT {
        public static int NumberOfLeds = 12;


        public static NeoPixelRing npr;
        static Thread neoPixelThread;

        private static void CreateCyclesCollection() {
            npr.cycles = new DoCycle[] {
                new DoCycle(Chunks),
                new DoCycle(Lost),
                new DoCycle(npr.BlueMoon),
                new DoCycle(npr.Xbox),
                new DoCycle(npr.ColourWheel),
                new DoCycle(npr.Windmill),
                new DoCycle(npr.CosmicWorm),
                new DoCycle(npr.BackNForth),
                new DoCycle(npr.Sparkles),
                new DoCycle(npr.RomanCandle),
                new DoCycle(npr.FeelingBlue),
                new DoCycle(npr.Duet),
                new DoCycle(npr.Finale),
                //new DoCycle(FinaleJson),
                //new DoCycle(SmallJson),
                //new DoCycle(TestJson),
                // new DoCycle(EncodedJson),
            };
        }

        //private static void EncodedJson()
        //{
        //    Debug.Print(Debug.GC(false).ToString());
        //    Debug.Print(Debug.GC(true).ToString());
        //    string encoded = Resources.GetString(Resources.StringResources.EncodedJson);
        //    string decoded = HttpUtility.HtmlDecode(encoded);
        //    Command command = CommandHelpers.CommandFromJson(decoded);
        //    Debug.Print(Debug.GC(false).ToString());
        //    npr.RunCommand(command);
        //    Debug.Print(Debug.GC(false).ToString());
        //}

        //private static void TestJson()
        //{
        //    Debug.Print(Debug.GC(false).ToString());
        //    Command command = CommandHelpers.CommandFromJson(
        //        Resources.GetString(Resources.StringResources.TestJson));
        //    Debug.Print(Debug.GC(false).ToString());
        //    npr.RunCommand(command);
        //}

        //private static void SmallJson()
        //{
        //    Command command = CommandHelpers.CommandFromJson(
        //        Resources.GetString(Resources.StringResources.SmallJson));
        //    npr.RunCommand(command);
        //}

        protected static void StartNeoPixel() {
            npr = new NeoPixelRing(NumberOfLeds, "neopixel01");

            CreateCyclesCollection();

            neoPixelThread = new Thread(StartNeoPixelThread);
            neoPixelThread.Priority = ThreadPriority.Lowest;
            neoPixelThread.Start();
        }

        // run the neopixel is seperate thread
        private static void StartNeoPixelThread() {

               while (true) {
                for (int i = 0; i < npr.cycles.Length; i++) {
                    npr.FrameClear();
                    npr.ExecuteCycle(npr.cycles[i]);
                }
            }
        }

        private static void Chunks() {

            npr.FrameSetBlocks(new Pixel[] { Pixel.CoolColours.CoolRed, Pixel.CoolColours.CoolGreen, Pixel.CoolColours.CoolBlue });
            npr.FrameDraw();

            for (int i = 0; i < 200; i++) {
                npr.FrameShiftForward(1);
                npr.FrameDraw();
                Thread.Sleep(50);
            }
        }

        private static void Lost() {

            Random rnd = new Random();

            npr.FrameSet(npr.coolPalette, 0, 3);
            //npr.FrameSet(npr.coolPalette[0], 1);

            for (int i = 0; i < 40; i++) {
                for (int p = 0; p < npr.Length; p++) {
                    npr.FrameDraw();
                    npr.FrameShift(1);
                    Thread.Sleep(50);
                }

                for (int p = 0; p < npr.Length; p++) {
                    npr.FrameDraw();
                    npr.FrameShift(-1);
                    Thread.Sleep(25);
                }
            }
        }
    }
}
