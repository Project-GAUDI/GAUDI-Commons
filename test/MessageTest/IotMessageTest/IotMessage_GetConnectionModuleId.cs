using Xunit;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    public class IotMessage_GetConnectionModuleId
    {
        [Fact]
        public void GetConnectionModuleIdTest001()
        {
            // Arrange
            IotMessage MyIotMessage = new IotMessage();
            //Act
            string connnectionmoduleId = MyIotMessage.GetConnectionModuleId();
            //Assert
            Assert.True(string.IsNullOrEmpty(connnectionmoduleId));
        }

    }
}