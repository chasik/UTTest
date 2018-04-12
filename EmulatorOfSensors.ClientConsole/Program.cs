using System;
using EmulatorOfSensors.Client.Sensors;
using EmulatorOfSensors.ClientConsole.Properties;
using EmulatorOfSensors.Helpers;

namespace EmulatorOfSensors.ClientConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endPoint = args.GetEndPoint();

            if (endPoint?.Address == null || endPoint.Port == 0)
                ConsoleHelpers.RequestEndPoint(endPoint, Settings.Default.Address, Settings.Default.Port);

            var sensorsCount = args.GetSensorCount();
            if (sensorsCount == uint.MinValue)
                sensorsCount = ConsoleHelpers.RequestSensorCount(Settings.Default.SensorCount);

            for (var i = 1; i <= sensorsCount; i++)
            {
                var sensor = new Sensor(endPoint, i);

                sensor.SensorSendEvent += (sender, sensorId, value) => { Console.WriteLine($"{sensorId}\t{value}\tsended"); };
                sensor.SensorBufferedEvent += (sender, sensorId, value) => { Console.WriteLine($"{sensorId}\t{value}\tbuffered"); };

                sensor.SensorFailed += ConsoleHelpers.OnFailed;

                sensor.StartGenerate(0, 1000);
            }

            Console.ReadLine();
        }
    }
}
