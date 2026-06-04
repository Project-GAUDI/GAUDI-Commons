namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetContentEncoding : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No063_フィールド\"message\"がnull")]
    public void Message_Case063()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);

        var result = obj.GetContentEncoding();

        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No064_フィールド\"message\"がnull以外")]
    public void Message_Case064()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        var expected = message!.ContentEncoding;

        var result = obj.GetContentEncoding();

        Assert.Equal(expected, result);
    }
}
