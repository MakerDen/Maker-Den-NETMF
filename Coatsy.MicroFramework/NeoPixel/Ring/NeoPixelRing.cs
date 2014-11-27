using System;
using Microsoft.SPOT;
using System.Collections;
using System.Threading;
using Glovebox.MicroFramework.IoT;
using Coatsy.Netduino.Helpers;


namespace Coatsy.Netduino.NeoPixel.Ring {
    public class NeoPixelRing : NeoPixelFrameBase {


        /// <summary>
        /// The order of this cycle names collection must match the over that cycle definitions are added
        /// to the cycles collection
        /// </summary>
        public string[] CycleNames = new string[] { 
            "Lost", "BlueMoon", "Xbox", "ColourWheel", 
            "Windmill", "Comet", 
            "BackNForth", "Sparkles", "RomanCandle",
            "FeelingBlue", "Duet", "Finale",
        };

        ////private const int DefaultLEDCount = 12;
        //public NeoPixelRing(string name)
        //    : base(DefaultLEDCount, name) {
        //}

        public NeoPixelRing(int ledCount, string name)
            : base(ledCount, name) {
        }

        /// <summary>
        /// Executes the neopixel pixel definition and then checks if there are 
        /// any external actions commands to be executed
        /// </summary>
        /// <param name="doCycle"></param>
        public void ExecuteCycle(DoCycle doCycle) {
            try {
                doCycle();
                Thread.Sleep(50);

                var a = GetNextAction();
                while (a != null) {
                    DoAction(a);
                    Thread.Sleep(50);
                    a = GetNextAction();
                }
            }
            catch { ActuatorErrorCount++; }
        }


        private void DoAction(IotAction a) {

            switch (a.cmd) {
                case "play":
                    Command command = CommandHelpers.CommandFromJson(HttpUtility.HtmlDecode(a.parameters));
                    if (command != null) { RunCommand(command); }
                    break;
                case "start":
                    for (int i = 0; i < cycles.Length; i++) {
                        if (a.parameters == CycleNames[i].ToLower()) {
                            cycles[i]();
                            break;
                        }
                    }
                    break;
            }
        }

        public void Finale() {
            Command command = new Command() {
                CommandType = CommandType.Parent,
                Commands = new ArrayList()
                {
                    new Command()
                    {
                        CommandType = CommandType.AllOff
                    },
                    new Command()
                    {
                        CommandType = CommandType.AllOn,
                        PrimaryColour= PixelColour.Aquamarine,
                        PauseAfter=500
                    },
                    new Command()
                    {
                        CommandType = CommandType.LightMultiPixel,
                        PrimaryColour = PixelColour.Crimson,
                        PixelPositions = new int[] {3, 4, 5, 9, 10, 11},
                        PauseAfter=500
                    },
                    new Command()
                    {
                        CommandType=CommandType.Rotate,
                        StepTime=50,
                        Cycles=6,
                        PauseAfter=500
                    },
                    new Command()
                    {
                        CommandType = CommandType.Repeat,
                        PauseBetween = 200,
                        Repetitions = 5,
                        Commands = new ArrayList()
                        {
                            new Command()
                            {
                                CommandType = CommandType.AllOff,
                                PauseAfter=250
                            },
                            new Command()
                            {
                                CommandType = CommandType.AllOn,
                                PrimaryColour = PixelColour.LightGreen,
                                PauseAfter = 250
                            },

                        }
                    },
                    new Command()
                    {
                        CommandType = CommandType.Wait,
                        PauseAfter = 1000
                    },
                    new Command()
                    {
                        CommandType = CommandType.ColourBlocks,
                        ColourSet = new PixelColour[]
                        {
                            PixelColour.Red,
                            PixelColour.Green,
                            PixelColour.Blue
                        },
                        PauseAfter = 0
                    },
                    new Command()
                    {
                        CommandType = CommandType.Repeat,
                        Repetitions = 36,
                        PauseBetween = 50,
                        PauseAfter = 1000,
                        Commands = new ArrayList()
                        {
                            new Command()
                            {
                                CommandType = CommandType.Rotate,
                                RotateIncrement = -1
                            }
                        }
                    },
                    new Command()
                    {
                        CommandType = CommandType.AllOff,
                        PauseAfter = 1000
                    }
                }
            };

            RunCommand(command);
        }

        public void Duet() {
            FrameSetBlocks(new Pixel[] { Pixel.Colour.Blue, Pixel.Colour.Cyan, Pixel.Colour.Blue, Pixel.Colour.Cyan, Pixel.Colour.Blue, });
            FrameDraw();
            for (int i = 0; i < 36; i++) {
                FrameShift();
                FrameDraw();
                Thread.Sleep(50);
            }
            FrameSetBlocks(new Pixel[] { Pixel.Colour.Yellow, Pixel.Colour.Cyan, Pixel.Colour.Magenta, Pixel.Colour.Black, });
            FrameDraw();
            for (int i = 0; i < 36; i++) {
                FrameShift(-1);
                FrameDraw();
                Thread.Sleep(50);
            }
        }

        public void FeelingBlue() {
            FrameSet(new Pixel[] { Pixel.Colour.DarkBlue, Pixel.Colour.Cyan });
            FrameDraw();
            for (int i = 0; i < 36; i++) {
                FrameShift();
                FrameDraw();
                Thread.Sleep(50);
            }
        }

        public void RomanCandle() {
            SpinColourOnBackground(Pixel.Colour.Green, Pixel.Colour.Red, 2, 50);
            Thread.Sleep(500);
            SpinColourOnBackground(Pixel.Colour.Green, Pixel.Colour.Yellow, 2, 50);
            Thread.Sleep(500);
            SpinColourOnBackground(Pixel.Colour.Green, Pixel.Colour.White, 2, 50);
            Thread.Sleep(500);
            SpinColourOnBackground(Pixel.Colour.Red, Pixel.Colour.Green, 2, 50);
            Thread.Sleep(500);
            SpinColourOnBackground(Pixel.Colour.Red, Pixel.Colour.Yellow, 2, 50);
            Thread.Sleep(500);
            SpinColourOnBackground(Pixel.Colour.Red, Pixel.Colour.Blue, 2, 50);
        }

        public void Sparkles() {
            FrameClear();
            FrameDraw();
            Random rand = new Random();

            for (int i = 0; i < 5; i++) {

                for (int j = 0; j < 120; j++) {
                    FrameSet(
                        colourList[rand.Next(colourList.Length)],
                        (ushort)rand.Next(Length)
                        );
                    FrameDraw();
                    Thread.Sleep(25);
                }

                for (int j = 0; j < 12; j++) {
                    FrameShift();
                    FrameDraw();
                    Thread.Sleep(50);
                }
                for (int j = 0; j < 12; j++) {
                    FrameShift(-1);
                    FrameDraw();
                    Thread.Sleep(50);
                }
            }
        }

        public void BackNForth() {

            FrameSetBlocks(new Pixel[] { Pixel.Colour.Red, Pixel.Colour.Green, Pixel.Colour.Blue });
            FrameDraw();
            Thread.Sleep(200);
            for (int j = 1; j < 5; j++) {
                for (int i = 0; i < 20; i++) {
                    FrameShift();
                    FrameDraw();
                    Thread.Sleep(100);
                }
                for (int h = 0; h < 20; h++) {
                    FrameShift(-1);
                    FrameDraw();
                    Thread.Sleep(100);
                }
            }
        }

        // ht Sophie Byrne for naming this one
        public void CosmicWorm() {
            FrameClear();
            FrameSet(new Pixel(0, 16, 0), (ushort)1);
            FrameSet(new Pixel(0, 32, 0), (ushort)2);
            FrameSet(new Pixel(0, 64, 0), (ushort)3);
            FrameSet(new Pixel(0, 128, 0), (ushort)4);
            FrameSet(Pixel.Colour.Green, (ushort)5);
            FrameSet(Pixel.Colour.Green, (ushort)6);

            FrameDraw();
            Thread.Sleep(200);

            for (int i = 0; i < 100; i++) {
                FrameShift();
                FrameDraw();
                Thread.Sleep(100);
            }
        }

        public void Windmill() {
            FrameSetBlocks(new Pixel[] { Pixel.Colour.Red, Pixel.Colour.Green, Pixel.Colour.Blue });
            FrameDraw();

            Thread.Sleep(200);

            for (int i = 0; i < 100; i++) {
                FrameShift();
                FrameDraw();
                Thread.Sleep(100);
            }
        }

        public void ColourWheel() {
            for (int i = 0; i < colourList.Length; i = i + 5) {
                SpinColour(colourList[i], 2, 50);
            }
        }

        public void Xbox() {
            GreenSegments();
            Thread.Sleep(3000);
            FrameClear();
            FrameDraw();
        }

        public void BlueMoon() {
            SpinColour(Pixel.Colour.Cyan);
            SpinColour(Pixel.Colour.Cyan, 1, 100);
            SpinColour(Pixel.Colour.Blue, 2, 50);
        }

        public void GreenRing() {

            FrameSet(Pixel.Colour.Green);
            FrameDraw();
        }

        public void GreenSegments() {

            FrameSet(new Pixel[] { Pixel.Colour.Green, Pixel.Colour.Black, Pixel.Colour.Black, Pixel.Colour.Black});
            FrameDraw();
            for (int i = 0; i < 12; i++) {
                Thread.Sleep(250);
                FrameShift(3);
                FrameDraw();
            }

            GreenRing();
        }

    }
}
