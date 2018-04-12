using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using EmulatorOfSensors.Client.Interfaces;

namespace EmulatorOfSensors.Client.Sensors
{
    public delegate void SensorFailedHandler(object sender, Exception e);
    public delegate void SensorEventHandler(object sender, int sensorId, int sensorValue);

    public class Sensor : ISensor
    {
        public event SensorEventHandler SensorBufferedEvent;
        public event SensorEventHandler SensorSendEvent;
        public event SensorFailedHandler SensorFailed;

        private TcpClient _tcpClient;
        private NetworkStream _stream;

        private readonly Queue<int> _valuesBuffer;

        private readonly IPEndPoint _endPoint;
        private readonly int _sensorId;
        private readonly byte[] _sensorIdAsBytesArray;

        private readonly Random _random;
        private readonly Timer _reconnectTimer;


        public Sensor()
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
            _valuesBuffer = new Queue<int>();

            _reconnectTimer = new Timer(ReConnectToServer, null, 0, 5000);
        }

        public Sensor(IPEndPoint endPoint, int sensorId) : this()
        {
            _endPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
            _sensorId = sensorId;
            _sensorIdAsBytesArray = BitConverter.GetBytes(_sensorId);
        }

        private void ReConnectToServer(object state)
        {
            try
            {
                _tcpClient = new TcpClient(_endPoint.Address.ToString(), _endPoint.Port);
                _stream = _tcpClient.GetStream();

                _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                OnSensorFailed(ex);
            } 
        }

        public ISensor StartGenerate(int minValue = int.MinValue, int maxValue = int.MaxValue, int minTimeInterval = 1, int maxTimeInterval = 10000)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        var sensorValue = _random.Next(minValue, maxValue);

                        _valuesBuffer.Enqueue(sensorValue);

                        try
                        {
                            if (_stream != null && _stream.CanWrite)
                            {
                                while (_valuesBuffer.Count > 0)
                                {
                                    var sensorBufferedValue = _valuesBuffer.Peek();

                                    var bytes = _sensorIdAsBytesArray
                                        .Concat(BitConverter.GetBytes(sensorBufferedValue))
                                        .ToArray();

                                    _stream.Write(bytes, 0, bytes.Length);

                                    _valuesBuffer.Dequeue();

                                    OnSensorSendEvent(_sensorId, sensorBufferedValue);
                                }
                            }
                            else
                                OnSensorBufferedEvent(_sensorId, sensorValue);
                        }
                        catch (IOException e)
                        {
                            if (e.InnerException is SocketException innerExcception)
                            {
                                if (innerExcception.SocketErrorCode == SocketError.ConnectionReset) 

                                _reconnectTimer.Change(0, 5000);
                            }

                            _stream?.Close();
                            _tcpClient.Close();

                            OnSensorFailed(e);
                        }

                        Thread.Sleep(_random.Next(minTimeInterval, maxTimeInterval));
                    }
                }
                catch (Exception e)
                {
                    OnSensorFailed(e);
                    throw;
                }
            });

            return this;
        }

        protected virtual void OnSensorFailed(Exception ex)
        {
            SensorFailed?.Invoke(this, ex);
        }

        protected virtual void OnSensorSendEvent(int sensorId, int sensorValue)
        {
            SensorSendEvent?.Invoke(this, sensorId, sensorValue);
        }

        protected virtual void OnSensorBufferedEvent(int sensorId, int sensorValue)
        {
            SensorBufferedEvent?.Invoke(this, sensorId, sensorValue);
        }
    }
}
