using System.Net;
using EmulatorOfSensors.Client.Interfaces;
using EmulatorOfSensors.Client.Sensors;

namespace EmulatorOfSensors.Helpers
{
    public class SensorFactory
    {
        public static ISensor Create(IPEndPoint endPoint, int sensorId)
        {
            var sensor = new Sensor(new Generator(), endPoint, sensorId);

            return sensor;
        }
    }
}
