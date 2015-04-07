using System;
using Microsoft.SPOT;

//#acknowledgement: http://blog.codeblack.nl/post/Netduino-Getting-Started-with-steppermotors.aspx
namespace Glovebox.Netduino.Actuators

{
    public enum StepType
    {
        /// <summary>
        /// Single Coil activation.
        /// </summary>
        Single,

        /// <summary>
        /// Double Coil activation.
        /// </summary>
        /// <remarks>Higher torque than Single</remarks>
        Double,

        /// <summary>
        /// Alternating between Single and Double.
        /// </summary>
        /// <remarks>Twice the resolution, but half the speed</remarks>
        Interleave,
    }
}
