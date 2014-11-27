using System;
using System.Collections;
using Microsoft.SPOT;

namespace Coatsy.Netduino.NeoPixel
{
    public enum CommandType
    {
        Parent,
        Light1Pixel,
        LightMultiPixel,
        Set1Pixel,
        SetMultiPixel,
        Rotate,
        AllOff,
        AllOn,
        Spin,
        SpinOnBackground,
        AlternateColours,
        ColourBlocks,
        Wait,
        Repeat,
    }

    public class Command
    {
        /// <summary>
        /// The type of command this is
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// ms delay to execute after this command
        /// </summary>
        public int PauseAfter { get; set; }
        /// <summary>
        /// ms delay between steps in this command (e.g. between steps in rotation)
        /// </summary>
        public int StepTime { get; set; }
        /// <summary>
        /// Commands to execute as part of this command
        /// Allows for composite commands
        /// </summary>
        public ArrayList Commands { get; set; }
        /// <summary>
        /// Number of times to repeat the command
        /// </summary>
        public int Repetitions { get; set; }
        /// <summary>
        /// ms to pause between repetitions of the command
        /// </summary>
        public int PauseBetween { get; set; }
        /// <summary>
        /// colours to use for multi-colour sets
        /// </summary>
        public PixelColour[] ColourSet { get; set; }
        /// <summary>
        /// main colour for command
        /// </summary>
        public PixelColour PrimaryColour { get; set; }
        /// <summary>
        /// background colour
        /// </summary>
        public PixelColour SecondaryColour { get; set; }
        /// <summary>
        /// single pixel position to be set
        /// </summary>
        public int StartingPosition { get; set; }
        /// <summary>
        /// Pixel positions for which the primary colour should be set
        /// </summary>
        public int[] PixelPositions { get; set; }
        /// <summary>
        /// The number of positions to rotate by (-ve is anti-clockwise)
        /// </summary>
        public int RotateIncrement { get; set; }
        /// <summary>
        /// Number of times a spin should happen
        /// </summary>
        public int Cycles { get; set; }
    }
}
