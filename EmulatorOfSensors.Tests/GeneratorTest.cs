using System;
using System.Threading;
using EmulatorOfSensors.Client.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmulatorOfSensors.Tests
{
    [TestClass]
    public class GeneratorTest
    {
        [TestMethod]
        public void Raise_ValueGenerated_Event_And_Check_Value_Interval()
        {
            // Arrange
            const int intervalBoundary = -112;
            int? result = null;

            var generator = new Generator();
            var resetEvent = new ManualResetEventSlim(false);

            void ValueGeneratedHandler(object sender, int value)
            {
                result = value;
                resetEvent.Set();

                (sender as Generator)?.Stop();
            }

            generator.ValueGenerated += ValueGeneratedHandler;

            // Act
            generator.Start(intervalBoundary, intervalBoundary, 1, 1);

            // Assert
            Assert.AreEqual(resetEvent.Wait(TimeSpan.FromMilliseconds(500)), true);
            Assert.AreEqual(intervalBoundary, result);

            generator.ValueGenerated -= ValueGeneratedHandler;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException),
            "minTimeInterval must be greates than 0")]
        public void MinTimeInterval_Argument_Must_Be_Greater_Than_Zero()
        {
            // Arrange
            var generator = new Generator();

            // Act
            generator.Start(0, 0, minTimeInterval: -1, maxTimeInterval: 100);

            // Assert
        }
    }
}
