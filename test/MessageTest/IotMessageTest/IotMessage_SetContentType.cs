namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_SetContentType : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No061_フィールド\"message\"がnull")]
    public void Message_Case061()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);

        obj.SetContentType(CONTENT_TYPE_APPLICATION_XML);
    }

    [Fact(DisplayName = "Message No062_フィールド\"message\"がnull以外")]
    public void Message_Case062()
    {
        using var obj = new IotMessage();

        obj.SetContentType(CONTENT_TYPE_APPLICATION_XML);
        var message = GetMessageProperty(obj);
        var result = message!.ContentType;

        Assert.Equal(CONTENT_TYPE_APPLICATION_XML, result);
    }
}
