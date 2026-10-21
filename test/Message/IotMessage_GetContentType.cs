using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_GetContentType
{
    [Fact(DisplayName = "No368_正常系：正常動作の確認_インスタンス生成直後の状態で、メソッドを呼び出す")]
    public void Case368()
    {
        var message = new IotMessage();

        var result = message.GetContentType();

        Assert.Equal("application/json", result);
    }

    [Fact(DisplayName = "No369_正常系：正常動作の確認_SetContentTypeで値を設定した状態で、メソッドを呼び出す")]
    public void Case369()
    {
        var message = new IotMessage();
        message.SetContentType("text/plain");

        var result = message.GetContentType();

        Assert.Equal("text/plain", result);
    }

    [Fact(DisplayName = "No370_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case370()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.GetContentType();

        Assert.Null(result);
    }
}