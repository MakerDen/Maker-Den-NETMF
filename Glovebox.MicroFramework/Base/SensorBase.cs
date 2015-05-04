using Glovebox.MicroFramework.IoT;
using Glovebox.MicroFramework.Json;
using Microsoft.SPOT;
using System;
using System.Threading;

namespace Glovebox.MicroFramework.Base {
    public abstract class SensorBase : IotBase {
        public enum Actions { Start, Stop, Measure };

        protected abstract void Measure(double[] value);
        protected abstract string GeoLocation();
        public abstract double Current { get; }
        protected abstract void SensorCleanup();

        // note order of sensorType amd SensorType must match.  There is no enum.parse in micro framework
        //static readonly string[] _sensorType = new string[] { "light", "mem", "memfh", "sound", "temp", "error", "tmphumd", "misc", "diag", "chl" };
        //static readonly string[] _sensorUnit = new string[] { "p", "b", "b", "d", "c", "n", "cp", "n", "g", "n" };
        //public enum SensorType { Light, Memory, MemFezHydra, Sound, Temperature, Error, TempAndHumidity, Miscellaneous, Diagnostic, Challenge }
        public enum ValuesPerSample { One = 1, Two = 2, Three = 3, Four = 4, Five = 5 };

        protected JSONWriter jw = new JSONWriter();

        public delegate uint SensorEventHandler(object sender, EventArgs e);
        public event SensorEventHandler OnAfterMeasurement;
        public event SensorEventHandler OnBeforeMeasurement;


        public class SensorIdEventArgs : EventArgs {
            public readonly uint id;
            public SensorIdEventArgs(uint id) {
                this.id = id;
            }
        }

        public class SensorItemEventArgs : EventArgs {
            readonly byte[] jsonBytes;
            public readonly string topic;
            public readonly string type;
            public readonly double[] value;
            public readonly int msgId;

            public SensorItemEventArgs(byte[] jsonBytes, string topic, string type, double[] value, int msgId) {
                this.jsonBytes = jsonBytes;
                this.topic = topic;
                this.type = type;
                this.value = value;
                this.msgId = msgId;
            }

            public byte[] ToJson() {
                return jsonBytes;
            }

            public override string ToString() {
                var j = jsonBytes;
                char[] Output = new char[j.Length];
                for (int Counter = 0; Counter < j.Length; ++Counter) { Output[Counter] = (char)j[Counter]; }
                return new string(Output);
            }
        }

        private Thread SensorThread;

        private static uint sensorErrorCount;
        public uint SensorErrorCount {
            get { return sensorErrorCount; }
        }

        private readonly string topicNamespace = ConfigurationManager.MqttNameSpace;
        private readonly string topic;


        protected string Geo { get; set; }
        private int sampleRateMilliseconds;


        protected static int msgId;

        public SensorBase(string sensorType, string sensorUnit, ValuesPerSample valuesPerSensor, int SampleRateMilliseconds, string name)
            : base(name == null ? sensorType : name, sensorType) {

            topic = topicNamespace + deviceName + "/" + type;
            this.sampleRateMilliseconds = SampleRateMilliseconds;
            this.ThisIotType = IotType.Sensor;

            SensorThread = new Thread(new ThreadStart(this.MeasureThread));
            SensorThread.Priority = ThreadPriority.Highest;
        }

        public void StartMeasuring() {
            if (sampleRateMilliseconds > 0) {
                SensorThread.Start();
            }
        }

        private void MeasureThread() {
            while (true) {
                try {
                    DoMeasure();
                }
                catch (Exception ex) {
                    Debug.Print(ex.Message);
                    sensorErrorCount++;
                }
                Thread.Sleep(sampleRateMilliseconds);
            }
        }


        protected void DoMeasure() {
            ////lock (threadSync) {
            TotalSensorMeasurements++;
            BeforeMeasurement(new SensorIdEventArgs(id));
            //Measure(value);
            Geo = GeoLocation();
            //sensorErrorCount = AfterMeasurement(new SensorItemEventArgs(ToJson(), topic, type, value, msgId));
            ////}
        }


        private uint AfterMeasurement(EventArgs e) {
            if (OnAfterMeasurement != null) {
                return OnAfterMeasurement(this, e);
            }
            return 0;
        }

        private void BeforeMeasurement(EventArgs e) {
            if (OnBeforeMeasurement != null) { OnBeforeMeasurement(this, e); }
        }



        public override void Action(IotAction action) {
            double sampleRate;
            if (action.cmd == null) { return; }
            switch (action.cmd) {
                case "measure":
                    DoMeasure();
                    break;
                case "start":
                    Action(Actions.Start);
                    break;
                case "stop":
                    Action(Actions.Stop);
                    break;
                case "rate":
                    //test for numeric sensor sample rate
                    if (action.parameters == null) { return; }
                    if (double.TryParse(action.parameters, out sampleRate)) {
                        Action((int)sampleRate);
                    }
                    break;
            }
        }

        public void Action(Actions action) {
            switch (action) {
                case Actions.Start:
                    if (SensorThread.ThreadState == ThreadState.Running) { return; }
                    if (sampleRateMilliseconds > 0) { SensorThread.Resume(); }
                    break;
                case Actions.Stop:
                    SensorThread.Suspend();
                    break;
                case Actions.Measure:
                    if (SensorThread.ThreadState == ThreadState.Running) { return; }
                    DoMeasure();
                    break;
                default:
                    break;
            }
        }

        public void Action(int sampleRateMilliseconds) {
            if (sampleRateMilliseconds > 0) {
                this.sampleRateMilliseconds = sampleRateMilliseconds;
            }
        }

        protected override void CleanUp() {
            SensorCleanup();
            if (SensorThread != null) {
                SensorThread.Abort();
            }
        }
    }
}



