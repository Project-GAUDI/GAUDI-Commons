namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_SetContentEncoding : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No065_フィールド\"message\"がnull")]
    public void Message_Case065()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);

        obj.SetContentEncoding(CONTENT_ENCODING_SHIFT_JIS);
    }

    [Fact(DisplayName = "Message No066_フィールド\"message\"がnull以外")]
    public void Message_Case066()
    {
        using var obj = new IotMessage();

        obj.SetContentEncoding(CONTENT_ENCODING_SHIFT_JIS);
        var message = GetMessageProperty(obj);
        var result = message!.ContentEncoding;

        Assert.Equal(CONTENT_ENCODING_SHIFT_JIS, result);
    }
}
