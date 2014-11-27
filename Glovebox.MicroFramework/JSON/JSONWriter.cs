using System;
using Microsoft.SPOT;

namespace Glovebox.MicroFramework.Json {
    public class JSONWriter : ByteArrayBuilder {
        bool firstProperty = true;

        public void Begin() {
            firstProperty = true;
            Clear();
            Append("{");
        }


        public void End() {
            Append("}");    
        }

        public override byte[] ToArray() {
            return base.ToArray();
        }

        public override string ToString() {
            return BytesToString();
        }

        public string BytesToString() {
            byte[] input = base.ToArray();
            char[] output = new char[input.Length];            

            for (int Counter = 0; Counter < input.Length; ++Counter) {
                output[Counter] = (char)input[Counter];
            }
            return new string(output);
        }



        public void AddProperty(string name, string value) {
            AddPropertyName(name);
            AddPropertyValue(value);
        }

        public void AddProperty(string name, float value) {
            AddPropertyName(name);
            AddPropertyValue(value);
        }

        public void AddProperty(string name, DateTime value) {
            AddPropertyName(name);
            AddPropertyValue(value);
        }

        public void AddProperty(string name, double[] value, string format)
        {
            AddPropertyName(name);
            AddPropertyValue(value, format);
        }

        public void AddProperty(string name, string[] value) {
            AddPropertyName(name);
            AddPropertyValue(value);
        }

        private void AddPropertyName(string name) {
            if (firstProperty) { firstProperty = false; }
            else { Append(","); }
            Append("\"");
            Append(name);
            Append("\":");
        }

        private void AddPropertyValue(string value) {
            Append("\"");
            Append(value);
            Append("\"");
        }

        public void AddPropertyValue(DateTime value) {
            Append("\"");
            Append(value.ToString("s"));
            Append("\"");
        }

        //"Val":[14.00,14.00,14.00]
        private void AddPropertyValue(double[] value, string format)
        {
            Append("[");
            for (int i = 0; i < value.Length; i++)
            {
                Append(value[i].ToString(format));
                if (i < value.Length - 1)
                {
                    Append(",");
                }
            }
            Append("]");
        }

        private void AddPropertyValue(string[] value) {
            bool addComma = false;
            Append("[");
            for (int i = 0; i < value.Length; i++) {
                if (value[i] == null) { continue; }
                if (addComma) { Append(","); addComma = false; }
                Append("\"" + value[i] + "\"");
                addComma = true;
            }
            Append("]");
        }


        public void AddPropertyValue(int value) {
            Append(value);
        }

        public void AddPropertyValue(uint value) {
            Append(value);
        }

        private void AddPropertyValue(float value) {
            Append(value);
        }

    }
}
