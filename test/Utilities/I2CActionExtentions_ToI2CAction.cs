using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class I2CActionExtentions_ToI2CAction
{
    [Fact(DisplayName = "No581_正常系：正常動作の確認_selfに\"read\"を指定して、メソッドを呼び出す")]
    public void Case581()
    {
        var result = "read".ToI2CAction();

        Assert.Equal(I2CAction.Read, result);
    }

    [Fact(DisplayName = "No582_正常系：正常動作の確認_selfに\"wait\"を指定して、メソッドを呼び出す")]
    public void Case582()
    {
        var result = "wait".ToI2CAction();

        Assert.Equal(I2CAction.Wait, result);
    }

    [Fact(DisplayName = "No583_正常系：正常動作の確認_selfに\"write\"を指定して、メソッドを呼び出す")]
    public void Case583()
    {
        var result = "write".ToI2CAction();

        Assert.Equal(I2CAction.Write, result);
    }

    [Fact(DisplayName = "No584_正常系：正常動作の確認_selfに\"READ\"を指定して、メソッドを呼び出す")]
    public void Case584()
    {
        var result = "READ".ToI2CAction();

        Assert.Equal(I2CAction.Read, result);
    }

    [Fact(DisplayName = "No585_異常系：異常動作の確認_selfに\"Invalid\"を指定して、メソッドを呼び出す")]
    public void Case585()
    {
        Assert.Throws<ArgumentException>(() => "Invalid".ToI2CAction());
    }

    [Fact(DisplayName = "No586_異常系：異常動作の確認_selfにnullを指定して、メソッドを呼び出す")]
    public void Case586()
    {
        string? self = null;

        Assert.Throws<ArgumentException>(() => self!.ToI2CAction());
    }
}
