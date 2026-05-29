using System.Text;

namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_StringToByates : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No042_引数\"convString\"がnull")]
    public void Message_Case042()
    {
        var result = IotMessage.StringToByates(null);

        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No043_引数\"convString\"がnull以外")]
    public void Message_Case043()
    {
        var uuid = Guid.NewGuid().ToString();
        var bytes = Encoding.UTF8.GetBytes(uuid);

        var result = IotMessage.StringToByates(uuid);
        Assert.Equal(bytes, result);
    }
}
