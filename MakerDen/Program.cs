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
            var lab = Labs.First;
            EstablishNetworkService(lab);
            var servo2 = new ServoPwm(PWMChannels.PWM_PIN_D5,"csb01");//, 20000, 1000, 2500, 180, "csb01");
            //servo.
            servo2.PositionByDegrees(0);

            servo2.PositionByDegrees(90);// = 50;
            servo2.PositionByDegrees(0);
            //servo2.SetDirection(true);
            servo2.PositionByDegrees(90);// = 50;
            servo2.PositionByDegrees(0);

            
            //servo2.Position(1500);
            return;
            SensorMoisture moisture = new SensorMoisture(Microsoft.SPOT.Hardware.Cpu.AnalogChannel.ANALOG_0, 1000, "moisture");
            while (true)
            {
                Debug.Print(moisture.Current.ToString());
                Debug.Print(moisture.ToString());
                Thread.Sleep(1000);
            }
            using (SensorTemp temp = new SensorTemp(Pins.GPIO_PIN_D8, 1000, "temp01"))
            using (SensorLight light = new SensorLight(AnalogChannels.ANALOG_PIN_A0, 1000, "light01"))
            using (SensorDistance ultrasonic = new SensorDistance(Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D10, 1000, "sonic01"))
            using (rgb = new RgbLed(Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6, "rgb01"))
            {

                /*var servo = new ServoPwm(Microsoft.SPOT.Hardware.Cpu.PWMChannel.PWM_0, "servoCSB");
                var x = servo.CurrentPosition;
                servo.Reset();
                servo.PositionByDegrees(90);
                */
                //servo.Action( new Glovebox.MicroFramework.IoT.IotAction(){cmd=}
                ExecuteStateLoop(temp, light, ultrasonic, rgb, lab);
            }



        }
    }
}