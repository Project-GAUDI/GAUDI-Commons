using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class MyExtensions_ToBytes
{
    [Fact(DisplayName = "No747_正常系：正常動作の確認_thisStringに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case747()
    {
        var result = "test".ToBytes();

        Assert.Equal(Encoding.UTF8.GetBytes("test"), result);
    }

    [Fact(DisplayName = "No748_正常系：正常動作の確認_thisStringに空文字列を指定して、メソッドを呼び出す")]
    public void Case748()
    {
        var result = "".ToBytes();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "No749_正常系：正常動作の確認_thisStringにnullを指定して、メソッドを呼び出す")]
    public void Case749()
    {
        string? value = null;

        var result = value!.ToBytes();

        Assert.Null(result);
    }
}
