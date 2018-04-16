namespace EmulatorOfSensors.Server.Interfaces
{
    public interface IValueSaver
    {
        string Save(int sensorId, int sensorValue);
    }
}