using System;
using Microsoft.SPOT;
using Coatsy.Netduino;
using System.Collections;
using netduino.helpers.Helpers;

namespace Coatsy.Netduino.NeoPixel
{
    public static class CommandHelpers_noLib
    {
        public static string JsonDecode(string encodedString)
        {
            return "";
        }

        public static Command CommandFromJson(string jsonString)
        {
            //var parser = new Json.NETMF.JsonSerializer();
            //var result = (Hashtable)parser.Deserialize(jsonString);
            var parser = new JSONParser();
            parser.AutoCasting = true;
            var result = parser.Parse(jsonString);
            return getCommand(result, parser);
        }

        private static Command getCommand(Hashtable result, JSONParser parser)
        {
            CommandType commandType;
            int commandTypeInt;
            int pauseAfter;
            int stepTime;
            ArrayList commands;
            int repetitions;
            int pauseBetween;
            ArrayList colourSet;
            int primaryColourInt;
            int secondaryColourInt;
            int startingPosition;
            ArrayList pixelPositions;
            int rotateIncrement;
            int cycles;

            Command answer = new Command();

            if (parser.Find("CommandType", result, out commandTypeInt))
            {
                commandType = (CommandType)commandTypeInt;
            }
            else
            {
                commandType = (CommandType)0;
            }

            if (parser.Find("PauseAfter", result, out pauseAfter))
                answer.PauseAfter = pauseAfter;

            if (parser.Find("StepTime", result, out stepTime))
                answer.StepTime = stepTime;

            answer.Commands = new ArrayList();
            if(parser.Find("Commands", result, out commands))
            {
                foreach (Hashtable c in commands)
                {
                    answer.Commands.Add(getCommand(c, parser));
                }

            }

            if (parser.Find("Repetitions", result, out repetitions))
                answer.Repetitions = repetitions;

            if (parser.Find("PauseBetween", result, out pauseBetween))
                answer.PauseBetween = pauseBetween;

            if(parser.Find("ColourSet", result, out colourSet))
            {
                answer.ColourSet = new PixelColour[colourSet.Count];
                int pos = 0;
                foreach (int i in colourSet)
                {
                    answer.ColourSet[pos] = (PixelColour)(int)colourSet[pos];
                    pos++;

                }
            }

            if (parser.Find("PrimaryColour", result, out primaryColourInt))
                answer.PrimaryColour = (PixelColour)primaryColourInt;

            if (parser.Find("SecondaryColour", result, out secondaryColourInt))
                answer.SecondaryColour = (PixelColour)secondaryColourInt;

            if (parser.Find("StartingPosition", result, out startingPosition))
                answer.StartingPosition = startingPosition;

            if (parser.Find("PixelPositions", result, out pixelPositions))
            {
                answer.PixelPositions = new int[pixelPositions.Count];
                int pos = 0;
                foreach (int i in pixelPositions)
                {
                    answer.PixelPositions[pos] = (int)pixelPositions[pos];
                    pos++;

                }
            }

            if (parser.Find("RotateIncrement", result, out rotateIncrement))
                answer.RotateIncrement = rotateIncrement;

            if (parser.Find("Cycles", result, out cycles))
                answer.Cycles = cycles;


            return answer;
        }

        //private static int[] GetIntArray(Hashtable result, string propertyName)
        //{
        //    int[] answer = new int[0];
        //    var key = getKey(result, propertyName);
        //    if (key != null)
        //    {
        //        var ints = (ArrayList)result[key];
        //        if (ints != null)
        //        {
        //            answer = new int[ints.Count];
        //            int pos = 0;
        //            foreach (long i in ints)
        //            {
        //                answer[pos] = (int)i;
        //                pos++;
        //            }
        //        }
        //    }

        //    return answer;

        //}

        //private static PixelColour GetPixelColour(Hashtable result, string propertyName)
        //{
        //    int pcInt = GetInt(result, propertyName, 0);
        //    return (PixelColour)pcInt;
        //}

        //private static PixelColour[] GetPixelColourList(Hashtable result, string propertyName)
        //{
        //    PixelColour[] answer = new PixelColour[0];
        //    var key = getKey(result, propertyName);
        //    if (key != null)
        //    {
        //        var pixelColours = (ArrayList)result[key];
        //        if (pixelColours != null)
        //        {
        //            answer = new PixelColour[pixelColours.Count];
        //            int pos = 0;
        //            foreach (long c in pixelColours)
        //            {
        //                answer[pos] = (PixelColour)(int)c;
        //                pos++;
        //            }
        //        }
        //    }

        //    return answer;
        //}

        //private static CommandType GetCommandType(Hashtable result, string propertyName, CommandType defaultValue)
        //{
        //    int ctInt = GetInt(result, propertyName, 0);
        //    return (CommandType)ctInt;
        //}

        //private static int GetInt(Hashtable result, string propertyName, int defaultValue = 0)
        //{
        //    int theInt;
        //    var key = getKey(result, propertyName);
        //    if (key == null)
        //        theInt = defaultValue;
        //    else
        //    {
        //        theInt = (int)(long)result[key];
        //    }
        //    return theInt;
        //}

        //private static object getKey(Hashtable result, string p)
        //{
        //    object answer = null;
        //    foreach (var key in result.Keys)
        //    {
        //        if (key.ToString() == p)
        //        {
        //            answer = key;
        //            break;
        //        }
        //    }

        //    return answer;
        //}


    }
}
