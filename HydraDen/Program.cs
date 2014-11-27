using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Glovebox.MicroFramework;

namespace test {
    public partial class Program {

        SensorLight light;
        SensorTemp temp;
        SensorError error;

        void ProgramStarted() {

            Message("Initialising");

            Initialise("fez", true);

            light = new SensorLight(1000);
            temp = new SensorTemp(20000);

            light.OnBeforeMeasurement += OnBeforeMeasure;
            light.OnAfterMeasurement += OnMeasureCompleted;

            temp.OnBeforeMeasurement += OnBeforeMeasure;
            temp.OnAfterMeasurement += OnMeasureCompleted;

            Message(string.Empty);
        }     
    }
}
