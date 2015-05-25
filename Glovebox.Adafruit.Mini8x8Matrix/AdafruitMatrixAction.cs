
using Glovebox.IoT.Base;
using Glovebox.IoT.IoT;
using System.Collections;

namespace Glovebox.Adafruit.Mini8x8Matrix
{

    public delegate void DoCycle();

    // queue manager for NeoPixel
    public abstract class AdafruitMatrixAction
        : ActuatorBase {

        protected abstract void DoAction(IotAction action);

        public DoCycle[] cycles;
        Queue actionQueue = new Queue();

        public AdafruitMatrixAction(string name)
            : base(name, "matrix") {
        }

        public IotAction GetNextAction() {
            if (actionQueue.Count > 0) {
                return (IotAction)(actionQueue.Dequeue());
            }
            else { return null; }
        }

        public void ClearActionQueue() {
            actionQueue.Clear();
        }

        protected override void ActuatorCleanup() {
        }

        protected void ExecuteCycle(DoCycle doCycle) {
            try {
                doCycle();

                var a = GetNextAction();
                while (a != null) {
                    DoAction(a);
                    a = GetNextAction();
                }
            }
            catch { ActuatorErrorCount++; }
        }

        public override void Action(IotAction action) {
            // cap the queue to prevent flooding attack

            if (actionQueue.Count > 50) { return; }
            actionQueue.Enqueue((object)action);
        }
    }
}
