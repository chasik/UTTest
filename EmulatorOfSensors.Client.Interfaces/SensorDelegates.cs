using System;

namespace EmulatorOfSensors.Client.Interfaces
{
    public delegate void SensorFailedHandler(object sender, string comment, Exception e);
    public delegate void SensorEventHandler(object sender, int sensorId, int sensorValue);
    public delegate void ValueGeneratedHandler(object sender, int value);
}
