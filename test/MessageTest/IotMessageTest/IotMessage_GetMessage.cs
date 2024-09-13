using Xunit;
using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    public class IotMessage_GetMessage
    {
        [Fact]
        public void GetMessageTest001()
        {
            // Arrange
            IotMessage MyIotMessage = new IotMessage();
            Message expectedMessage = MyIotMessage.GetMessage();

            // Act
            Message actualMessage = MyIotMessage.GetMessage();

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

    }
}