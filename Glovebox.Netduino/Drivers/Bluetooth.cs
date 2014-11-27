using System;
using Microsoft.SPOT;
//using System.IO.Ports;
using System.Threading;
using System.Text;
using System.IO.Ports;
using System.IO;
using Glovebox.MicroFramework;


namespace Glovebox.Netduino.Drivers {

    public class Bluetooth : IDisposable {

        static UTF8Encoding encoding = new UTF8Encoding();
        byte[] buffer = new byte[1024];

        public class DataRecievedEventArgs : EventArgs {
            public readonly string Data;
            public readonly bool Valid;
            public readonly int Crc;


            public DataRecievedEventArgs(string data, bool valid, int crc) {
                Data = data;
                Valid = valid;
                Crc = crc;
            }

        }


        public event DataReceivedEventHandler OnDataReceived;

        SerialPort bt;
        object channelLock = new object();

        public delegate void DataReceivedEventHandler(object sender, EventArgs e);


        public Bluetooth() {

            bt = new SerialPort(Serial.COM1, 115200, Parity.Odd, 8, StopBits.One);
           // bt = new SerialPort(Serial.COM1, 9600, Parity.None, 8, StopBits.One);

            bt.DataReceived += bt_DataReceived;
            bt.Open();

            //Thread.Sleep(0);

            //Thread.Sleep(1000);
            //bt.Write(StringToByteArray("AT"), 0, 2);

            //Thread.Sleep(1000);

            //bt.Write(StringToByteArray("AT+VERSION"), 0, 10);

            //Thread.Sleep(1000);

            //bt.Write(StringToByteArray("AT+PO"), 0, 5);



            //Thread.Sleep(1000);

            //bt.Write(StringToByteArray("AT+BAUD8"), 0, 8);

            // for (int i = 0; i < 10000000; i++) {
            //     Thread.Sleep(250);
            //     bt.Write(StringToByteArray("hello world"), 0, 11);
            // }

        }



        void bt_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            ushort passedCrc;
            ushort crcno;
            bool valid = false;
            int bytesRead;

            Thread.Sleep(100);

            lock (channelLock) {

                //byte[] buffer = new byte[bt.BytesToRead];

          

                if (bt.BytesToRead == 0) { return; }
                bytesRead = bt.BytesToRead > 1024 ? 1024 : bt.BytesToRead;

                bt.Read(buffer, 0, bytesRead);

                Debug.Print(ToString(buffer));

                if (bytesRead < 2) {
                    passedCrc = 1;
                    crcno = 0;
                }
                else {

                    crcno = CRC.CRC16(buffer, 2, bytesRead);
                    passedCrc = BitConverter.ToUInt16(buffer, 0);
                }

                if (crcno != passedCrc) {
                    valid = false;
                }
                else {
                    valid = true;
                }

                OnChanged(new DataRecievedEventArgs(BytesToString(buffer), valid, crcno));

            }
        }


        /// <summary>
        /// Send data over bluetooth serial
        /// </summary>
        /// <param name="dataToSend"></param>
        /// <returns></returns>
        public bool SendData(string dataToSend) {
            lock (channelLock) {

                byte[] data = StringToByteArray(dataToSend);

                ushort crcno = CRC.CRC16(data, 0, data.Length);
                Byte[] crcbytes = BitConverter.GetBytes(crcno);

                bt.Write(crcbytes, 0, crcbytes.Length);
                bt.Write(data, 0, dataToSend.Length);

                Thread.Sleep(200);
            }
            return true;
        }

        /// <summary>
        /// Send data over bluetooth serial
        /// </summary>
        /// <param name="dataToSend"></param>
        /// <returns></returns>
        public bool SendData(byte[] data) {
            lock (channelLock) {

                ushort crcno = CRC.CRC16(data, 0, data.Length);
                Byte[] crcbytes = BitConverter.GetBytes(crcno);

                bt.Write(crcbytes, 0, crcbytes.Length);
                bt.Write(data, 0, data.Length);

                Thread.Sleep(200);
            }
            return true;
        }


        /// <summary>
        /// convert string to byte array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string str) {
            return encoding.GetBytes(str);
        }

        public string ToString(byte[] Input) {
            char[] Output = new char[Input.Length];
            for (int Counter = 0; Counter < Input.Length; ++Counter) {
                Output[Counter] = (char)Input[Counter];
            }
            return new string(Output);
        }


        /// <summary>
        /// First two bytes are CRC16 bytes, rest is data
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] Input) {
            if (Input.Length < 2) return string.Empty;

            char[] Output = new char[Input.Length - 2];
            for (int Counter = 2; Counter < Input.Length; ++Counter) {
                Output[Counter - 2] = (char)Input[Counter];
            }
            return new string(Output);
        }


        protected virtual void OnChanged(EventArgs e) {
            if (OnDataReceived != null)
                OnDataReceived(this, e);
        }


        void IDisposable.Dispose() {
            bt.Close();
            bt.Dispose();
        }
    }
}
