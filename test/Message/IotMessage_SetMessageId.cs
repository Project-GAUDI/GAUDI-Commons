using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_SetMessageId
{
    [Fact(DisplayName = "No366_正常系：正常動作の確認_messageIdに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case366()
    {
        var message = new IotMessage();

        message.SetMessageId("message-id");

        Assert.Equal("message-id", message.GetMessageId());
    }

    [Fact(DisplayName = "No367_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case367()
    {
        var message = new IotMessage();
        message.Dispose();

        message.SetMessageId("message-id");

        Assert.Null(message.GetMessageId());
    }
}