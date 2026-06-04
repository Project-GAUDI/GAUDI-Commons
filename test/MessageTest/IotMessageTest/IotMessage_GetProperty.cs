namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetProperty : IotMessageTesterBase
{
    private const string DUMMY_KEY = "dummy-key";

    private const string DUMMY_VALUE = "dummy-key";

    [Fact(DisplayName = "Message No028_フィールド\"message\"がnull")]
    public void Message_Case028()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);
        var result = obj.GetProperty(DUMMY_KEY);

        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No029_フィールド\"message\"がnull以外、かつ、\"message.Properties\"に引数\"key\"が含まれる")]
    public void Message_Case029()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.Properties.Add(DUMMY_KEY, DUMMY_VALUE);

        var result = obj.GetProperty(DUMMY_KEY);

        Assert.Equal(DUMMY_VALUE, result);
    }

    [Fact(DisplayName = "Message No030_フィールド\"message\"がnull以外、かつ、\"message.Properties\"に引数\"key\"が含まれない")]
    public void Message_Case030()
    {
        using var obj = new IotMessage();
        var result = obj.GetProperty(DUMMY_KEY);

        Assert.Null(result);
    }
}
