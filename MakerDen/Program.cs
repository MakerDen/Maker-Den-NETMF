using Coatsy.Netduino.NeoPixel;
using Coatsy.Netduino.NeoPixel.Grid;
using Glovebox.MicroFramework.Sensors;
using Glovebox.Netduino.Actuators;
using Glovebox.Netduino.Sensors;
using Microsoft.SPOT;
//using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;
using System;
using Glovebox.Netduino.Drivers;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
//using NetMf.CommonExtensions;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace MakerDen
{
    public enum Labs
    {
        First = 1,
        Second = 2,
        Third = 3,
        Forth = 4,
        Fifth = 5,
        Sixth = 6


    }
#if NEOPIXEL
    public class Program : MakerBaseNeoPixelRing
#else
    public class Program : MakerBaseIoT
#endif
    {

        private static void EstablishNetworkService(Labs lab)
        {
            if (lab == Labs.Forth)
            {
                StartNetworkServices("csb4", true);
            }
            else if (lab == Labs.Fifth)
            {
                StartNetworkServices("csb5", true, "5C-86-4A-01-08-1C");
            }

        }
        private static void ExecuteStateLoop(SensorTemp temp, SensorLight light, RgbLed rgb, Labs lab)
        {

            ExecuteStateLoop(temp, light, null, rgb, lab);
        }
        private static void ExecuteStateLoop(SensorTemp temp, SensorLight light, SensorDistance ultrasonic, RgbLed rgb, Labs lab)
        {
            //lab4

            if (lab >= Labs.Forth)
            {
                //temp.OnBeforeMeasurement += OnBeforeMeasure;
                temp.OnBeforeMeasurement += MakerDen_OnBeforeMeasurement;
                //temp.OnAfterMeasurement += OnMeasureCompleted;
                temp.OnAfterMeasurement += MakerDen_OnAfterMeasurement;
                light.OnBeforeMeasurement += MakerDen_OnBeforeMeasurement;
                light.OnAfterMeasurement += MakerDen_OnAfterMeasurement;
                Thread.Sleep(Timeout.Infinite);
                //end lab4
                //lab 3
            }
            else// if (lab == Labs.Third)
            {
                while (true)
                {
                    Debug.Print(light.ToString());

                    Debug.Print(temp.ToString());
                    if (ultrasonic != null)
                        Debug.Print(ultrasonic.ToString());
                    Debug.Print(temp.ToString());
                    //changes. original was red only
                    var targetLight = RgbLed.Led.Green;
                    if (temp.Current > 25)
                        targetLight = RgbLed.Led.Blue;
                    if (light.Current > 40)
                        targetLight = RgbLed.Led.Red;

                    rgb.On(targetLight);
                    Thread.Sleep(500);
                    rgb.Off(targetLight);
                    Thread.Sleep(500);
                }  // End of while loop
            }
            //end lab3


        }

        static uint MakerDen_OnAfterMeasurement(object sender, EventArgs e)
        {
            //throw new System.NotImplementedException();
            //original method. this one is just a facade so you can see the flow.
            Debug.Print("AfterMeasurement: " + sender.ToString());
            return OnMeasureCompleted(sender, e);

        }

        static uint MakerDen_OnBeforeMeasurement(object sender, EventArgs e)
        {
            //tried using .Net Micro Framework Common Extensions, however it won't debug with it.
            Debug.Print("BeforeMeasurement: " + sender.ToString());
            //original method. this one is just a facade so you can see the flow.
            return OnBeforeMeasure(sender, e);

        }
        public static void Main()
        {
#if NEOPIXEL
            StartNeoPixel();
#endif
          /*   PWM speaker = new PWM(Cpu.PWMChannel.PWM_5,0,0,PWM.ScaleFactor.Milliseconds,false);
             speaker.Duration = 3 * 1000;
             speaker.Frequency = 505;
             speaker.Start();
             Thread.Sleep(3 * 1000);
             speaker.Dispose();
           */
            //using (SensorSound sound = new SensorSound(Cpu.Pin.GPIO_Pin0, 0, "snd01")) {
            //    sound.StartMeasuring();
                
            //for(int i =0; i<50;i++){

            //    Debug.Print(sound.Current.ToString());
            //  Debug.Print(sound.ToString());
            //        Thread.Sleep(200);
                
            //}
            
            //}
            //using (PiezoSL piezo = new PiezoSL(Pins.GPIO_PIN_D5, "piezo01"))
            //using (Piezo piezo = new Piezo(Cpu.PWMChannel.PWM_5, "piezo01"))
            using (PiezoSL piezo = new PiezoSL(Pins.GPIO_PIN_D5, "piezo01"))
            {
                //piezo.QueueScript
                //twinkle
                piezo.QueueScript("C C G G A A * G ! F F E E D D * C ! G G F F E E * D ! G G F F E E * D ! C C G G A A * G ! F F E E D D * C",4,'h',120);
                piezo.PlayWait();
                piezo.QueueScript("C C G G A A * G ! F F E E D D * C ! G G F F E E * D ! G G F F E E * D ! C C G G A A * G ! F F E E D D * C", 4, 'h', 120);
                //piezo.QueueScript("C C G G A A * G / F ! F E E D D * C / G ! G F F E E * D / G ! G F F E E * D / C ! C G G A A * G / F ! F E E D D * C", 4, 'h', 120);
                piezo.PlayWait();

                return;
                //piezo.Wait();
                //ENTER SANDMAN
                //starts with intro1 and 2, needs to build 
                string intro1 = "! E3 * 0 ! D4 C#4 ! * * C4";
                string intro2 =" ! / E3 E3 E4 E3 E3 D#4 E3 E3 ! D4 C#4 ! * C4";
                    string intro3 = " ! / E3 E3 B3 E3 E3 A#3 E3 E3 A3 E3 G#3 E3 G3 E3 F#3 E3";
                    string masterOfPuppets = intro1 + intro2 + intro3 + intro2 + intro3;
                //EnterSandman = " ! * E3 E3 B3 E3 E3 A#3 E3 E3 A3 E3 G#3 E3 G3 E3 F#3 E3";
                //preverse 1
                    string preverse1 = " ! / E3 F3 B3 E3 F3 C4 E3 F3 C#4 E3 F3 C4 E3 F3 B3 B3";
                //EnterSandman +=" E3 F3 B3 E3 F3 C4 E3 F3 CS4 E3 F3 C4 E3 F3 B3 B3";
                //preverse 2
                string preverse2 =" ! / E3 F3 B3 E3 F3 C4 E3 F3 C#4 E3 F3 C4 E3 F3 B3 0";
                string preverse3 = " ! / E3 F3 B3 E3 F3 C4 E3 F3 G3 FS3 E3 G3 F#3 E3 G3 F#3";
                string endIntro = " ! / G3 F#3 E3 G3 F#3 E3 G3 F#3 E3 D#4 A5 E3 D#4 A5 E3 D#4 A5 E3 D#4 A5 E3 D#4 ! * A5";
                string eightVamp = " ! / E3 E3 E3 E3 E3 E3 E3 E3";
                string verse1 = " ! / 0 G3 A3 0 A#3 A3 G3 A3";
                string verse2 = " ! / A3 0 A3 0";
                string eightVampF = " ! / F#3 F#3 F#3 F#3 F#3 F#3 F#3 F#3";

                masterOfPuppets += preverse1 + preverse2 + preverse1 + preverse3 + endIntro;
                masterOfPuppets += eightVamp + verse1 + eightVamp + verse2;
                masterOfPuppets += eightVamp + verse1 + eightVamp + verse2;
                masterOfPuppets += eightVampF;
                //piezo.QueueScript(masterOfPuppets, 7, 'q', 120);
                //piezo.PlayWait();
                //piezo.QueueClear();
                //return;
                //mario
                //piezo.QueueScript("E7 E7 0 E7 0 C7 E7 0 G7 0 0 0 G6 0 0 0 C7 0 0 G6 0 0 E6 0 0 A6 0 B6 0 A#6 A6 0 # G6 E7 G7 ! A7 0 F7 G7 0 E7 0 C7 D7 B6 0 0 C7 0 0 G6 0 0 E6 0 0 A6 0 B6 0 A#6 A6 0 # G6 E7 G7 ! A7 0 F7 G7 0 E7 0 C7 D7 B6 0 0", 7, 'q', 120);
                //piezo.PlayWait();
                //return;
               //imperial march
                var script = "a3 a3 a3 ^ f3 @ c4 ! a3 ^ f3 @ c4 * a3 ! e4 e4 e4 ^ f4 @ c4 ! g#3 ^ f3 @ c4 * a3 ! a4 ^ a3 @ a3 ! a4 / g#4 g4 $ f#4 f4 / f#4 03 a#3 ! d#4 / d4 c#4 $ c4 b3 / c4 03 $ f3 ! g#3 # f3 $ a3 ! c4 # a3 $ c4 * e4 ! a4 ^ a3 @ a3 ! a4 / g#4 g4 $ f#4 f4 / f#4 03 a#3 ! d#4 / d4 c#4 $ c4 b3 / c4 03 f3 ! g#3 # f3 $ c4 ! a3 # f3 $ c3 * a3 ";
                script = "a4 a4 a4 ^ f4 @ c5 ! a4 ^ f4 ! @ c5 * a4 ! e5 e5 e5 ^ f5 @ c5 ! g#4 ^ f4 @ c5 * a4 ! a5 ^ a4 @ a4 ! a5 / g#5 g5 $ f#5 f5 / f#5 04 a#4 ! d#5 / d5 c#5 $ c5 b4 / c5 04 $ f4 ! g#4 # f4 $ a4 ! c5 # a4 $ c5 * e5 ! a5 ^ a4 @ a4 ! a5 / g#5 g5 $ f#5 f5 / f#5 04 a#4 ! d#5 / d5 c#5 $ c5 b4 / c5 04 f4 ! g#4 # f4 $ c5 ! a4 # f4 $ c4 * a4 ";
                
                //reviewed
                //script = "a4 a4 a4 ^ f4 @ c5 ! a4 ^ f4 ! @ c5 * a4 ! e5 e5 e5 ^ f5 @ c5 ! g#4 ^ f4 @ c5 * a4 ! a5 ^ a4 @ a4 ! a5 / g#5 g5 $ f#5 f5 / f#5 04 a#4 ! d#5 / d5 c#5 $ c5 b4 / c5 04 $ f4 ! g#4 # f4 $ a4 ! c5 # a4 $ c5 * e5 ! a5 ^ a4 @ a4 ! a5 / g#5 g5 $ f#5 f5 / f#5 04 a#4 ! d#5 / d5 c#5 $ c5 b4 / c5 04 f4 ! g#4 # f4 $ c5 ! a4 # f4 $ c4 * a4 ";
                piezo.QueueScript(script.ToUpper(), 4, 'e', 120);
                piezo.PlayWait();
                piezo.QueueClear();
                
                //piezo.QueueScript("E E N E N C E N G N N N - G N N N", 7, 'h', 60);
                //piezo.QueuePlay();

                //while (piezo.IsActive())
                //{
                //    Thread.Sleep(50);
                //}
                //piezo.BeebStartup();
                
                //while (piezo.IsActive()) {
                //    Thread.Sleep(50);
                //}
                //piezo.QueueClear();
                //piezo.BeepAlert();
                //while (piezo.IsActive()) {
                //    Thread.Sleep(50);
                //}
                //piezo.QueueClear();
            }
            //PWMChannels.PWM_PIN_D3
            return;

            var lab = Labs.First;
            EstablishNetworkService(lab);
            using (SensorTemp temp = new SensorTemp(Pins.GPIO_PIN_D8, 1000, "temp01"))
            using (SensorLight light = new SensorLight(AnalogChannels.ANALOG_PIN_A0, 1000, "light01"))
            using (SensorDistance ultrasonic = new SensorDistance(Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D10, 1000, "sonic01"))
            using (rgb = new RgbLed(Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6, "rgb01"))
            {
                ExecuteStateLoop(temp, light, ultrasonic, rgb, lab);
            }



        }
    }
}