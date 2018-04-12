using System.IO;
using System.Text;
using EmulatorOfSensors.Helpers;

namespace EmulatorOfSensors.Server
{
    public class FileStreamWrapper
    {
        private readonly FileStream _fileStream;

        public FileStreamWrapper(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
            {
                var lastString = fileStream.ReadLastLine(Encoding.ASCII);
                var values = lastString?.Split(',');

                if (values != null && values.Length > 1 && int.TryParse(values[1], out var lastValue))
                    LastValue = lastValue;
            }

            _fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write);
        }

        public int? LastValue { get; set; }

        public void Write(byte[] bytes)
        {
            _fileStream.Write(bytes, 0, bytes.Length);
            _fileStream.Flush();
        }
    }
}
