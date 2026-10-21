using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_SetContentType
{
    [Fact(DisplayName = "No371_正常系：正常動作の確認_contentTypeに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case371()
    {
        var message = new IotMessage();

        message.SetContentType("text/plain");

        Assert.Equal("text/plain", message.GetContentType());
    }

    [Fact(DisplayName = "No372_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case372()
    {
        var message = new IotMessage();
        message.Dispose();

        message.SetContentType("text/plain");

        Assert.Null(message.GetContentType());
    }
}