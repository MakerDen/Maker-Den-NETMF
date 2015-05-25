using System;
using System.IO;
using System.Text;

namespace Glovebox.IoT.Json {
    public class ByteArrayBuilder : IDisposable {

        MemoryStream ms = new MemoryStream();
        UTF8Encoding encoder = new UTF8Encoding();

        protected void Clear() {
            ms.SetLength(0);
        }

        protected void Append(string data) {
            ms.Write(encoder.GetBytes(data), 0, data.Length);
        }

        protected void Append(byte[] data) {
            ms.Write(data, 0, data.Length);
        }

        protected void Append(int number) {
            string data = number.ToString();
            ms.Write(encoder.GetBytes(data), 0, data.Length);
        }

        protected void Append(uint number) {
            string data = number.ToString();
            ms.Write(encoder.GetBytes(data), 0, data.Length);
        }

        protected void Append(float number) {
            string data = number.ToString();
            ms.Write(encoder.GetBytes(data), 0, data.Length);
        }

        protected void Append(double number) {
            string data = number.ToString();
            ms.Write(encoder.GetBytes(data), 0, data.Length);
        }

        public virtual byte[] ToArray() {
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        void IDisposable.Dispose() {
            ms.Dispose();
        }
    }
}
