*Documentation in progress*

## First time deploying code to your Netduino
After either downloading or cloning the source code you will need to reset the .NET Micro Framework Deployment Transport.  

1. Ensure your Netduino is connected to your PC via the USB cable.  
2. Right mouse click the MakerDen project, and select Properties.  
3. From the properties page select the .NET Micro Framework 
4. Select Emulator from the Transport dropdown then reselect USB and your Netduino.  

The project should now deploy correctly to your Netduino.


## Getting Started

The getting started lab code and the [complete Maker Den Lab Guide](https://github.com/MakerDen/IoT-Maker-Den-NETMF/blob/master/MakerDen/Lab%20Code/IoT%20Maker%20Den%20v2.0.pdf)
can be found in the Lab Code folder in the Maker Den Project.

Be sure to read the Lab Guide appendix section to understand:-

1. Trouble Shooting
2. Software Requirements
3. Lab Parts
4. Initial Hardware Setup

## What is the Internet of Things Solution Accelerator?

The IoT Framework for the .NET Micro Framework provides a pluggable foundation to support sensors, actuators, data serialisation, communications, and command and control. 


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


## Implementing a Sensor


### Inheriting from SensorBase

Your sensor class must inherit from SensorBase.  


    using Glovebox.MicroFramework.Base;

    namespace Glovebox.Netduino.Sensors {

        public class SensorLight : SensorBase {
        }
    }


### Implement the Abstract Class

Next right mouse click on SensorBase to Implement the Abstract Class.


    using Glovebox.MicroFramework.Base;

    namespace Glovebox.Netduino.Sensors {
        public class SensorLight : SensorBase {

            protected override void Measure(double[] value) {
                throw new NotImplementedException();
            }

            protected override string GeoLocation() {
                throw new NotImplementedException();
            }

            public override double Current {
                get { throw new NotImplementedException(); }
            }

            protected override void SensorCleanup() {
                throw new NotImplementedException();
            }
        }
    }

### Initialise the Sensor Base Constructor

Add a Sensor Constructor and initialise the SensorBase Base constructor.  

The SensorBase base constructor requires

1. **Sensor Type** - arbitrary/sensible type for the sensor.  The value is published alongside the sensor reading to provide some type information.
2. **Sensor Unit** - arbitrary/sensible measurement unit for the sensor.  Example p for percentage, n for numeric etc.  The unit is published alongside the sensor reading to provide some unit information.
3. **Sample Rate in Milliseconds** - how often to take a sensor reading.
4. **Name** - This is a unique name that you can use to identify a sensor from the command and control service.


        public SensorLight(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
            : base("light", "p", ValuesPerSample.One, SampleRateMilliseconds, name) {

            ldrAnalogPin = new AnalogInput(pin, -1);

            // Call StartMeasuring() after sensor initialisation
            StartMeasuring();
        }


### Implement the sensor logic

Implement the abstract methods and properties for the Sensor class.


    using Glovebox.MicroFramework.Base;
    using Microsoft.SPOT.Hardware;
    using System;

    namespace Glovebox.Netduino.Sensors {
        public class SensorLdr : SensorBase {

            protected AnalogInput ldrAnalogPin;

            /// <summary>
            /// Light Dependent Resistor Sensor Class
            /// </summary>
            /// <param name="pin">Analog Pin</param>
            /// <param name="SampleRateMilliseconds">How often to sample the sensor on milliseconds</param>
            /// <param name="name">Sensor target name for command and control</param>
            public SensorLdr(Cpu.AnalogChannel pin, int SampleRateMilliseconds, string name)
                : base("light", "p", ValuesPerSample.One, SampleRateMilliseconds, name) {

                ldrAnalogPin = new AnalogInput(pin, -1);

                // Call StartMeasuring() after sensor initialisation
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


### Using your newly created sensor

    using Glovebox.Netduino.Sensors;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;
    using System.Threading;

    namespace MakerDen {
        public class Program : MakerBaseIoT {

            public static void Main() {

                using (SensorLdr ldr = new SensorLdr(AnalogChannels.ANALOG_PIN_A0, 1000, "ldr01")) {

                    // the event handlers are implemented in the MakerBaseIoT subclass
                    ldr.OnBeforeMeasurement += OnBeforeMeasure;
                    ldr.OnAfterMeasurement += OnMeasureCompleted;

                    // Thread sleep the main thread forever.  
                    // Your newly created sensor runs on its own thread and in this case wakes up 1000 milliseconds
                    Thread.Sleep(Timeout.Infinite);
                }
            }
        }
    }

Or

    using Glovebox.Netduino.Sensors;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;
    using System.Threading;

    namespace MakerDen {
        public class Program : MakerBaseIoT {

            public static void Main() {

                using (SensorLdr ldr = new SensorLdr(AnalogChannels.ANALOG_PIN_A0, -1, "ldr01")) {

                    while (true) {
                        if (ldr.Current < 60) {
                            // do something...
                        }
                        else {
                            // do something...
                        }
                        // good practice not to put your netduino in to a hard loop, so add a thread sleep
                        Thread.Sleep(100);
                    }
                }
            }
        }
    }

## Creating an Actuator

The following example creates a relay switch with on/off command and control support.

    using Glovebox.MicroFramework.Base;

    namespace Glovebox.Netduino.Actuators {
        public class MyRelay : ActuatorBase {
        }
    }

### Implement the Abstract Class

Next right mouse click on ActuatorBase to Implement the Abstract Class.

    using Glovebox.MicroFramework.Base;

    namespace Glovebox.Netduino.Actuators {
        public class MyRelay : ActuatorBase {

            protected override void ActuatorCleanup() {
                throw new System.NotImplementedException();
            }

            public override void Action(MicroFramework.IoT.IotAction action) {
                throw new System.NotImplementedException();
            }
        }
    }


## Implement the class constructor

        public MyRelay(Cpu.Pin pin, string name)
            : base(name, ActuatorType.Relay) {
            relay = new OutputPort(pin, false);
        }

## Implement the Actuator Cleanup

        protected override void ActuatorCleanup() {
            relay.Dispose();
        }

## Implement Command and Control

See the Lab Guide Appendix for information on sending a command via MQTT.

        public override void Action(Glovebox.MicroFramework.IoT.IotAction action) {
            switch (action.cmd) {
                case "on":
                    relay.Write(true);
                    break;
                case "off":
                    relay.Write(false);
                    break;
            }
        }

## The Completed Relay Class

    using Glovebox.MicroFramework.Base;
    using Microsoft.SPOT.Hardware;

    namespace Glovebox.Netduino.Actuators {
        public class MyRelay : ActuatorBase {

            public OutputPort relay;

            public MyRelay(Cpu.Pin pin, string name)
                : base(name, ActuatorType.Relay) {
                relay = new OutputPort(pin, false);
            }

            protected override void ActuatorCleanup() {
                relay.Dispose();
            }

            public override void Action(Glovebox.MicroFramework.IoT.IotAction action) {
                switch (action.cmd) {
                    case "on":
                        relay.Write(true);
                        break;
                    case "off":
                        relay.Write(false);
                        break;
                }
            }
        }
    }


## Coder Friendly Relay Class

    using Glovebox.MicroFramework.Base;
    using Microsoft.SPOT.Hardware;

    namespace Glovebox.Netduino.Actuators {
        public class MyRelay : ActuatorBase {

            public enum Actions { On, Off }

            public OutputPort relay;

            public MyRelay(Cpu.Pin pin, string name)
                : base(name, ActuatorType.Relay) {
                relay = new OutputPort(pin, false);
            }

            protected override void ActuatorCleanup() {
                relay.Dispose();
            }

            public override void Action(Glovebox.MicroFramework.IoT.IotAction action) {
                switch (action.cmd) {
                    case "on":
                        TurnOn();
                        break;
                    case "off":
                        TurnOff();
                        break;
                }
            }

            public void Action(Actions action) {
                switch (action) {
                    case Actions.On:
                        TurnOn();
                        break;
                    case Actions.Off:
                        TurnOff();
                        break;
                    default:
                        break;
                }
            }

            public void TurnOn() {
                relay.Write(true);
            }

            public void TurnOff() {
                relay.Write(false);
            }
        }
    }


## Using the newly created Relay Actuator

This example uses the Light Dependent Resistor Sensor to determine the light levels.  Depending on the light level, the Relay will be turned on or off.  The relay could be controlling a light.

    using Glovebox.Netduino.Actuators;
    using Glovebox.Netduino.Sensors;
    using SecretLabs.NETMF.Hardware.NetduinoPlus;
    using System.Threading;

    namespace MakerDen {
        public class Program : MakerBaseIoT {

            public static void Main() {

                using (Sensorldr ldr = new Sensorldr(AnalogChannels.ANALOG_PIN_A0, -1, "ldr01")) 
                using (MyRelay relay = new MyRelay(Pins.GPIO_PIN_D0, "myRelay01"))
            

                    while (true) {
                        if (ldr.Current < 60) {
                            relay.Action(MyRelay.Actions.On);
                        }
                        else {
                            relay.Action(MyRelay.Actions.Off);
                        }
                        // good practice not to put your netduino in to a hard loop, so add a thread sleep
                        Thread.Sleep(100);
                    }
                }
            }
        }
    }


# NeoPixels and Netduino

The IoT Solution Accelerator also includes a comprehensive library to drive NeoPixels.  There is support for multiple NeoPixel grids daisy chained together such as the [NeoPixel NeoMatrix 8x8 - 64 RGB LED](http://www.adafruit.com/products/1487), plus rings and strips.

Below is an example of driving a Happy Birthday Message to three daisy chained 8x8 NeoPixel Grids.  

    using Coatsy.Netduino.NeoPixel;
    using Coatsy.Netduino.NeoPixel.Grid;
    using System.Threading;

    namespace MakerDen {
        public class Program {

            public static void Main() {
                // main code marker

                // Create new instance of a grid named grid01, there are three 8x8 grids daisy chained
                NeoPixelGrid8x8 grid = new NeoPixelGrid8x8("grid01", 3);

                while (true) {

                    // Scroll Happy Birthday in from the right
                    grid.ScrollStringInFromRight("Happy Birthday", grid.PaletteHotLowPower, 10);
                    Thread.Sleep(500);

                    // write the letter K on the first panel
                    grid.DrawLetter('K', Pixel.ColourLowPower.HotRed, 0);
                    // write the letter I on the second panel
                    grid.DrawLetter('I', Pixel.ColourLowPower.HotGreen, 1);
                    // write the letter T on the third panel
                    grid.DrawLetter('T', Pixel.ColourLowPower.HotBlue, 2);


                    // now create a randomised pattern by shuntting alternate rows right and left
                    for (int i = 0; i < grid.Columns * grid.Panels / 2; i++) {
                        grid.ColumnRollRight(0);
                        grid.ColumnRollRight(0);

                        grid.ColumnRollLeft(2);
                        grid.ColumnRollLeft(2);

                        grid.ColumnRollRight(4);
                        grid.ColumnRollRight(4);

                        grid.ColumnRollLeft(6);
                        grid.ColumnRollLeft(6);

                        // now draw the frame buffer to the neopixel grids
                        grid.FrameDraw();
                        Thread.Sleep(10);
                    }
                    Thread.Sleep(1000);

                    // now draw the letters KIT in different colours from the PaletteHotLowPower palette
                    foreach (var c in grid.PaletteHotLowPower) {
                        grid.DrawLetter('K', c, 0);
                        grid.DrawLetter('I', c, 1);
                        grid.DrawLetter('T', c, 2);

                        // now draw the frame buffer to the neopixel grids
                        grid.FrameDraw();
                        Thread.Sleep(500);
                    }

                    // clear the frame buffer
                    grid.FrameClear();

                    // scroll in the heart symbols in from the left
                    grid.ScrollSymbolInFromLeft(new NeoPixelGrid8x8.Symbols[] { NeoPixelGrid8x8.Symbols.Heart, NeoPixelGrid8x8.Symbols.Heart, NeoPixelGrid8x8.Symbols.Heart }, Pixel.ColourLowPower.HotRed, 10);
                    Thread.Sleep(500);

                    // now redraw the heart symbols in different colours
                    grid.DrawSymbol(NeoPixelGrid8x8.Symbols.Heart, Pixel.ColourLowPower.HotBlue, 0);
                    grid.DrawSymbol(NeoPixelGrid8x8.Symbols.Heart, Pixel.ColourLowPower.HotBlue, 1);
                    grid.DrawSymbol(NeoPixelGrid8x8.Symbols.Heart, Pixel.ColourLowPower.HotBlue, 2);
                    grid.FrameDraw();
                    Thread.Sleep(500);

                    grid.DrawSymbol(NeoPixelGrid8x8.Symbols.Heart, Pixel.ColourLowPower.HotGreen, 0);
                    grid.DrawSymbol(NeoPixelGrid8x8.Symbols.Heart, Pixel.ColourLowPower.HotGreen, 1);
                    grid.DrawSymbol(NeoPixelGrid8x8.Symbols.Heart, Pixel.ColourLowPower.HotGreen, 2);
                    grid.FrameDraw();
                    Thread.Sleep(500);

                }
            }
        }
    }













