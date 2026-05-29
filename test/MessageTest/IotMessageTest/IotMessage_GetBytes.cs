namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetBytes : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No008_フィールド\"byteData\"がnull以外")]
    public void Message_Case008()
    {
        var bytes = new byte[50];
        new Random().NextBytes(bytes);

        using var obj = new IotMessage(bytes);

        var byteData = GetByteDataProperty(obj);
        Assert.NotNull(byteData);

        var actual = obj.GetBytes();
        Assert.Equal(byteData, actual);
    }

    [Fact(DisplayName = "Message No009_フィールド\"byteData\"がnullかつ、\"message\"がnullではない")]
    public void Message_Case009()
    {
        var bytes = new byte[50];
        new Random().NextBytes(bytes);

        using var message = new Message(bytes);
        using var obj = new IotMessage();

        SetMessageProperty(obj, message);
        var actual = obj.GetBytes();
        Assert.Equal(bytes, actual);
    }

    [Fact(DisplayName = "Message No010_フィールド\"byteData\"がnullかつ、\"message\"がnull")]
    public void Message_Case010()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();

        SetMessageProperty(obj, null);
        var actual = obj.GetBytes();
        Assert.Null(actual);
    }
}
