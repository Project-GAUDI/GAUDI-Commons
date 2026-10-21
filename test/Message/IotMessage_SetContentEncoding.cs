using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_SetContentEncoding
{
    [Fact(DisplayName = "No376_正常系：正常動作の確認_contentEncodingに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case376()
    {
        var message = new IotMessage();

        message.SetContentEncoding("utf-16");

        Assert.Equal("utf-16", message.GetContentEncoding());
    }

    [Fact(DisplayName = "No377_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case377()
    {
        var message = new IotMessage();
        message.Dispose();

        message.SetContentEncoding("utf-16");

        Assert.Null(message.GetContentEncoding());
    }
}