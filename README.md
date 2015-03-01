*Documentation in progress*

## First time deploying code to your Netduino
After either downloading or cloning the source code you will need to reset the .NET Micro Framework Deployment Transport.  

1. Ensure your Netduino is connected to your PC via the USB cable.  
2. Right mouse click the MakerDen project, and select Properties.  
3. From the properties page select the .NET Micro Framework 
4. Select Emulator from the Transport dropdown then reselect USB and your Netduino.  

The project should now deploy correctly to your Netduino.

## What is the Internet of Things Solution Accelerator?

The IoT Framework for the .NET Micro Framework provides a pluggable foundation to support sensors, actuators, data serialisation, communications, and command and control. 

## Getting Started

The getting started lab code and the [complete Maker Den Lab Guide](https://github.com/MakerDen/IoT-Maker-Den-NETMF/blob/master/MakerDen/Lab%20Code/IoT%20Maker%20Den%20v2.0.pdf)
can be found in the Lab Code folder in the Maker Den Project.


![Alt text](https://github.com/MakerDen/IoT-Maker-Den-NETMF/blob/master/MakerDen/Lab%20Code/Maker%20Den%20IoT%20Framework.jpg)


## Extensible/pluggable framework supporting

1. Sensors
 * Physical: Light, Sound, Temperature (onewire), Servo
 * Virtual: Memory Usage, Diagnostics
 * Sensor data serialised to a JSON schema

2. Actuators
 * RGB, RGB PWM, Piezo, Relay
 * NeoPixel - Supports Strips, Rings, and Grids
    - Low and high level pixel frame transformation primitives 
    - Extensive colour library support along with some predefined palettes
    - NeoPixel Grids library adds alphanumeric character drawing and scrolling capability 

3. Command and Control
 * Control relays, start NeoPixels etc via the communications layer

4. Communications
 * Pluggable – currently implemented on MQTT (MQTT Server running on Azure)

5. Supported and Tested
 * Netduino 2 Plus and Gadgeteer
 * Supports Visual Studio 2012 and 2013, 2013 preferred.

 

## Programming Models

### Declarative Event Driven Model

    using Glovebox.MicroFramework.Sensors;
    using Glovebox.Netduino.Actuators;
    using Glovebox.Netduino.Sensors;
    using Microsoft.SPOT;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;
    using System.Threading;
    
    namespace MakerDen {
        public class Program : MakerBaseIoT  {
            public static void Main() {
                // main code marker
                
                //Replace the "emul" which is the name of the device with a unique 3 to 5 character name
                //use your initials or something similar.  This code will be visible on the IoT Dashboard
                StartNetworkServices("emul", true);
                
                
                // sensor timer value 10000 measure every 10000 milliseconds (10 seconds)
                using (SensorTemp temp = new SensorTemp(Pins.GPIO_PIN_D8, 10000, "temp01"))
                using (SensorLight light = new SensorLight(AnalogChannels.ANALOG_PIN_A0, 1000, "light01"))
                using (rgb = new RgbLed(Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6, "rgb01")) {
                
                    temp.OnBeforeMeasurement += OnBeforeMeasure;
                    temp.OnAfterMeasurement += OnMeasureCompleted;
                    light.OnBeforeMeasurement += OnBeforeMeasure;
                    light.OnAfterMeasurement += OnMeasureCompleted;
                    
                    Thread.Sleep(Timeout.Infinite);
                }
            }
        }
    }

### Imperative Model

    using Glovebox.MicroFramework.Sensors;
    using Glovebox.Netduino.Actuators;
    using Glovebox.Netduino.Sensors;
    using Microsoft.SPOT;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;
    using System.Threading;
    
    namespace MakerDen {
        public class Program : MakerBaseIoT  {
            public static void Main() {
                // main code marker
    
                // sensor timer value -1 disables auto measure
                using (SensorLight light = new SensorLight(AnalogChannels.ANALOG_PIN_A0, -1, "light01"))
                using (rgb = new RgbLed(Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D6, "rgb01")) {
    
                    while (true) {
                        if (light.Current < 60) {
                            rgb.On(RgbLed.Led.Red);
                            rgb.Off(RgbLed.Led.Green);
                        }
                        else {
                            rgb.Off(RgbLed.Led.Red);
                            rgb.On(RgbLed.Led.Green);
                        }
                        Thread.Sleep(100);
                    }
                }
            }
        }
    }


## Creating a Sensor

    using Glovebox.MicroFramework.Base;
    using Microsoft.SPOT.Hardware;
    using System;

    namespace Glovebox.Netduino.Sensors {
        class SensorLdr : SensorBase {

            protected AnalogInput ldrAnalogPin;

            public SensorLdr(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
                // SensorBase constructor: sensor type, sensor unit, number of values to collect per sample, 
                // sample rate, target name for command and control
                : base("light", "p", ValuesPerSample.One, SampleRateMilliseconds, name) {

                ldrAnalogPin = new AnalogInput(pin, -1);

                // after initalisation call StartMeasuring() to start sensor sampling at defined sample rate
                StartMeasuring();
            }


            protected override void Measure(double[] value) {
                value[0] = (int)(ldrAnalogPin.Read() * 100);
            }

            protected override string GeoLocation() {
                return string.Empty;
            }

            public override double Current {
                get { return (int)(ldrAnalogPin.Read() * 100); }
            }

            protected override void SensorCleanup() {
                ldrAnalogPin.Dispose();
            }
        }
    }






