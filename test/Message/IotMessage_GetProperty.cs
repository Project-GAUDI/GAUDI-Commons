using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_GetProperty
{
    [Fact(DisplayName = "No348_正常系：正常動作の確認_登録済みのkeyを指定して、メソッドを呼び出す")]
    public void Case348()
    {
        var message = new IotMessage();
        message.SetProperty("key", "value");

        var result = message.GetProperty("key");

        Assert.Equal("value", result);
    }

    [Fact(DisplayName = "No349_正常系：正常動作の確認_未登録のkeyを指定して、メソッドを呼び出す")]
    public void Case349()
    {
        var message = new IotMessage();

        var result = message.GetProperty("key");

        Assert.Null(result);
    }

    [Fact(DisplayName = "No350_異常系：異常動作の確認_keyにnullを指定して、メソッドを呼び出す")]
    public void Case350()
    {
        var message = new IotMessage();

        Assert.Throws<ArgumentNullException>(() => message.GetProperty(null!));
    }

    [Fact(DisplayName = "No351_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case351()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.GetProperty("key");

        Assert.Null(result);
    }
}