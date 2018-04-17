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

            for (var id = 1; id <= sensorsCount; id++)
            {
                var newSensorId = id;
                var sensor = new Sensor(new Generator(), endPoint, newSensorId);

                sensor.SensorSendEvent += (sender, sensorId, value) => { Console.WriteLine($"{sensorId}\t{value}\tsended"); };
                sensor.SensorBufferedEvent += (sender, sensorId, value) => { Console.WriteLine($"{sensorId}\t{value}\tbuffered"); };

                sensor.SensorFailed += ConsoleHelpers.OnFailed;

                sensor.StartGenerate(minValue: 0, maxValue: 1000, minTimeInterval: 1, maxTimeInterval: 10000);
            }

            Console.ReadLine();
        }
    }
}
