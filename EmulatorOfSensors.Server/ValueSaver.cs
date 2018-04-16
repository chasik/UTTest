using System.IO;
using System.Text;
using EmulatorOfSensors.Helpers;
using EmulatorOfSensors.Server.Interfaces;

namespace EmulatorOfSensors.Server
{
    public class ValueSaver : IValueSaver
    {
        private readonly FileStream _fileStream;
        private int? _lastValue;

        public ValueSaver(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
            {
                var lastString = fileStream.ReadLastLine(Encoding.ASCII);
                var values = lastString?.Split(',');

                if (values != null && values.Length > 1 && int.TryParse(values[1], out var lastValue))
                    _lastValue = lastValue;
            }

            _fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write);
        }

        public string Save(int sensorId, int sensorValue)
        {
            var lastValueDelta = sensorValue - (_lastValue ?? 0);

            var savedRow = $"{sensorId},{sensorValue},{lastValueDelta}\n";
            var bytes = Encoding.ASCII.GetBytes(savedRow);

            _fileStream.Write(bytes, 0, bytes.Length);
            _fileStream.Flush();

            _lastValue = sensorValue;

            return savedRow;
        }
    }
}
