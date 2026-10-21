using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class Util_HexStringToByte
{
    [Fact(DisplayName = "No608_正常系：正常動作の確認_valに\"FF\"を指定して、メソッドを呼び出す")]
    public void Case608()
    {
        var result = Util.HexStringToByte("FF");

        Assert.Equal(255, result);
    }

    [Fact(DisplayName = "No609_正常系：正常動作の確認_valに\"0a\"を指定して、メソッドを呼び出す")]
    public void Case609()
    {
        var result = Util.HexStringToByte("0a");

        Assert.Equal(10, result);
    }

    [Fact(DisplayName = "No610_異常系：異常動作の確認_valに\"F\"を指定して、メソッドを呼び出す")]
    public void Case610()
    {
        Assert.Throws<Exception>(() => Util.HexStringToByte("F"));
    }

    [Fact(DisplayName = "No611_異常系：異常動作の確認_valに\"GG\"を指定して、メソッドを呼び出す")]
    public void Case611()
    {
        Assert.Throws<FormatException>(() => Util.HexStringToByte("GG"));
    }

    [Fact(DisplayName = "No612_異常系：異常動作の確認_valにnullを指定して、メソッドを呼び出す")]
    public void Case612()
    {
        Assert.Throws<NullReferenceException>(() => Util.HexStringToByte(null!));
    }
}
