using System;
using System.Threading;
using EmulatorOfSensors.Client.Interfaces;

namespace EmulatorOfSensors.Client.Sensors
{
    public class Generator : IValueGenerator
    {
        private readonly Random _random;
        private Thread _generatorThread;

        public event ValueGeneratedHandler ValueGenerated;

        public Generator()
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
        }

        public void Start(int minValue, int maxValue, int minTimeInterval, int maxTimeInterval)
        {
            if (minTimeInterval < 1)
                throw new ArgumentOutOfRangeException();

            _generatorThread = new Thread(() =>
            {
                while (true)
                {
                    var sensorValue = _random.Next(minValue, maxValue);

                    OnValueGenerated(sensorValue);

                    Thread.Sleep(_random.Next(minTimeInterval, maxTimeInterval));
                }
            });

            _generatorThread.Start();
        }

        public void Stop()
        {
            _generatorThread?.Abort();
        }

        protected virtual void OnValueGenerated(int value)
        {
            ValueGenerated?.Invoke(this, value);
        }
    }
}
