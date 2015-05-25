using Glovebox.IoT.Base;
using System;

namespace Glovebox.IoT.Command {
    public static class IotActionManager {
        const uint maxIoT = 64;
        private static uint _actionErrors;
        public static uint TotalActions { get; private set; }

        public static uint ActionErrorCount {
            get { return _actionErrors; }
        }

        private static IotBase[] IotItems = new IotBase[maxIoT];

        public static uint AddItem(IotBase iotItem) {
            uint id = GetFreeId();
            if (id == uint.MaxValue) {
                throw new Exception("maximum Iots allocation reached - increase value of MaxIot in IotList.cs");
            }
            IotItems[id] = iotItem;
            return id;
        }

        public static void RemoveItem(uint id) {
            IotItems[id] = null;
        }

        private static uint GetFreeId() {
            for (uint i = 0; i < maxIoT; i++) {
                if (IotItems[i] == null) {
                    return i;
                }
            }
            return uint.MaxValue;
        }

        public static uint ActionCountByName(string name) {
            string n = name.ToLower();
            for (int i = 0; i < maxIoT; i++) {
                if (IotItems[i] == null || IotItems[i].Name.Length == 0) { continue; }
                if (IotItems[i].Name == n) {
                    return IotItems[i].TotalActionCount;
                }
            }
            return 0;
        }

        public static string[] Action(IotAction action) {
            if (action.cmd == "hello") { return GetAllItemName(); }
            if (action.cmd == null || action.item == null) { return null; }
            ActionByName(action);
            return null;
        }

        private static string[] GetAllItemName() {
            string[] result = new string[maxIoT];
            for (int i = 0; i < maxIoT; i++) {
                result[i] = IotItems[i] == null || IotItems[i].Name.Length == 0 ? null : IotItems[i].Name;
            }
            return result;
        }

        static void ActionByName(IotAction action) {
            for (int i = 0; i < maxIoT; i++) {
                if (IotItems[i] == null || IotItems[i].Name.Length == 0) { continue; }
                if (IotItems[i].Name == action.item) {
                    try {
                        TotalActions++;
                        IotItems[i].IncrementActionCount();
                        IotItems[i].Action(action);
                    }
                    catch { _actionErrors++; }
                    break;
                }
            }
        }

        public static IotBase FindByName(string name) {
            string n = name.ToLower();
            for (int i = 0; i < maxIoT; i++) {
                if (IotItems[i] == null) { continue; }
                if (IotItems[i].Name.Length != 0 && IotItems[i].Name == n) {
                    return IotItems[i];
                }
            }
            return null;
        }

        public static IotBase FindById(uint id) {
            if (id < maxIoT) {
                return IotItems[id];
            }
            return null;
        }
    }
}
