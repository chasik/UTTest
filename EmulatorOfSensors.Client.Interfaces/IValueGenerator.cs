namespace EmulatorOfSensors.Client.Interfaces
{
    public interface IValueGenerator
    {
        event ValueGeneratedHandler ValueGenerated;

        void Start(int minValue, int maxValue, int minTimeInterval, int maxTimeInterval);
    }
}
