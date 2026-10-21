using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_GetMessageId
{
    [Fact(DisplayName = "No364_正常系：正常動作の確認_SetMessageIdで値を設定した状態で、メソッドを呼び出す")]
    public void Case364()
    {
        var message = new IotMessage();
        message.SetMessageId("message-id");

        var result = message.GetMessageId();

        Assert.Equal("message-id", result);
    }

    [Fact(DisplayName = "No365_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case365()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.GetMessageId();

        Assert.Null(result);
    }
}