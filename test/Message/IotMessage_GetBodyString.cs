using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_GetBodyString
{
    [Fact(DisplayName = "No332_正常系：正常動作の確認_Bodyにデータが存在する状態で、メソッドを呼び出す")]
    public void Case332()
    {
        const string body = "message";
        var message = new IotMessage(Encoding.UTF8.GetBytes(body));

        var result = message.GetBodyString();

        Assert.Equal(body, result);
    }

    [Fact(DisplayName = "No333_正常系：正常動作の確認_GetBytesがnullを返す状態で、メソッドを呼び出す")]
    public void Case333()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.GetBodyString();

        Assert.Null(result);
    }
}