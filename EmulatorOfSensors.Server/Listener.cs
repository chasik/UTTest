using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EmulatorOfSensors.Server
{
    public delegate void TcpListenerFailedHandler(object sender, Exception e);

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

                    worker.ReceiveSensorValue += OnReceiveSensorValue;

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            worker.WaitData();
                        }
                        catch (Exception ex)
                        {
                            OnListenerFailed(ex);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                OnListenerFailed(e);
                throw;
            }
            finally
            {
                _server?.Stop();
            }
        }

        protected virtual void OnListenerFailed(Exception ex)
        {
            ListenerFailed?.Invoke(this, ex);
        }

        protected virtual void OnReceiveSensorValue(object sender, int sensorId, int sensorValue)
        {
            ReceiveSensorValue?.Invoke(sender, sensorId, sensorValue);
        }
    }
}
