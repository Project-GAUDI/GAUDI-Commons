using System.Text;

namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_BytesToString : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No040_引数\"convBytes\"がnull")]
    public void Message_Case040()
    {
        var result = IotMessage.BytesToString(null);

        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No041_引数\"convBytes\"がnull以外")]
    public void Message_Case041()
    {
        var uuid = Guid.NewGuid().ToString();
        var bytes = Encoding.UTF8.GetBytes(uuid);

        var result = IotMessage.BytesToString(bytes);
        Assert.Equal(uuid, result);
    }
}
