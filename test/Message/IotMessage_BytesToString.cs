using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_BytesToString
{
    [Fact(DisplayName = "No360_正常系：正常動作の確認_convBytesに任意のUTF8バイト配列を指定して、メソッドを呼び出す")]
    public void Case360()
    {
        const string value = "テスト";
        var bytes = Encoding.UTF8.GetBytes(value);

        var result = IotMessage.BytesToString(bytes);

        Assert.Equal(value, result);
    }

    [Fact(DisplayName = "No361_正常系：正常動作の確認_convBytesにnullを指定して、メソッドを呼び出す")]
    public void Case361()
    {
        var result = IotMessage.BytesToString(null!);

        Assert.Null(result);
    }
}