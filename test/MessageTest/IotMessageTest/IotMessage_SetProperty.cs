namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_SetProperty : IotMessageTesterBase
{
    private const string DUMMY_KEY = "dummy-key";

    private const string DUMMY_VALUE = "dummy-value";

    private const string PREVIOUS_VALUE = "previous-value";

    [Fact(DisplayName = "Message No019_フィールド\"message\"がnull")]
    public void Message_Case019()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);
        var result = obj.SetProperty(DUMMY_KEY, DUMMY_VALUE);

        Assert.False(result);
    }

    [Fact(DisplayName = "Message No020_フィールド\"message\"がnull以外、かつ、\"message.Properties\"に引数\"key\"が存在し、\"setMode\"が未指定")]
    public void Message_Case020()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.Properties.Add(DUMMY_KEY, PREVIOUS_VALUE);

        var result = obj.SetProperty(DUMMY_KEY, DUMMY_VALUE);

        Assert.True(result);
        Assert.Equal(DUMMY_VALUE, message.Properties[DUMMY_KEY]);
    }

    [Fact(DisplayName = "Message No021_フィールド\"message\"がnull以外、かつ、\"message.Properties\"に引数\"key\"が存在し、\"setMode\"が\"Add\"")]
    public void Message_Case021()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.Properties.Add(DUMMY_KEY, PREVIOUS_VALUE);

        var result = obj.SetProperty(DUMMY_KEY, DUMMY_VALUE, IotMessage.PropertySetMode.Add);

        Assert.False(result);
    }

    [Fact(DisplayName = "Message No022_フィールド\"message\"がnull以外、かつ、\"message.Properties\"に引数\"key\"が存在し、\"setMode\"が\"Modify\"")]
    public void Message_Case022()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.Properties.Add(DUMMY_KEY, PREVIOUS_VALUE);

        var result = obj.SetProperty(DUMMY_KEY, DUMMY_VALUE, IotMessage.PropertySetMode.Modify);

        Assert.True(result);
        Assert.Equal(DUMMY_VALUE, message.Properties[DUMMY_KEY]);
    }

    [Fact(DisplayName = "Message No023_フィールド\"message\"がnull以外、かつ、\"message.Properties\"に引数\"key\"が存在し、\"setMode\"が\"AddOrModify\"")]
    public void Message_Case023()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.Properties.Add(DUMMY_KEY, PREVIOUS_VALUE);

        var result = obj.SetProperty(DUMMY_KEY, DUMMY_VALUE, IotMessage.PropertySetMode.AddOrModify);

        Assert.True(result);
        Assert.Equal(DUMMY_VALUE, message.Properties[DUMMY_KEY]);
    }

    [Fact(DisplayName = "Message No024_フィールド\"message\"がnull以外、かつ、\"message.Properties\"に引数\"key\"が存在せず、\"setMode\"が未指定")]
    public void Message_Case024()
    {
        using var obj = new IotMessage();

        var result = obj.SetProperty(DUMMY_KEY, DUMMY_VALUE);
        var message = GetMessageProperty(obj);

        Assert.True(result);
        Assert.Equal(DUMMY_VALUE, message!.Properties[DUMMY_KEY]);
    }

    [Fact(DisplayName = "Message No025_フィールド\"message\"がnull以外、かつ、\"message.Properties\"に引数\"key\"が存在せず、\"setMode\"が\"Add\"")]
    public void Message_Case025()
    {
        using var obj = new IotMessage();

        var result = obj.SetProperty(DUMMY_KEY, DUMMY_VALUE, IotMessage.PropertySetMode.Add);
        var message = GetMessageProperty(obj);

        Assert.True(result);
        Assert.Equal(DUMMY_VALUE, message!.Properties[DUMMY_KEY]);
    }

    [Fact(DisplayName = "Message No026_フィールド\"message\"がnull以外、かつ、\"message.Properties\"に引数\"key\"が存在せず、\"setMode\"が\"Modify\"")]
    public void Message_Case026()
    {
        using var obj = new IotMessage();

        var result = obj.SetProperty(DUMMY_KEY, DUMMY_VALUE, IotMessage.PropertySetMode.Modify);
        var message = GetMessageProperty(obj);

        Assert.False(result);
    }

    [Fact(DisplayName = "Message No027_フィールド\"message\"がnull以外、かつ、\"message.Properties\"に引数\"key\"が存在せず、\"setMode\"が\"AddOrModify\"")]
    public void Message_Case027()
    {
        using var obj = new IotMessage();

        var result = obj.SetProperty(DUMMY_KEY, DUMMY_VALUE, IotMessage.PropertySetMode.AddOrModify);
        var message = GetMessageProperty(obj);

        Assert.True(result);
        Assert.Equal(DUMMY_VALUE, message!.Properties[DUMMY_KEY]);
    }
}
