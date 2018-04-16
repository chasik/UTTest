using System.Collections.Concurrent;
using EmulatorOfSensors.Server.Interfaces;

namespace EmulatorOfSensors.Server
{
    public class SaversStorage : ISaversStorage
    {
        private readonly ConcurrentDictionary<string, IValueSaver> _fileStreamsStorage;

        public SaversStorage()
        {
            _fileStreamsStorage = new ConcurrentDictionary<string, IValueSaver>();
        }

        public string SensorInfo(int sensorId, int sensorValue)
        {
            var fileName = $"{sensorId}.txt";

            var fileStreamWrapper = _fileStreamsStorage.GetOrAdd(fileName, f => new ValueSaver(f));

            return fileStreamWrapper.Save(sensorId, sensorValue);
        }
    }
}
