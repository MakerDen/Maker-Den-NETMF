using System;
using Microsoft.SPOT;
using System.Threading;

namespace test {
    class MakerMain : MakerBase {

        public MakerMain() {

            Initialise("hydra", false);

            using (SensorLight light = new SensorLight(500))
            using (SensorTemp temp = new SensorTemp(5000))
          //  using (SensorMemory mem = new SensorMemory(5000))
            using (SensorError error = new SensorError(5000)) {
                light.OnBeforeMeasurement += OnBeforeMeasure;
                light.OnAfterMeasurement += OnMeasureCompleted;

                temp.OnBeforeMeasurement += OnBeforeMeasure;
                temp.OnAfterMeasurement += OnMeasureCompleted;

                //mem.OnBeforeMeasurement += OnBeforeMeasure;
                //mem.OnAfterMeasurement += OnMeasureCompleted;

                error.OnBeforeMeasurement += OnBeforeMeasure;
                error.OnAfterMeasurement += OnMeasureCompleted;

                Thread.Sleep(Timeout.Infinite);

            }
        }
    }
}
