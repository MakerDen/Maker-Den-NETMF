using System;
using System.Collections;
using Microsoft.SPOT;

namespace Glovebox.Netduino.Sensors
{
    public class SensorMultipleBase:IDisposable
    {
        public delegate uint SensorEventHandler(object sender, EventArgs e);
        public event SensorEventHandler OnAfterMeasurement;
        public event SensorEventHandler OnBeforeMeasurement;


        public ArrayList Sensors { get; set; }
        //public ArrayList Sensors { get; set; }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
