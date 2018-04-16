using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EmulatorOfSensors.Server
{
    public delegate void TcpListenerFailedHandler(object sender, string comment, Exception e);

    public class Listener
    {
        private readonly IPEndPoint _endPoint;
        private readonly TcpListener _server;

        public event TcpListenerFailedHandler ListenerFailed;
        public event ReceiveSensorValueHandler ReceiveSensorValue;

        public Listener(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
            _server = new TcpListener(endPoint);
        }

        public void StartListen()
        {
            try
            {
                _server.Start();

                while (true)
                {

                    var worker = new Worker(_server.AcceptTcpClient());

                    new Thread(() =>
                    {
                        try
                        {
                            worker.ReceiveSensorValue += OnReceiveSensorValue;
                            worker.WaitData();
                        }
                        catch (Exception ex)
                        {
                            OnListenerFailed("", ex);
                        }
                    }).Start();
                }
            }
            catch (Exception e)
            {
                OnListenerFailed("", e);
                throw;
            }
            finally
            {
                _server?.Stop();
            }
        }

        protected virtual void OnListenerFailed(string comment, Exception ex)
        {
            ListenerFailed?.Invoke(this, comment, ex);
        }

        protected virtual void OnReceiveSensorValue(object sender, int sensorId, int sensorValue)
        {
            ReceiveSensorValue?.Invoke(sender, sensorId, sensorValue);
        }
    }
}
