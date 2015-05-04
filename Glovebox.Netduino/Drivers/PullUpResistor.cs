using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace Glovebox.Netduino.Drivers
{
    public enum PullUpResistor
    {
        /// <summary>
        /// A value that represents an external pull-up resistor.
        /// </summary>
        External = Port.ResistorMode.Disabled,

        /// <summary>
        /// A value that represents an internal pull-up resistor.
        /// </summary>
        Internal = Port.ResistorMode.PullUp
    }
}
