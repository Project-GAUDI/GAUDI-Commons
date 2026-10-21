using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_Dispose
{
    [Fact(DisplayName = "No327_正常系：正常動作の確認_インスタンス生成後、Disposeを1回呼び出す")]
    public void Case327()
    {
        var message = new IotMessage();

        message.Dispose();

        Assert.Null(message.GetBytes());
    }

    [Fact(DisplayName = "No328_異常系：異常動作の確認_Disposeを2回連続で呼び出す")]
    public void Case328()
    {
        var message = new IotMessage();
        message.Dispose();

        Assert.Throws<NullReferenceException>(() => message.Dispose());
    }
}