namespace Glovebox.IoT.Command {
    public class IotAction {
        public string cmd; // eg on, off, blink, play
        public string item;  //eg rgb01
        public string subItem; // red
        public string parameters;  // eg rate duration, json string for neopixel
    }
}
