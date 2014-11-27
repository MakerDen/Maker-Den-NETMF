using System;
using Microsoft.SPOT;
using Glovebox.MicroFramework.Base;
using Glovebox.MicroFramework.IoT;

namespace Glovebox.MicroFramework.Sensors {
    public class Challenge : SensorBase {
        IotBase iotItem;

        public Challenge(IotBase iotItem, string userDefinedType)
            : base(SensorType.Challenge, ValuesPerSample.One, 60000, "challenge", userDefinedType ) {
                this.iotItem = iotItem;

            StartMeasuring();
        }

        protected override void Measure(double[] value) {
            //value[0] = IotList.ActionCountByName(iotName);
            value[0] = iotItem.TotalActionCount;
        }

        protected override string GeoLocation() {
            return string.Empty;
        }

        protected override double Current {
            get { return TotalActionCount; }
        }

        protected override void SensorCleanup() {
        }
    }
}
