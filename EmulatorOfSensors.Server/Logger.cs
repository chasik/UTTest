using System.Collections.Concurrent;
using System.Text;

namespace EmulatorOfSensors.Server
{
    public class Logger
    {
        private readonly ConcurrentDictionary<string, FileStreamWrapper> _fileStreamsStorage;

        public Logger()
        {
            _fileStreamsStorage = new ConcurrentDictionary<string, FileStreamWrapper>();
        }

        public void AddValue(int sensorId, int sensorValue)
        {
            var fileName = $"{sensorId}.txt";

            var fileStreamWrapper = _fileStreamsStorage.GetOrAdd(fileName, f => new FileStreamWrapper(f));

            var lastValueDelta = sensorValue - (fileStreamWrapper.LastValue ?? 0);
            var bytes = Encoding.ASCII.GetBytes($"{sensorId},{sensorValue},{lastValueDelta}\n");

            fileStreamWrapper.Write(bytes);
            fileStreamWrapper.LastValue = sensorValue;
        }
    }
}
