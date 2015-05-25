#if MF_FRAMEWORK_VERSION_V4_3

/*
 * Author: Arron Chapman
 * Author Contact: arron@arronchapman.com
 * Author Website: http://arronchapman.com
 * 
 * Project:VariableLabs.PowerManagment
 * Project URL: TBA
 *
 * Creation Date: November 26th, 2012
 *
 * License: Apache 2.0
 * Copyright 2011 Arron Chapman

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 * 
 * Note: For the purposes of license attribution, keeping the above information in the file is adequate.
 * If you make any modifacations to this file and you think others might benefit from them, please email me at arron@arronchapman.com
 * 
 */

/*Changelog
 * Date		    | Editor		| Reason
 * 11/26/2012   | Arron Chapman | Initial Draft
 * 
 */

/*Changelog
 * Date		    | Editor		| Reason
 * 12/02/20124   | Dave Glover | Added On Off enum
 * 
 */


/*ToDo List
 * Implement board power states.
 * 
 */

using Microsoft.SPOT.Hardware;
using System;

//namespace VariableLabs.PowerManagment
namespace Glovebox.IoT {


    //http://forums.netduino.com/index.php?/topic/10668-adding-to-cpupin-enumeration/
    //http://www.netduino.com/netduinoplus2/schematic.pdf

    public enum Peripheral {
        PowerHeaders = 18, //decimal 18, PB2
        Ethernet = 47, //decimal 47, PC15
        SDCard = 17,   //decimal 17, PB1
        PowerLED = 45, //decimal 45, PC13
    }

    public enum Switch {
        On, Off
    }

    public class PowerManagment {
        private static OutputPort[] peripheralPorts = new OutputPort[4];
        private static bool portState;

        public static void SetPeripheralState(Peripheral device, Switch state) {
            portState = state == Switch.On ? true : false;

            switch (device) {
                case Peripheral.Ethernet:
                    if (peripheralPorts[0] == null)
                        peripheralPorts[0] = new OutputPort((Cpu.Pin)Peripheral.Ethernet, !portState);
                    else
                        peripheralPorts[0].Write(!portState);
                    break;
                case Peripheral.PowerLED:
                    if (peripheralPorts[1] == null)
                        peripheralPorts[1] = new OutputPort((Cpu.Pin)Peripheral.PowerLED, portState);
                    else
                        peripheralPorts[1].Write(portState);
                    break;
                case Peripheral.SDCard:
                    if (peripheralPorts[2] == null)
                        peripheralPorts[2] = new OutputPort((Cpu.Pin)Peripheral.SDCard, portState);
                    else
                        peripheralPorts[2].Write(portState);
                    break;
                case Peripheral.PowerHeaders:
                    if (peripheralPorts[3] == null)
                        peripheralPorts[3] = new OutputPort((Cpu.Pin)Peripheral.PowerHeaders, portState);
                    else
                        peripheralPorts[3].Write(portState);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("device");
            }
        }
    }
}

#endif