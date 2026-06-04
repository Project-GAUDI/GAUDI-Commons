namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetBodyLength : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No017_GetBytesの戻り値がnull")]
    public void Message_Case017()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);
        var actual = obj.GetBodyLength();
        Assert.Equal(0, actual);
    }

    [Fact(DisplayName = "Message No018_GetBytesの戻り値がnull以外")]
    public void Message_Case018()
    {
        const int BODY_LENGTH = 50;
        var bytes = new byte[BODY_LENGTH];
        new Random().NextBytes(bytes);

        using var obj = new IotMessage(bytes);

        var actual = obj.GetBodyLength();
        Assert.Equal(BODY_LENGTH, actual);
    }
}
