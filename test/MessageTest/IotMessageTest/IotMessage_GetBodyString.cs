using System.Text;

namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetBodyString : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No011_GetBytesの戻り値がnull")]
    public void Message_Case011()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();

        // GetBytesの戻り値をnullにするため、自動生成されるMessageを破棄する
        SetMessageProperty(obj, null);

        var result = obj.GetBodyString();
        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No012_GetBytesの戻り値がnull以外")]
    public void Message_Case012()
    {
        var uuid = Guid.NewGuid().ToString();
        var args = Encoding.UTF8.GetBytes(uuid);

        using var obj = new IotMessage(args);

        var bytes = obj.GetBytes();
        var expected = Encoding.UTF8.GetString(bytes);
        var actual = obj.GetBodyString();
        Assert.Equal(expected, actual);
    }
}
