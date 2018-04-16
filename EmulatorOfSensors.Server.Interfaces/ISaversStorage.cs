namespace EmulatorOfSensors.Server.Interfaces
{
    public interface ISaversStorage
    {
        string SensorInfo(int sensorId, int sensorValue);
    }
}
