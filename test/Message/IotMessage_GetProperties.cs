using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_GetProperties
{
    [Fact(DisplayName = "No358_正常系：正常動作の確認_プロパティが設定済みの状態で、メソッドを呼び出す")]
    public void Case358()
    {
        var message = new IotMessage();
        message.SetProperty("key1", "value1");
        message.SetProperty("key2", "value2");

        var result = message.GetProperties();

        Assert.NotNull(result);
        Assert.Equal("value1", result["key1"]);
        Assert.Equal("value2", result["key2"]);
    }

    [Fact(DisplayName = "No359_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case359()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.GetProperties();

        Assert.Null(result);
    }
}