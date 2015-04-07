using System;
using Microsoft.SPOT;

namespace Codeblack.Micro.GettingStarted.StepperMotor
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
