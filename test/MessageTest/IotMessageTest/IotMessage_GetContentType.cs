namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetContentType : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No059_フィールド\"message\"がnull")]
    public void Message_Case059()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);

        var result = obj.GetContentType();

        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No060_フィールド\"message\"がnull以外")]
    public void Message_Case060()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        var expected = message!.ContentType;

        var result = obj.GetContentType();

        Assert.Equal(expected, result);
    }
}
