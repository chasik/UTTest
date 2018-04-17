using System;
using System.Net;

namespace EmulatorOfSensors.Helpers
{
    public static class ConsoleHelpers
    {
        private static object _locker = new object();

        public static void RequestEndPoint(IPEndPoint endPoint, string defaultAddress, string defaultPort)
        {
            if (Equals(endPoint.Address, IPAddress.Any))
            {
                Console.Write($"Введите адрес сервера [{defaultAddress}] :");
                var enteredAddress = Console.ReadLine();

                if (IPAddress.TryParse(string.IsNullOrEmpty(enteredAddress) ? defaultAddress : enteredAddress, out var address))
                    endPoint.Address = address;
            }

            if (endPoint.Port == 0)
            {
                Console.Write($"Введите порт подключения [{defaultPort}] :");
                var enteredPort = Console.ReadLine();

                if (int.TryParse(string.IsNullOrEmpty(enteredPort) ? defaultPort : enteredPort, out var port))
                    endPoint.Port = port;
            }
        }

        public static uint RequestSensorCount(uint defaultCount)
        {
            Console.Write($"Количество датчиков [{defaultCount}] :");
            var enteredCount = Console.ReadLine();

            return uint.TryParse(enteredCount, out var newCount) ? newCount : defaultCount;
        }

        public static void OnFailed(object sender, string comment, Exception exception)
        {
            lock (_locker)
            {
                var oldTextColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{comment} {exception.Message}");
                Console.ForegroundColor = oldTextColor == ConsoleColor.Red ? ConsoleColor.White : oldTextColor;
            }
        }
    }
}
