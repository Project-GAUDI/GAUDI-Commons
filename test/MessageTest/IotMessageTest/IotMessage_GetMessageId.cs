using Xunit;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    public class IotMessage_GetMessageId
    {
        [Fact]
        public void GetMessageIdTest001()
        {
            // Arrange
            IotMessage MyIotMessage = new IotMessage();

            // Act
            string actualMessageId = MyIotMessage.GetMessageId();

            // Assert
            Assert.Null(actualMessageId);
        }

    }
}