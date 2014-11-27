using Glovebox.MicroFramework.Base;
using Glovebox.MicroFramework.IoT;
using System.Collections;

namespace Coatsy.Netduino.NeoPixel
{

    // queue manager for NeoPixel
    public class NeoPixelAction : ActuatorBase {

        Queue actionQueue = new Queue();

        public NeoPixelAction(string name)
            : base(name, ActuatorType.NeoPixel) {
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

        public override void Action(IotAction action) {
            // cap the queue to prevent flooding attack
            if (actionQueue.Count > 100) { return; }
            actionQueue.Enqueue((object)action);
        }
    }
}
