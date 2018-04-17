using System.Collections.Generic;
using System.Net;

namespace EmulatorOfSensors.Helpers
{
    public static class EmulatorExtensions
    {
        public static IPEndPoint GetEndPoint(this IReadOnlyList<string> args)
        {
            IPAddress address = null;
            var port = 0;

            var i = 0;

            while (i < args?.Count)
            {
                switch (args[i])
                {
                    case "-s" when i < args.Count - 1:
                        IPAddress.TryParse(args[++i], out address);
                        break;
                    case "-p" when i < args.Count - 1:
                        int.TryParse(args[++i], out port);
                        break;
                    default:
                        i++;
                        break;
                }
            }

            if (address != null && port > 0)
                return new IPEndPoint(address, port);

            if (address != null)
                return new IPEndPoint(address, 0);

            if (port > 0)
                return new IPEndPoint(IPAddress.Any, port);

            return new IPEndPoint(IPAddress.Any, 0);
        }

        public static uint GetSensorCount(this IReadOnlyList<string> args)
        {
            var countSensor = uint.MinValue;
            var i = 0;

            while (i < args?.Count)
            {
                switch (args[i])
                {
                    case "-n" when i < args.Count - 1:
                        uint.TryParse(args[++i], out countSensor);
                        break;
                    default:
                        i++;
                        break;
                }
            }

            return countSensor;
        }
    }
}
