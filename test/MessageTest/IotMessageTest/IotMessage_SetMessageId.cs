using Xunit;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    public class IotMessage_SetMessageId
    {
        [Fact]
        public void SetMessageIdTest001()
        {
            IotMessage MyIotMessage = new IotMessage();
            MyIotMessage.SetMessageId("test");

            Assert.Equal("test", MyIotMessage.GetMessageId());
        }

    }
}