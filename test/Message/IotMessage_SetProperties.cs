using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_SetProperties
{
    [Fact(DisplayName = "No352_正常系：正常動作の確認_setModeを指定せず複数の未登録key/valueを含むDictionaryを指定して、メソッドを呼び出す")]
    public void Case352()
    {
        var message = new IotMessage();
        var properties = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        var result = message.SetProperties(properties);

        Assert.True(result);
        Assert.Equal("value1", message.GetProperty("key1"));
        Assert.Equal("value2", message.GetProperty("key2"));
    }

    [Fact(DisplayName = "No353_正常系：正常動作の確認_setMode=Addを指定し複数の未登録のkeyを含むDictionaryを指定して、メソッドを呼び出す")]
    public void Case353()
    {
        var message = new IotMessage();
        var properties = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        var result = message.SetProperties(properties, IotMessage.PropertySetMode.Add);

        Assert.True(result);
        Assert.Equal("value1", message.GetProperty("key1"));
        Assert.Equal("value2", message.GetProperty("key2"));
    }

    [Fact(DisplayName = "No354_正常系：正常動作の確認_setMode=Addを指定し複数の未登録のkeyと登録済みのkeyを含むDictionaryを指定して、メソッドを呼び出す")]
    public void Case354()
    {
        var message = new IotMessage();
        message.SetProperty("key1", "value1");
        var properties = new Dictionary<string, string>
        {
            { "key1", "new-value1" },
            { "key2", "value2" }
        };

        var result = message.SetProperties(properties, IotMessage.PropertySetMode.Add);

        Assert.False(result);
        Assert.Equal("value1", message.GetProperty("key1"));
        Assert.Equal("value2", message.GetProperty("key2"));
    }

    [Fact(DisplayName = "No355_正常系：正常動作の確認_setMode=Modifyを指定し複数の未登録のkeyと登録済みのkeyを含むDictionaryを指定して、メソッドを呼び出す")]
    public void Case355()
    {
        var message = new IotMessage();
        message.SetProperty("key1", "value1");
        var properties = new Dictionary<string, string>
        {
            { "key1", "new-value1" },
            { "key2", "value2" }
        };

        var result = message.SetProperties(properties, IotMessage.PropertySetMode.Modify);

        Assert.False(result);
        Assert.Equal("new-value1", message.GetProperty("key1"));
        Assert.Null(message.GetProperty("key2"));
    }

    [Fact(DisplayName = "No356_異常系：異常動作の確認_propertiesにnullを指定して、メソッドを呼び出す")]
    public void Case356()
    {
        var message = new IotMessage();

        Assert.Throws<NullReferenceException>(() => message.SetProperties(null!));
    }

    [Fact(DisplayName = "No357_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case357()
    {
        var message = new IotMessage();
        message.Dispose();
        var properties = new Dictionary<string, string>
        {
            { "key", "value" }
        };

        var result = message.SetProperties(properties);

        Assert.False(result);
    }
}