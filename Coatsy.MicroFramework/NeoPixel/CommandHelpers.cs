using System;
using Microsoft.SPOT;
using Coatsy.Netduino;
using System.Collections;

namespace Coatsy.Netduino.NeoPixel
{
    public static class CommandHelpers
    {
        #region Field Names
        private const string COMMAND_TYPE = "ct";
        private const string PAUSE_AFTER = "pa";
        private const string STEP_TIME = "st";
        private const string COMMANDS = "c";
        private const string REPETITIONS = "r";
        private const string PAUSE_BETWEEN = "pb";
        private const string COLOUR_SET = "cs";
        private const string PRIMARY_COLOUR = "pc";
        private const string SECONDARY_COLOUR = "sc";
        private const string STARTING_POSITION = "sp";
        private const string PIXEL_POSITIONS = "pp";
        private const string ROTATE_INCREMENT = "ri";
        private const string CYCLES = "cy";
        #endregion
        public static Command CommandFromJson(string jsonString)
        {
            var parser = new Json.NETMF.JsonSerializer();
            var result = (Hashtable)parser.Deserialize(jsonString);
            return getCommand(result);
        }

        private static Command getCommand(Hashtable result)
        {
            Command answer = new Command();
            answer.CommandType = GetCommandType(result, COMMAND_TYPE, CommandType.Parent);
            answer.PauseAfter = GetInt(result, PAUSE_AFTER);
            answer.StepTime = GetInt(result, STEP_TIME);
            answer.Commands = new ArrayList();
            var commands = (ArrayList)result[getKey(result, COMMANDS)];
            if (commands != null)
            {
                foreach (Hashtable c in commands)
                {
                    answer.Commands.Add(getCommand(c));
                }
            }
            answer.Repetitions = GetInt(result, REPETITIONS);
            answer.PauseBetween = GetInt(result, PAUSE_BETWEEN);
            answer.ColourSet = GetPixelColourList(result, COLOUR_SET);
            answer.PrimaryColour = GetPixelColour(result, PRIMARY_COLOUR);
            answer.SecondaryColour = GetPixelColour(result, SECONDARY_COLOUR);
            answer.StartingPosition = GetInt(result, STARTING_POSITION);
            answer.PixelPositions = GetIntArray(result, PIXEL_POSITIONS);
            answer.RotateIncrement = GetInt(result, ROTATE_INCREMENT);
            answer.Cycles = GetInt(result, CYCLES);

            return answer;
        }

        private static int[] GetIntArray(Hashtable result, string propertyName)
        {
            int[] answer = new int[0];
            var key = getKey(result, propertyName);
            if (key != null)
            {
                var ints = (ArrayList)result[key];
                if (ints != null)
                {
                    answer = new int[ints.Count];
                    int pos = 0;
                    foreach (long i in ints)
                    {
                        answer[pos] = (int)i;
                        pos++;
                    }
                }
            }

            return answer;

        }

        private static PixelColour GetPixelColour(Hashtable result, string propertyName)
        {
            int pcInt = GetInt(result, propertyName, 0);
            return (PixelColour)pcInt;
        }

        private static PixelColour[] GetPixelColourList(Hashtable result, string propertyName)
        {
            PixelColour[] answer = new PixelColour[0];
            var key = getKey(result, propertyName);
            if (key != null)
            {
                var pixelColours = (ArrayList)result[key];
                if (pixelColours != null)
                {
                    answer = new PixelColour[pixelColours.Count];
                    int pos = 0;
                    foreach (long c in pixelColours)
                    {
                        answer[pos] = (PixelColour)(int)c;
                        pos++;
                    }
                }
            }

            return answer;
        }

        private static CommandType GetCommandType(Hashtable result, string propertyName, CommandType defaultValue)
        {
            int ctInt = GetInt(result, propertyName, 0);
            return (CommandType)ctInt;
        }

        private static int GetInt(Hashtable result, string propertyName, int defaultValue = 0)
        {
            int theInt;
            var key = getKey(result, propertyName);
            if (key == null)
                theInt = defaultValue;
            else
            {
                theInt = (int)(long)result[key];
            }
            return theInt;
        }

        private static object getKey(Hashtable result, string p)
        {
            object answer = null;
            foreach (var key in result.Keys)
            {
                if (key.ToString().ToLower() == p.ToLower())
                {
                    answer = key;
                    break;
                }
            }

            return answer;
        }


    }
}
