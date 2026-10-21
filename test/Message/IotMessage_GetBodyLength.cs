using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_GetBodyLength
{
    [Fact(DisplayName = "No338_正常系：正常動作の確認_Bodyにデータが存在する状態で、メソッドを呼び出す")]
    public void Case338()
    {
        var bytes = Encoding.UTF8.GetBytes("message");
        var message = new IotMessage(bytes);

        var result = message.GetBodyLength();

        Assert.Equal(bytes.Length, result);
    }

    [Fact(DisplayName = "No339_正常系：正常動作の確認_GetBytesがnullを返す状態で、メソッドを呼び出す")]
    public void Case339()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.GetBodyLength();

        Assert.Equal(0, result);
    }
}