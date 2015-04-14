using Glovebox.Gadgeteer.Actuators;
using Glovebox.Gadgeteer.Sensors;
using HydraDen;
using System.Threading;

namespace HyrdaDen {
    public partial class Program {

        void ProgramStarted() {

            MakerBaseIoT.StartNetworkServices(ethernetENC28, "gdgr", true);

            using (MakerBaseIoT.relay = new Relay(relayX1, "relay01"))
            using (MakerBaseIoT.led = new Led7R(led7R, "led7r01"))
            using (SensorTemp temp = new SensorTemp(tempHumidity, 2000, "temp01"))
            using (SensorLight light = new SensorLight(lightSense, 1000, "light01")) {

                light.OnBeforeMeasurement += MakerBaseIoT.OnBeforeMeasure;
                light.OnAfterMeasurement += MakerBaseIoT.OnMeasureCompleted;

                temp.OnBeforeMeasurement += MakerBaseIoT.OnBeforeMeasure;
                temp.OnAfterMeasurement += MakerBaseIoT.OnMeasureCompleted;

                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
