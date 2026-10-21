using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_SetProperty
{
    [Fact(DisplayName = "No340_正常系：正常動作の確認_setModeを指定せず未登録のkeyとvalueを指定して、メソッドを呼び出す")]
    public void Case340()
    {
        var message = new IotMessage();

        var result = message.SetProperty("key", "value");

        Assert.True(result);
        Assert.Equal("value", message.GetProperty("key"));
    }

    [Fact(DisplayName = "No341_正常系：正常動作の確認_setModeを指定せず登録済みのkeyと新しいvalueを指定して、メソッドを呼び出す")]
    public void Case341()
    {
        var message = new IotMessage();
        message.SetProperty("key", "value");

        var result = message.SetProperty("key", "new-value");

        Assert.True(result);
        Assert.Equal("new-value", message.GetProperty("key"));
    }

    [Fact(DisplayName = "No342_正常系：正常動作の確認_setMode=Addを指定し未登録のkeyを指定して、メソッドを呼び出す")]
    public void Case342()
    {
        var message = new IotMessage();

        var result = message.SetProperty("key", "value", IotMessage.PropertySetMode.Add);

        Assert.True(result);
        Assert.Equal("value", message.GetProperty("key"));
    }

    [Fact(DisplayName = "No343_正常系：正常動作の確認_setMode=Addを指定し登録済みのkeyを指定して、メソッドを呼び出す")]
    public void Case343()
    {
        var message = new IotMessage();
        message.SetProperty("key", "value");

        var result = message.SetProperty("key", "new-value", IotMessage.PropertySetMode.Add);

        Assert.False(result);
        Assert.Equal("value", message.GetProperty("key"));
    }

    [Fact(DisplayName = "No344_正常系：正常動作の確認_setMode=Modifyを指定し登録済みのkeyと新しいvalueを指定して、メソッドを呼び出す")]
    public void Case344()
    {
        var message = new IotMessage();
        message.SetProperty("key", "value");

        var result = message.SetProperty("key", "new-value", IotMessage.PropertySetMode.Modify);

        Assert.True(result);
        Assert.Equal("new-value", message.GetProperty("key"));
    }

    [Fact(DisplayName = "No345_正常系：正常動作の確認_setMode=Modifyを指定し未登録のkeyを指定して、メソッドを呼び出す")]
    public void Case345()
    {
        var message = new IotMessage();

        var result = message.SetProperty("key", "value", IotMessage.PropertySetMode.Modify);

        Assert.False(result);
        Assert.Null(message.GetProperty("key"));
    }

    [Fact(DisplayName = "No346_異常系：異常動作の確認_keyにnullを指定して、メソッドを呼び出す")]
    public void Case346()
    {
        var message = new IotMessage();

        Assert.Throws<ArgumentNullException>(() => message.SetProperty(null!, "value"));
    }

    [Fact(DisplayName = "No347_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case347()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.SetProperty("key", "value");

        Assert.False(result);
    }
}