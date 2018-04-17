using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using EmulatorOfSensors.Client.Interfaces;
using EmulatorOfSensors.Helpers;

namespace EmulatorOfSensors.Client.Sensors
{
    public class Sensor : ISensor
    {
        #region | Fields |

        public event SensorEventHandler SensorBufferedEvent;
        public event SensorEventHandler SensorSendEvent;
        public event SensorFailedHandler SensorFailed;

        private TcpClient _tcpClient;
        private NetworkStream _stream;

        private readonly ConcurrentQueue<int> _valuesBuffer;

        private readonly IPEndPoint _endPoint;
        private readonly IValueGenerator _generator;

        private readonly int _sensorId;
        private readonly byte[] _sensorIdAsBytesArray;

        private readonly Timer _reconnectTimer;

        #endregion

        #region | Constructors |

        public Sensor()
        {
            _valuesBuffer = new ConcurrentQueue<int>();

            _reconnectTimer = new Timer(ReConnectToServer, null, 0, 0);
        }

        public Sensor(IValueGenerator generator, IPEndPoint endPoint, int sensorId) : this()
        {
            _generator = generator;

            _endPoint = endPoint;
            _sensorId = sensorId;

            _sensorIdAsBytesArray = BitConverter.GetBytes(_sensorId);
        }

        #endregion

        public ISensor StartGenerate(int minValue = int.MinValue, int maxValue = int.MaxValue, int minTimeInterval = 1, int maxTimeInterval = 10000)
        {
            _generator.ValueGenerated += (sender, value) => NewValueGenerated(value);

            _generator.Start(minValue, maxValue, minTimeInterval, maxTimeInterval);

            return this;
        }

        #region | Private methods |

        private void ReConnectToServer(object state)
        {
            try
            {
                _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);

                _tcpClient = new TcpClient(_endPoint.Address.ToString(), _endPoint.Port);
                _stream = _tcpClient.GetStream();
            }
            catch (Exception ex)
            {
                _reconnectTimer.Change(5000, 0);
                OnSensorFailed($"SensorId:{_sensorId}", ex);
            } 
        }

        private void SendValues()
        {
            try
            {
                while (_stream != null && _stream.CanWrite 
                                       && _valuesBuffer.Count > 0 
                                       && _valuesBuffer.TryPeek(out var sensorBufferedValue))
                {
                    var bytes = _sensorIdAsBytesArray.SerializeForSend(sensorBufferedValue);

                    _stream.Write(bytes, 0, bytes.Length);

                    _valuesBuffer.TryDequeue(out var _);

                    OnSensorSendEvent(_sensorId, sensorBufferedValue);
                }
            }
            catch (IOException e)
            {
                if (e.InnerException is SocketException innerExcception)
                {
                    if (innerExcception.SocketErrorCode == SocketError.ConnectionReset)
                        _reconnectTimer.Change(0, 0);
                }

                _stream?.Close();
                _tcpClient.Close();

                OnSensorFailed($"SensorId:{_sensorId}", e);
            }
        }

        private void NewValueGenerated(int sensorValue)
        {
            try
            {
                _valuesBuffer.Enqueue(sensorValue);

                SendValues();

                if (_valuesBuffer.Count > 0)
                    OnSensorBufferedEvent(_sensorId, sensorValue);
            }
            catch (Exception e)
            {
                OnSensorFailed($"SensorId:{_sensorId}", e);
                throw;
            }
        }

        #endregion

        #region | Events invocators |

        protected virtual void OnSensorFailed(string comment, Exception ex)
        {
            SensorFailed?.Invoke(this, comment, ex);
        }

        protected virtual void OnSensorSendEvent(int sensorId, int sensorValue)
        {
            SensorSendEvent?.Invoke(this, sensorId, sensorValue);
        }

        protected virtual void OnSensorBufferedEvent(int sensorId, int sensorValue)
        {
            SensorBufferedEvent?.Invoke(this, sensorId, sensorValue);
        }

        #endregion
    }
}
