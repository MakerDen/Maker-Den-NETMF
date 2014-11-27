using System;
using Microsoft.SPOT;
using System.Collections;
using System.Threading;
using Glovebox.MicroFramework.IoT;
using Coatsy.Netduino.Helpers;


namespace Coatsy.Netduino.NeoPixel {
    public class NeoPixelRing : NeoPixelBase {


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

        private const int DefaultLEDCount = 12;
        public NeoPixelRing(string name)
            : base(DefaultLEDCount, name) {
        }

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
            ColourBlocks(new Pixel[] { Pixel.Colour.Blue, Pixel.Colour.Cyan, Pixel.Colour.Blue, Pixel.Colour.Cyan, Pixel.Colour.Blue, });
            for (int i = 0; i < 36; i++) {
                RotateDisplay();
                Thread.Sleep(50);
            }
            ColourBlocks(new Pixel[] { Pixel.Colour.Yellow, Pixel.Colour.Cyan, Pixel.Colour.Magenta, Pixel.Colour.Black, });
            for (int i = 0; i < 36; i++) {
                RotateDisplay(-1);
                Thread.Sleep(50);
            }
        }

        public void FeelingBlue() {
            AlternateColours(new Pixel[] { Pixel.Colour.DarkBlue, Pixel.Colour.Cyan });
            for (int i = 0; i < 36; i++) {
                RotateDisplay();
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
            AllOff();
            Random rand = new Random();

            for (int i = 0; i < 5; i++) {

                for (int j = 0; j < 120; j++) {
                    Set1Pixel(
                        colourList[rand.Next(colourList.Length)],
                        rand.Next(pixelCount)
                        );
                    Thread.Sleep(25);
                }

                for (int j = 0; j < 12; j++) {
                    RotateDisplay();
                    Thread.Sleep(50);
                }
                for (int j = 0; j < 12; j++) {
                    RotateDisplay(-1);
                    Thread.Sleep(50);
                }
            }
        }

        public void BackNForth() {
            Pixel[] start = new Pixel[]
            {
                Pixel.Colour.Red,
                Pixel.Colour.Red,
                Pixel.Colour.Red,
                Pixel.Colour.Red,
                Pixel.Colour.Green,
                Pixel.Colour.Green,
                Pixel.Colour.Green,
                Pixel.Colour.Green,
                Pixel.Colour.Blue,
                Pixel.Colour.Blue,
                Pixel.Colour.Blue,
                Pixel.Colour.Blue
            };

            Update(start);
            Thread.Sleep(200);
            for (int j = 1; j < 5; j++) {
                for (int i = 0; i < 20; i++) {
                    RotateDisplay();
                    Thread.Sleep(100);
                }
                for (int h = 0; h < 20; h++) {
                    RotateDisplay(-1);
                    Thread.Sleep(100);
                }
            }
        }

        public void Comet() {
            Pixel[] start = new Pixel[]
            {
                Pixel.Colour.Black,
                new Pixel(0, 16, 0),
                new Pixel(0, 32, 0),
                new Pixel(0, 64, 0),
                new Pixel(0, 128, 0),
                Pixel.Colour.Green,
                Pixel.Colour.Green,
                Pixel.Colour.Black,
                Pixel.Colour.Black,
                Pixel.Colour.Black,
                Pixel.Colour.Black,
                Pixel.Colour.Black,
                Pixel.Colour.Black,
            };

            Update(start);
            Thread.Sleep(200);

            for (int i = 0; i < 100; i++) {
                RotateDisplay();
                Thread.Sleep(100);
            }
        }

        public void Windmill() {
            Pixel[] start = new Pixel[]
            {
                Pixel.Colour.Red,
                Pixel.Colour.Red,
                Pixel.Colour.Red,
                Pixel.Colour.Red,
                Pixel.Colour.Green,
                Pixel.Colour.Green,
                Pixel.Colour.Green,
                Pixel.Colour.Green,
                Pixel.Colour.Blue,
                Pixel.Colour.Blue,
                Pixel.Colour.Blue,
                Pixel.Colour.Blue
            };

            Update(start);
            Thread.Sleep(200);

            for (int i = 0; i < 100; i++) {
                RotateDisplay();
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
            AllOff();
        }

        public void BlueMoon() {
            SpinColour(Pixel.Colour.Cyan);
            SpinColour(Pixel.Colour.Cyan, 1, 100);
            SpinColour(Pixel.Colour.Blue, 2, 50);
        }

        public void GreenRing() {

            Update(FullRing(Pixel.Colour.Green));
        }

        public void GreenSegments() {

            Update(segment1);
            for (int i = 0; i < 12; i++) {
                Thread.Sleep(250);
                RotateDisplay(3);
            }

            GreenRing();
        }

    }
}
