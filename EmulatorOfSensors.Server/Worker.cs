using System;
using System.Net.Sockets;
using System.Threading;
using EmulatorOfSensors.Helpers;

namespace EmulatorOfSensors.Server
{
    public delegate void ReceiveSensorValueHandler(object sender, int numberOfSensor, int sensorValue);

    public class Worker
    {
        private readonly TcpClient _client;

        public event ReceiveSensorValueHandler ReceiveSensorValue;

        public Worker(TcpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void WaitData()
        {
            var stream = _client.GetStream();

            try
            {
                var bytes = new byte[8];
                while (stream.CanRead)
                {
                    if (stream.Read(bytes, 0, bytes.Length) != 0)
                    {
                        var value = bytes.DeserializeToInt();
                        OnReceiveSensorValue(value[0], value[1]);
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
            finally
            {
                stream.Close();
            }
        }

        protected virtual void OnReceiveSensorValue(int numberofsensor, int sensorvalue)
        {
            ReceiveSensorValue?.Invoke(this, numberofsensor, sensorvalue);
        }
    }
}
