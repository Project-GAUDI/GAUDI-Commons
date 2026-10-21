using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class I2CActionExtentions_ToString
{
    [Fact(DisplayName = "No577_正常系：正常動作の確認_selfにI2CAction.Readを指定して、メソッドを呼び出す")]
    public void Case577()
    {
        var result = I2CActionExtentions.ToString(I2CAction.Read);

        Assert.Equal("read", result);
    }

    [Fact(DisplayName = "No578_正常系：正常動作の確認_selfにI2CAction.Waitを指定して、メソッドを呼び出す")]
    public void Case578()
    {
        var result = I2CActionExtentions.ToString(I2CAction.Wait);

        Assert.Equal("wait", result);
    }

    [Fact(DisplayName = "No579_正常系：正常動作の確認_selfにI2CAction.Writeを指定して、メソッドを呼び出す")]
    public void Case579()
    {
        var result = I2CActionExtentions.ToString(I2CAction.Write);

        Assert.Equal("write", result);
    }

    [Fact(DisplayName = "No580_異常系：異常動作の確認_selfに未定義の列挙値を指定して、メソッドを呼び出す")]
    public void Case580()
    {
        var self = (I2CAction)999;

        Assert.Throws<ArgumentException>(() => I2CActionExtentions.ToString(self));
    }
}
