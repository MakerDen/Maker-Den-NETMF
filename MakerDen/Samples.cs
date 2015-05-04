using System;
using System.Threading;
using Glovebox.Netduino.Actuators;
using Glovebox.Netduino.Sensors;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.Netduino;

namespace MakerDen
{
    public static class Samples
    {

        public static void Servo() {

            var servo2 = new ServoPwm(PWMChannels.PWM_PIN_D5, "csb01");//, 20000, 1000, 2500, 180, "csb01");

            servo2.PositionByDegrees(0);

            servo2.PositionByDegrees(90);// = 50;
            servo2.PositionByDegrees(0);
            servo2.PositionByDegrees(90);// = 50;
            servo2.PositionByDegrees(0);

        }
        public static void MoistureLevel() {
            int loopSize = 50;
            using (SensorMoisture level = new SensorMoisture(Microsoft.SPOT.Hardware.Cpu.AnalogChannel.ANALOG_0, 1000, "moisture"))
            {
                for (int i = 0; i < loopSize;i++ )
                {
                    Debug.Print(level.Current.ToString());
                    Debug.Print(level.ToString());
                    Thread.Sleep(1000);
                }
            }
        }
        public static void Distance() {
            int loopSize = 50;
            using (SensorDistance ultrasonic = new SensorDistance(Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D10, 1000, "sonic01"))
            {
                for (int i = 0; i < loopSize; i++)
                {
                    Debug.Print(ultrasonic.Current.ToString());
                    Debug.Print(ultrasonic.ToString());
                }
            }
        }

    }
}
