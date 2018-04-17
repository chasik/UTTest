using EmulatorOfSensors.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmulatorOfSensors.Tests
{
    [TestClass]
    public class ValueSerializerTest
    {
        [TestMethod]
        public void Get_Equals_Values_Before_Serialize_And_After_Deserialize()
        {
            // Arange
            var sensorId = 17;
            var sensorValue = -997;

            // Act
            var bytesForSend = ValueSerializer.SerializeForSend(sensorId, sensorValue);
            var deserializedValues = bytesForSend.DeserializeToInt();

            // Assert
            Assert.AreEqual(deserializedValues[0], sensorId);
            Assert.AreEqual(deserializedValues[1], sensorValue);
        }
    }
}
