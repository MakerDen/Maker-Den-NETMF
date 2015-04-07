//---------------------------------------------------------------------------
//<copyright file="DhtSensor.cs">
//
// Copyright 2011 Stanislav "CW" Simicek
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
//</copyright>
//---------------------------------------------------------------------------
namespace Glovebox.Netduino.Drivers
{
  using System;
  using System.Runtime.CompilerServices;
  using System.Threading;
  using Microsoft.SPOT;
  using Microsoft.SPOT.Hardware;

  /// <summary>
  /// Encapsulates the common functionality of DHT sensors.
  /// </summary>
  public abstract class DhtSensor : IDisposable
  {
    private bool disposed;

    private InterruptPort portIn;
    private TristatePort portOut;

    private float rhum; // Relative Humidity
    private float temp; // Temperature

    private long data;
    private long bitMask;
    private long lastTicks;
    private byte[] bytes = new byte[4];

    private AutoResetEvent dataReceived = new AutoResetEvent(false);

    // Instantiated via derived class
    protected DhtSensor(Cpu.Pin pin1, Cpu.Pin pin2, Port.ResistorMode pullUp)///PullUpResistor pullUp)
    {
      var resistorMode = (Port.ResistorMode)pullUp;

      portIn = new InterruptPort(pin2, false, resistorMode, Port.InterruptMode.InterruptEdgeLow);
      portIn.OnInterrupt += new NativeEventHandler(portIn_OnInterrupt);
      portIn.DisableInterrupt();  // Enabled automatically in the previous call

      portOut = new TristatePort(pin1, true, false, resistorMode);

      if(!CheckPins())
      {
        throw new InvalidOperationException("DHT sensor pins are not connected together.");
      }
    }

    /// <summary>
    /// Deletes an instance of the <see cref="DhtSensor"/> class.
    /// </summary>
    ~DhtSensor()
    {
      Dispose(false);
    }

    /// <summary>
    /// Releases resources used by this <see cref="DhtSensor"/> object.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the resources associated with the <see cref="DhtSensor"/> object.
    /// </summary>
    /// <param name="disposing">
    /// <b>true</b> to release both managed and unmanaged resources;
    /// <b>false</b> to release only unmanaged resources.
    /// </param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    protected void Dispose(bool disposing)
    {
      if(!disposed)
      {
        try
        {
          portIn.Dispose();
          portOut.Dispose();
        }
        finally
        {
          disposed = true;
        }
      }
    }

    /// <summary>
    /// Gets the measured temperature value.
    /// </summary>
    public float Temperature
    {
      get
      {
        return temp;
      }
      protected set
      {
        temp = value;
      }
    }

    /// <summary>
    /// Gets the measured relative humidity value.
    /// </summary>
    public float Humidity
    {
      get
      {
        return rhum;
      }
      protected set
      {
        rhum = value;
      }
    }

    /// <summary>
    /// Gets the start delay, in milliseconds.
    /// </summary>
    protected abstract int StartDelay
    {
      get;
    }

    /// <summary>
    /// Converts raw sensor data.
    /// </summary>
    /// <param name="data">The sensor raw data, excluding the checksum.</param>
    /// <remarks>
    /// If the checksum verification fails, this method is not called.
    /// </remarks>
    protected abstract void Convert(byte[] data);

    /// <summary>
    /// Retrieves measured data from the sensor.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the operation succeeds and the data is valid, otherwise <c>false</c>.
    /// </returns>
    public bool Read()
    {
                  if (disposed)
            {
                throw new ObjectDisposedException();
            }
            // The 'bitMask' also serves as edge counter: data bit edges plus
            // extra ones at the beginning of the communication (presence pulse).
            bitMask = 1L << 41;

            data = 0;
            // lastTicks = 0; // This is not really needed, we measure duration
            // between edges and the first three values are ignored anyway.

            // Initiate communication
            portOut.Active = true;
            portOut.Write(false);       // Pull bus low
            Thread.Sleep(StartDelay);
            portIn.EnableInterrupt();   // Turn on the receiver
            portOut.Active = false;     // Release bus

            bool dataValid = false;

            // Now the interrupt handler is getting called on each falling edge.
            // The communication takes up to 5 ms, but the interrupt handler managed
            // code takes longer to execute than is the duration of sensor pulse
            // (interrupts are queued), so we must wait for the last one to finish
            // and signal completion. 20 ms should be enough, 50 ms is safe.
            if (dataReceived.WaitOne(50, false))
            {
                // TODO: Use two short-s ?
                bytes[0] = (byte)((data >> 32) & 0xFF);
                bytes[1] = (byte)((data >> 24) & 0xFF);
                bytes[2] = (byte)((data >> 16) & 0xFF);
                bytes[3] = (byte)((data >> 8) & 0xFF);

                byte checksum = (byte)(bytes[0] + bytes[1] + bytes[2] + bytes[3]);
                if (checksum == (byte)(data & 0xFF))
                {
                    dataValid = true;
                    if (bytes[0] == 0)
                    {
                        portIn.DisableInterrupt();
                    }
                    else
                    {
                        Convert(bytes);
                    }
                }
                else
                {
                    Debug.Print("DHT sensor data has invalid checksum.");
                }
            }
            else
            {
                portIn.DisableInterrupt();  // Stop receiver
                Debug.Print("DHT sensor data timeout.");  // TODO: TimeoutException?
            }
            return dataValid;
    }

    // If the received data has invalid checksum too often, adjust this value
    // based on the actual sensor pulse durations. It may be a little bit
    // tricky, because the resolution of system clock is only 21.33 µs.
    private const long BitThreshold = 1050;

    private void portIn_OnInterrupt(uint pin, uint state, DateTime time)
    {
      var ticks = time.Ticks;
      if((ticks - lastTicks) > BitThreshold)
      {
        // If the time between edges exceeds threshold, it is bit '1'
        data |= bitMask;
      }
      if((bitMask >>= 1) == 0)
      {
        // Received the last edge, stop and signal completion
        portIn.DisableInterrupt();
        dataReceived.Set();
      }
      lastTicks = ticks;
    }

    // Returns true if the ports are wired together, otherwise false.
    private bool CheckPins()
    {
        return true;

        //bug, apparently with Netduino Firmware 4.3. check is now disabled and initial reads will also fail
      Debug.Assert(portIn != null, "Input port should not be null.");
      Debug.Assert(portOut != null, "Output port should not be null.");
      Debug.Assert(!portOut.Active, "Output port should not be active.");
      portOut.Active = true;  // Switch to output
      portOut.Write(false);
      Thread.Sleep(50);
      var expectedFalse = portIn.Read();
      //portOut.Active = false; // Switch to input
      Thread.Sleep(50);
      var expectedTrue = portIn.Read();
      return (expectedTrue && !expectedFalse);

    }
  }
}
