using Newtonsoft.Json;

namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_SetProperties : IotMessageTesterBase
{
    private const string DUMMY_KEY1 = "dummy-key1";

    private const string DUMMY_KEY2 = "dummy-key2";

    private const string DUMMY_VALUE1 = "dummy-value1";

    private const string DUMMY_VALUE2 = "dummy-value2";

    private const string PREVIOUS_VALUE = "previous-value";

    private readonly IDictionary<string, string> _predefinedProperties = new Dictionary<string, string>()
    {
        { DUMMY_KEY1, DUMMY_VALUE1 },
        { DUMMY_KEY2, DUMMY_VALUE2 }
    };

    [Fact(DisplayName = "Message No031_フィールド\"message\"がnull")]
    public void Message_Case031()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);

        var properties = new Dictionary<string, string>();
        var result = obj.SetProperties(properties);

        Assert.False(result);
    }

    [Fact(DisplayName = "Message No032_フィールド\"message\"がnull以外、かつ、引数\"properties\"がnull")]
    public void Message_Case032()
    {
        using var obj = new IotMessage();
        Assert.ThrowsAny<Exception>(() => obj.SetProperties(null));
    }

    [Fact(DisplayName = "Message No033_フィールド\"message\"がnull以外、かつ、引数\"properties\"の要素数が0")]
    public void Message_Case033()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.Properties.Add(DUMMY_KEY1, PREVIOUS_VALUE);

        var previous = JsonConvert.SerializeObject(message!.Properties);

        var properties = new Dictionary<string, string>();
        var result = obj.SetProperties(properties);
        var next = JsonConvert.SerializeObject(message.Properties);

        Assert.True(result);
        Assert.Equal(previous, next);
    }

    [Fact(DisplayName = "Message No034_フィールド\"message\"がnull以外、かつ、引数\"properties\"の要素数が2(内1つは、フィールド\"message.Properties\"に存在するキー)、かつ、引数\"setMode\"が未指定")]
    public void Message_Case034()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.Properties.Add(DUMMY_KEY1, PREVIOUS_VALUE);

        var result = obj.SetProperties(_predefinedProperties);

        Assert.True(result);
        Assert.Equal(DUMMY_VALUE1, message.Properties[DUMMY_KEY1]);
        Assert.Equal(DUMMY_VALUE2, message.Properties[DUMMY_KEY2]);
    }

    [Fact(DisplayName = "Message No035_フィールド\"message\"がnull以外、かつ、引数\"properties\"の要素数が2(内1つは、フィールド\"message.Properties\"に存在するキー)、かつ、引数\"setMode\"が”Add”")]
    public void Message_Case035()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.Properties.Add(DUMMY_KEY1, PREVIOUS_VALUE);

        var result = obj.SetProperties(_predefinedProperties, IotMessage.PropertySetMode.Add);

        Assert.False(result);
        Assert.Equal(PREVIOUS_VALUE, message.Properties[DUMMY_KEY1]);
        Assert.Equal(DUMMY_VALUE2, message.Properties[DUMMY_KEY2]);
    }

    [Fact(DisplayName = "Message No036_フィールド\"message\"がnull以外、かつ、引数\"properties\"の要素数が2(内1つは、フィールド\"message.Properties\"に存在するキー)、かつ、引数\"setMode\"が\"Modify\"")]
    public void Message_Case036()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.Properties.Add(DUMMY_KEY1, PREVIOUS_VALUE);

        var result = obj.SetProperties(_predefinedProperties, IotMessage.PropertySetMode.Modify);

        Assert.False(result);
        Assert.Equal(DUMMY_VALUE1, message.Properties[DUMMY_KEY1]);
        Assert.False(message.Properties.ContainsKey(DUMMY_KEY2));
    }

    [Fact(DisplayName = "Message No037_フィールド\"message\"がnull以外、かつ、引数\"properties\"の要素数が2(内1つは、フィールド\"message.Properties\"に存在するキー)、かつ、引数\"setMode\"が\"AddOrModify\"")]
    public void Message_Case037()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.Properties.Add(DUMMY_KEY1, PREVIOUS_VALUE);

        var result = obj.SetProperties(_predefinedProperties, IotMessage.PropertySetMode.AddOrModify);

        Assert.True(result);
        Assert.Equal(DUMMY_VALUE1, message.Properties[DUMMY_KEY1]);
        Assert.Equal(DUMMY_VALUE2, message.Properties[DUMMY_KEY2]);
    }
}
