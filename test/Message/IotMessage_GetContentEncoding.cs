using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_GetContentEncoding
{
    [Fact(DisplayName = "No373_正常系：正常動作の確認_インスタンス生成直後の状態で、メソッドを呼び出す")]
    public void Case373()
    {
        var message = new IotMessage();

        var result = message.GetContentEncoding();

        Assert.Equal("utf-8", result);
    }

    [Fact(DisplayName = "No374_正常系：正常動作の確認_SetContentEncodingで値を設定した状態で、メソッドを呼び出す")]
    public void Case374()
    {
        var message = new IotMessage();
        message.SetContentEncoding("utf-16");

        var result = message.GetContentEncoding();

        Assert.Equal("utf-16", result);
    }

    [Fact(DisplayName = "No375_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case375()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.GetContentEncoding();

        Assert.Null(result);
    }
}