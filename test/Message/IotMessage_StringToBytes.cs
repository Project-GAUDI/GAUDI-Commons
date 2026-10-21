using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_StringToBytes
{
    [Fact(DisplayName = "No362_正常系：正常動作の確認_convStringに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case362()
    {
        const string value = "テスト";

        var result = IotMessage.StringToBytes(value);

        Assert.Equal(Encoding.UTF8.GetBytes(value), result);
    }

    [Fact(DisplayName = "No363_正常系：正常動作の確認_convStringにnullを指定して、メソッドを呼び出す")]
    public void Case363()
    {
        var result = IotMessage.StringToBytes(null!);

        Assert.Null(result);
    }
}