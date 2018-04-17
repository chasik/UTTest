using System;
using System.Linq;

namespace EmulatorOfSensors.Helpers
{
    public static class ValueSerializer
    {
        public static byte[] SerializeForSend(this int firstValue, int secondValue)
        {
            return BitConverter.GetBytes(firstValue)
                .SerializeForSend(secondValue);
        }

        public static byte[] SerializeForSend(this byte[] firstValue, int secondValue)
        {
            return firstValue
                .Concat(BitConverter.GetBytes(secondValue))
                .ToArray();
        }

        public static int[] DeserializeToInt(this byte[] bytes)
        {
            return new[] {BitConverter.ToInt32(bytes, 0), BitConverter.ToInt32(bytes, 4)};
        }
    }
}
