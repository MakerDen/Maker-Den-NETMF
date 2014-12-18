Documentation in progress.


### IoT-Maker-Den-NETMF

[Complete Maker Den Lab Guide](https://github.com/MakerDen/IoT-Maker-Den-NETMF/blob/master/MakerDen/Lab%20Code/IoT%20Maker%20Den%20v2.0.pdf)

The Maker Den IoT Framework provides a pluggable foundation to support sensors, actuators, data serialisation, communications, and command and control. 


![Alt text](https://github.com/MakerDen/IoT-Maker-Den-NETMF/blob/master/MakerDen/Lab%20Code/Maker%20Den%20IoT%20Framework.jpg)


## Extensible/pluggable framework supporting

Sensors

* Physical: Light, Sound, Temperature (onewire), Servo
* Virtual: Memory Usage, Diagnostics
* Sensor data serialised to a JSON schema

Actuators

* RGB, RGB PWM, Piezo, Relay, NeoPixel (strips, rings and grids)

Command and Control

* Control relays, start neo pixels etc via comms layer

Communications
* Pluggable – currently implemented on MQTT (MQTT Server running on Azure)

Supported and Tested
* Netduino 2 Plus and Gadgeteer
* Supports Visual Studio 2012 and 2013
 

## Programming Models

![Alt text](https://github.com/MakerDen/IoT-Maker-Den-NETMF/blob/master/MakerDen/Lab%20Code/DevModel1.JPG)

![Alt text](https://github.com/MakerDen/IoT-Maker-Den-NETMF/blob/master/MakerDen/Lab%20Code/DevModel2.JPG)
