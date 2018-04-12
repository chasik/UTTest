using System;
using System.Net.Sockets;
using System.Threading;

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
                        OnReceiveSensorValue(BitConverter.ToInt32(bytes, 0), BitConverter.ToInt32(bytes, 4));
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
