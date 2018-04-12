using System;
using EmulatorOfSensors.Helpers;
using EmulatorOfSensors.Server;
using EmulatorOfSensors.ServerConsole.Properties;

namespace EmulatorOfSensors.ServerConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endPoint = args.GetEndPoint();

            if (endPoint?.Address == null || endPoint.Port == 0)
                ConsoleHelpers.RequestEndPoint(endPoint, Settings.Default.Address, Settings.Default.Port);

            var logger = new Logger();

            var listener = new Listener(endPoint);

            listener.ReceiveSensorValue += (sender, sensorId, value) =>
            {
                Console.WriteLine($"{sensorId}\t{value}");

                logger.AddValue(sensorId, value);
            };

            listener.ListenerFailed += ConsoleHelpers.OnFailed;

            listener.StartListen();
        }
    }
}
