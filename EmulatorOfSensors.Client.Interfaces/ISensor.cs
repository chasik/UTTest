namespace EmulatorOfSensors.Client.Interfaces
{
    public interface ISensor
    {
        ISensor StartGenerate(int minValue, int maxValue, int minTimeInterval, int maxTimeInterval);
    }
}
