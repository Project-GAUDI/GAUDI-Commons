using System.Text.RegularExpressions;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class Util_GetMessageId
{
    [Fact(DisplayName = "No599_正常系：正常動作の確認_メソッドを呼び出す")]
    public void Case599()
    {
        var result = Util.GetMessageId();

        Assert.Equal(49, result.Length);
        Assert.Matches(new Regex(@"^\d{17}[0-9a-f]{32}$"), result);
    }

    [Fact(DisplayName = "No600_正常系：正常動作の確認_メソッドを連続して2回呼び出す")]
    public void Case600()
    {
        var first = Util.GetMessageId();
        var second = Util.GetMessageId();

        Assert.NotEqual(first, second);
    }
}
