
using System;
using System.Collections;
using System.Threading;
using Glovebox.MicroFramework.Base;
using Glovebox.MicroFramework.IoT;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
namespace Glovebox.Netduino.Actuators
{
    public class PiezoSL : ActuatorBase
    {

        static SecretLabs.NETMF.Hardware.PWM _piezo;
        enum BeatCount { Sixteenth, eigth, quarter, half, zero, one, two, three, four, five, six, seven, eight, nine };
        public enum Actions { BeepOk, BeepAlert, BeepStartup }
    
        static Queue toneQueue = new Queue();
        string[] validOctaves = new string[8];
        Thread playToneThread;

        /// <summary>
        /// Piezo speaker driver and notes and playback manager
        /// </summary>
        /// <param name="pin">From the SecretLabs.NETMF.Hardware.NetduinoPlus.PWMChannels namespace</param>
        /// <param name="name">Unique identifying name for command and control</param>
        public PiezoSL(Cpu.Pin pin, string name)
            : base(name, "piezoSL")
        {
            for (int i = 1; i <= 8; i++)
            {
                validOctaves[i - 1] = i.ToString();
            }

            _piezo = new SecretLabs.NETMF.Hardware.PWM(pin);//,0,0, PWM.ScaleFactor.Microseconds, false);
            //_piezo.DutyCycle = 0;
            //_piezo.Start();

        }

        public void QueuePlay()
        {

            if (playToneThread != null && playToneThread.IsAlive) { return; }
            PlayTone();

            playToneThread = new Thread(PlayTone);
            playToneThread.Start();
        }
        public void PlayWait()
        {
            QueuePlay();
            Wait();
        }
        public void Wait() {
            while (IsActive())
            {
                Thread.Sleep(50);
            }
        
        }
        public bool IsActive()
        {
            if (playToneThread == null)
                return false;
            return playToneThread.IsAlive;
        }
        private bool ValidOctave(string octave){
            foreach(var s in validOctaves){
            if(s == octave)
                return true;
            }
        
            return false;
        }
        /// <summary>
        /// Queue a music script.  
        /// </summary>
        /// <param name="script">
        /// A space delimited series of notes and commands. 
        /// Notes: c,c#,d,d#,e,f,f#,g,g#,a,a#,b.    
        /// Commands: + increase octave, - decrease octace, * double duration, / half duration, # 1/3 duration, $ 1/4 duration ^ 2/3 duration.
        /// Example C C G G A A * G / F F E E D D * C / G G F F E E * D / G G F F E E * D / C C G G A A * G / F F E E D D * C
        /// </param>
        ///<param name="startingOctave">Starting Octave between 0 and 8 inclusive.  Middle C Octave is 4</param>
        ///<param name="beatCount">Starting beats per note.  Beats: 0 to 9, t=thirtysecondth, s=sixteenth, e=eighth, q=quarter beat, h=half beats</param>
        /// <param name="beatsPerMintue">Tempo in beats per minute</param>
        public void QueueScript(string script, byte startingOctave, char beatCount, int beatsPerMintue)
        {
            byte _octave = startingOctave;
            ushort _thirtySecondthBeatCount = BeatCountInThirtySecondths(beatCount);
            
            if (startingOctave > 8) { _octave = 8; }
            else if (startingOctave < 0) { _octave = 0; }
            else { _octave = startingOctave; }

            foreach (string item in script.ToLower().Split(' '))
            {
                var subitem = item.ToCharArray();

                switch (item)
                {
                    case " ":
                        break;//blank
                    case "!":
                        //full duration
                        _thirtySecondthBeatCount = BeatCountInThirtySecondths(beatCount);
                        break;
                    case "/":
                        // half duration
                        //_thirtySecondthBeatCount = BeatCountInThirtySecondths(beatCount);
                        _thirtySecondthBeatCount /= 2;
                        break;
                    case "#":
                        // 3/4 duration
                        //_thirtySecondthBeatCount = BeatCountInThirtySecondths(beatCount);
                        _thirtySecondthBeatCount =(ushort)(( _thirtySecondthBeatCount / 4)*3);
                        break;
                    case "$":
                        // quarter duration
                        //_thirtySecondthBeatCount = BeatCountInThirtySecondths(beatCount);
                        _thirtySecondthBeatCount /= 4;
                        break;
                    case "@":
                        // 1/3 duration
                        //_thirtySecondthBeatCount = BeatCountInThirtySecondths(beatCount);
                        _thirtySecondthBeatCount = (ushort)((_thirtySecondthBeatCount / 3) * 2);
                        break;

                    case "^":
                        // 2/3 duration
                        //_thirtySecondthBeatCount = BeatCountInThirtySecondths(beatCount);
                        _thirtySecondthBeatCount = (ushort)((_thirtySecondthBeatCount / 3) * 2);
                        break;
                    case "*":
                        // double duration
                        //_thirtySecondthBeatCount = BeatCountInThirtySecondths(beatCount);
                        _thirtySecondthBeatCount *= 2;
                        break;
                    case "+":
                        // increase octave
                        if (_octave < 8) { _octave++; }
                        break;
                    case "-":
                        // reduce octave
                        if (_octave > 0) { _octave--; }
                        break;
                    default:
                        //note
                        var note = item.Trim().ToLower();
                        if (item.Length > 1 && item.Substring(item.Length - 1, 1) != "#")
                        {
                            note = item;
                            var noteSignature = item.Substring(item.Length - 1, 1);
                            if(ValidOctave(noteSignature)){
                                    _octave = byte.Parse(noteSignature);
                            }
                                note = item.Substring(0, item.Length - 1);
                        }

                        AddNoteToQueue(note, _thirtySecondthBeatCount, beatsPerMintue, _octave);
                        break;
                }
            }
        }


        private static ushort BeatCountInThirtySecondths(char beatCountChar)
        {
            ushort _thirtySecondthBeatCount = 32;

            switch (beatCountChar)
            {
                case 't': //thirthy secondth
                    _thirtySecondthBeatCount = 1;
                    break;
                case 's': //sixteenth
                    _thirtySecondthBeatCount = 2;
                    break;
                case 'e': //eighth
                    _thirtySecondthBeatCount = 4;
                    break;
                case 'q': //quarter
                    _thirtySecondthBeatCount = 8;
                    break;
                case 'h': //half
                    _thirtySecondthBeatCount = 16;
                    break;
                default:
                    if (beatCountChar >= '1' && beatCountChar <= '9')
                    {
                        _thirtySecondthBeatCount = (ushort)(ushort.Parse(beatCountChar.ToString()) * 32);
                    }
                    break;
            }
            return _thirtySecondthBeatCount;
        }

        /// <summary>
        /// Queue a series of comma demilimited note definitions
        /// </summary>
        /// <param name="score">note definition format: obn.  o=octave[0..9], b=beats per note, n=note[C, D, E, F, G, A, B.  Suffix: #=sharp, b=flat. eg c#] </param>
        /// <param name="beatsPerMintue">Tempo in beats per minute</param>
        public void QueueScore(string score, int beatsPerMintue)
        {
            char octaveChar, beatCountChar;
            byte octave = 0;
            ushort _thirtySecondthBeatCount = 0;

            string[] notes = score.ToLower().Split(',');

            foreach (string thisNote in notes)
            {
                string note = thisNote.Trim();

                if (note.Length < 3) { continue; }

                octaveChar = note.Substring(0, 1)[0];
                beatCountChar = note.Substring(1, 1)[0];

                if (octaveChar >= '0' && octaveChar <= '9') { octave = byte.Parse(note.Substring(0, 1)); }

                _thirtySecondthBeatCount = BeatCountInThirtySecondths(beatCountChar);

                AddNoteToQueue(note.Substring(2), _thirtySecondthBeatCount, beatsPerMintue, octave);
            }
        }


        /// <summary>
        /// Queue a note
        /// </summary>
        /// <param name="note">C, D, E, F, G, A, B.  Suffix: #=sharp, b=flat. eg c#</param>
        /// <param name="octave">Octave between 0 and 8 inclusive.  Middle C Octave is 4</param>
        ///<param name="beatCount">Beats per note. Beats: 0 to 9, t=thirty secondth, s=sixteenth, e=eighth, q=quarter beat, h=half beats</param>
        /// <param name="beatsPerMinute">Beats per minute</param>
        public void QueueNote(string note, byte octave, char beatCount, int beatsPerMinute)
        {
            byte octaveValue = 4;
            ushort _thirtySecondthBeatCount = BeatCountInThirtySecondths(beatCount);

            if (octave >= '0' && octave <= '9') { octaveValue = byte.Parse(octave.ToString()); }

            AddNoteToQueue(note, _thirtySecondthBeatCount, beatsPerMinute, octave);
        }

        public void QueueRest(int restInMilliseconds)
        {
            QueueTone(0, 1000, 0);
        }

        public void BeepAlert()
        {
            QueueScore("6qd", 120);
            QueuePlay();
        }

        public void BeepOK()
        {
            QueueNote("c", 6, 'h', 90);
            QueueNote("c", 6, 'h', 90);
            QueuePlay();
        }

        public void BeebStartup()
        {
            QueueNote("c#", 4, '1', 90);
            QueueTone(404, 50, 50);
            QueueTone(261, 50, 50);
            QueueTone(505, 50, 0);
            QueuePlay();
        }


        internal void AddNoteToQueue(string note, int thirtySecondthBeatCount, int beatsPerMinute, byte octave)
        {
            int beatTimeInMilliseconds = 60000 / beatsPerMinute; // 60,000 milliseconds per minute
            int pauseTimeInMilliseconds = (int)(beatTimeInMilliseconds * 0.1);
            int durationTimeInMilliseconds = ((beatTimeInMilliseconds /16) *  thirtySecondthBeatCount) -pauseTimeInMilliseconds;//) / 16) - pauseTimeInMilliseconds;
            var time = ((beatTimeInMilliseconds ) - pauseTimeInMilliseconds);

            if (durationTimeInMilliseconds <= 0) { durationTimeInMilliseconds = 1; }

            uint frequency = (uint)(CalculateFrequency(octave, note) );  // add 0.5f to round up

            QueueTone(frequency, durationTimeInMilliseconds, pauseTimeInMilliseconds);
        }

        /// <summary>
        /// Queue a tone
        /// </summary>
        /// <param name="frequency">In hertz</param>
        /// <param name="durationTimeInMilliseconds">how long to play note in milliseconds</param>
        /// <param name="pauseTimeInMilliseconds">pause after note is played in milliseconds</param>
        public void QueueTone(uint frequency, int durationTimeInMilliseconds, int pauseTimeInMilliseconds)
        {
            toneQueue.Enqueue((object)(new ToneDefinition(frequency, durationTimeInMilliseconds, pauseTimeInMilliseconds)));
        }


        public void QueueClear()
        {
            if (playToneThread != null && playToneThread.IsAlive) { playToneThread.Abort(); }
            toneQueue.Clear();
            _piezo.SetPulse(0, 0);
            /*_piezo.Period = 0;

            _piezo.Duration = 0;*/
            playToneThread = null;
        }


        static void PlayTone()
        {
            uint myPeriod;
            ToneDefinition bd;

            while (toneQueue.Count > 0)
            {
                bd = (ToneDefinition)(toneQueue.Dequeue());
                if (bd.Frequency != 0)
                {
                                    ///1000000
                    myPeriod = (uint)((1000000) / bd.Frequency); //261 Hz is middle c

                    //_piezo.Period = myPeriod;
                    //_piezo.Duration = myPeriod / 2;  //Duration is the proportion of the period that the wave is high
                    _piezo.SetPulse(myPeriod, myPeriod / 2);
                }
                else {
                    _piezo.SetPulse(0,0); 
                }
                
                Thread.Sleep(bd.DurationTimeInMilliseconds);
                // _piezo.DutyCycle = 0;
                _piezo.SetPulse(0, 0); 
                Thread.Sleep(bd.PauseTimeInMilliseconds);
            }
        }

        public float CalculateFrequency(int octave, string note)
        {
            // acknowledgements
            // http://www.techlib.com/reference/musical_note_frequencies.htm
            // http://forums.netduino.com/index.php?/topic/831-music-with-a-piezo-speaker/

            string noteLC = note.ToLower();
            string[] notes = "c,c#,d,d#,e,f,f#,g,g#,a,a#,b".Split(',');

            // loop through each note until we find the index of the one we want        
            for (int n = 0; n < notes.Length; n++)
            {
                if (notes[n] == noteLC // frequency found for major and sharp notes
                    || (note.Length > 1 && noteLC[1] == 'b' && notes[n + 1][0] == noteLC[0]))
                { // or flat of next note

                    // Multiply initial note by 2 to the power (n / 12) to get correct frequency, 
                    //  (where n is the number of notes above the first note). 
                    //  Then mutiply that value by 2 to go up each octave
                    var result =( (16.35f * (float)System.Math.Pow(2, (float)n / 12))
                        * (float)System.Math.Pow(2, octave)) + 0.5f;//0.5f is round up(??)
                    return result;
                }
            }
            // throw new ArgumentException("No frequency found for note : " + note, note);
            return 0f;
        }

        protected override void ActuatorCleanup()
        {
            if (_piezo == null) { return; }
            //_piezo.Stop();

            _piezo.Dispose();
        }

        /// <summary>
        /// Play a command
        /// </summary>
        /// <param name="action">
        /// Action object format.
        /// Action.cmd: beepok, beepalert, beepstartup, or play
        /// If cmd = play, Action.parameters required.  String format: BeatsPerMiniute, note definition, note definition, note definition... eg  60, 3qc#,31c
        /// BeatsPerMiniute: Numeric.
        /// Note Definition format: octave, length, note.
        /// Octave: Between 0 and 8 inclusive, Middle C Octave is 4.
        /// Length: Beats per note. Beats: 0 to 9, t=thirty secondth, s=sixteenth, e=eighth, q=quarter beat, h=half beats.
        /// Note: C, D, E, F, G, A, B.  Suffix: #=sharp, b=flat. eg c#.
        /// </param>
        public override void Action(IotAction action)
        {
            switch (action.cmd)
            {
                case "beepok":
                    Action(Actions.BeepOk);
                    break;
                case "beepalert":
                    Action(Actions.BeepAlert);
                    break;
                case "beepstartup":
                    Action(Actions.BeepStartup);
                    break;
                case "play":
                    DecodePlayAction(action.parameters);
                    break;
            }
        }

        public void Action(Actions action)
        {
            switch (action)
            {
                case Actions.BeepOk:
                    BeepOK();
                    break;
                case Actions.BeepAlert:
                    BeepAlert();
                    break;
                case Actions.BeepStartup:
                    BeebStartup();
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Decode a command string sent to this actuator
        /// </summary>
        /// <param name="command">string format: beats/miniute, note, note, note... eg  60, 3qc#,31c</param>
        public void DecodePlayAction(string command)
        {
            double beatsPerMinute = 60;
            string score = string.Empty;

            var index = command.IndexOf(',');
            if (index < 0 || index + 1 == command.Length) { return; }

            if (!double.TryParse(command.Substring(0, index), out beatsPerMinute)) { return; }
            score = command.Substring(index + 1).Trim().ToLower();

            if (score != string.Empty)
            {
                QueueScore(score, (int)beatsPerMinute);
                QueuePlay();
            }
        }
    }


}