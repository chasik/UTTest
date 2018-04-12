using System.IO;
using System.Text;

namespace EmulatorOfSensors.Helpers
{
    public static class FileHelper
    {
        public static string ReadLastLine(this FileStream fileStream, Encoding encoding, string newline = "\n")
        {
            var charsize = encoding.GetByteCount("\n");
            var buffer = encoding.GetBytes(newline);

            var endpos = fileStream.Length / charsize;

            for (long pos = charsize + 1; pos < endpos; pos += charsize)
            {
                fileStream.Seek(-pos, SeekOrigin.End);
                fileStream.Read(buffer, 0, buffer.Length);

                if (encoding.GetString(buffer) != newline)
                    continue;

                buffer = new byte[fileStream.Length - fileStream.Position];
                fileStream.Read(buffer, 0, buffer.Length);

                return encoding.GetString(buffer);
            }

            return null;
        }
    }
}
