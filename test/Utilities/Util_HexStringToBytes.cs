using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class Util_HexStringToBytes
{
    [Fact(DisplayName = "No613_正常系：正常動作の確認_valに\"01-0A-FF\"を指定して、メソッドを呼び出す")]
    public void Case613()
    {
        var result = Util.HexStringToBytes("01-0A-FF");

        Assert.Equal(new byte[] { 1, 10, 255 }, result);
    }

    [Fact(DisplayName = "No614_正常系：正常動作の確認_valに\"0A\"を指定して、メソッドを呼び出す")]
    public void Case614()
    {
        var result = Util.HexStringToBytes("0A");

        Assert.Equal(new byte[] { 10 }, result);
    }

    [Fact(DisplayName = "No615_異常系：異常動作の確認_valに\"01-F-FF\"を指定して、メソッドを呼び出す")]
    public void Case615()
    {
        Assert.Throws<Exception>(() => Util.HexStringToBytes("01-F-FF"));
    }

    [Fact(DisplayName = "No616_異常系：異常動作の確認_valに\"01-GG-FF\"を指定して、メソッドを呼び出す")]
    public void Case616()
    {
        Assert.Throws<FormatException>(() => Util.HexStringToBytes("01-GG-FF"));
    }

    [Fact(DisplayName = "No617_異常系：異常動作の確認_valにnullを指定して、メソッドを呼び出す")]
    public void Case617()
    {
        Assert.Throws<NullReferenceException>(() => Util.HexStringToBytes(null!));
    }
}
