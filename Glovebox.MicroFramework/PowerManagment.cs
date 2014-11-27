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

/*ToDo List
 * Implement board power states.
 * 
 */

using System;
using Microsoft.SPOT.Hardware;

//namespace VariableLabs.PowerManagment
namespace Glovebox.MicroFramework
{
    public enum Peripheral
    {
		PowerHeaders = 0x12, //decimal 18, PB2
        Ethernet = 0x2F, //decimal 47, PC15
        SDCard = 0x11,   //decimal 17, PB1
        PowerLED = 0x2D, //decimal 45, PC13
    }

    public class PowerManagment
    {
        private static OutputPort[] peripheralPorts = new OutputPort[4];

        public static void SetPeripheralState(Peripheral device, bool state)
        {
            switch (device)
            {
                case Peripheral.Ethernet:
                    if (peripheralPorts[0] == null)
                        peripheralPorts[0] = new OutputPort((Cpu.Pin)Peripheral.Ethernet, !state);
                    else
                        peripheralPorts[0].Write(!state);
                    break;
                case Peripheral.PowerLED:
                    if (peripheralPorts[1] == null)
                        peripheralPorts[1] = new OutputPort((Cpu.Pin)Peripheral.PowerLED, !state);
                    else
                        peripheralPorts[1].Write(!state);
                    break;
                case Peripheral.SDCard:
                    if (peripheralPorts[2] == null)
                        peripheralPorts[2] = new OutputPort((Cpu.Pin)Peripheral.SDCard, !state);
                    else
                        peripheralPorts[2].Write(!state);
                    break;
                case Peripheral.PowerHeaders:
                    if(peripheralPorts[3] == null)
                        peripheralPorts[3] = new OutputPort((Cpu.Pin)Peripheral.PowerHeaders, state);
                    else
                        peripheralPorts[3].Write(state);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("device");
            }
        }
    }
}
