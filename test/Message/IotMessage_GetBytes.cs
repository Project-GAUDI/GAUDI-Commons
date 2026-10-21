using System.Text;
using AzureMessage = Microsoft.Azure.Devices.Client.Message;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_GetBytes
{
    [Fact(DisplayName = "No329_正常系：正常動作の確認_byteDataが未設定の状態で、メソッドを呼び出す")]
    public void Case329()
    {
        var bytes = Encoding.UTF8.GetBytes("message");
        var message = new IotMessage(new AzureMessage(bytes));

        var result = message.GetBytes();

        Assert.Equal(bytes, result);
    }

    [Fact(DisplayName = "No330_正常系：正常動作の確認_byteDataが設定されている状態で、メソッドを呼び出す")]
    public void Case330()
    {
        var bytes = Encoding.UTF8.GetBytes("message");
        var message = new IotMessage(bytes);

        var result = message.GetBytes();

        Assert.Same(bytes, result);
    }

    [Fact(DisplayName = "No331_異常系：異常動作の確認_message=null、byteData未設定の状態で、メソッドを呼び出す")]
    public void Case331()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.GetBytes();

        Assert.Null(result);
    }
}