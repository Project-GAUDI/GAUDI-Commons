using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;
using System;

namespace TICO.GAUDI.Commons.Test.UtilitiesTest;

[Collection(nameof(I2CActionExtentions_ToString))]
public class I2CActionExtentions_ToString
{
    private readonly ITestOutputHelper _output;

    public I2CActionExtentions_ToString(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "I2CActionExtentions No001_Read に対する文字列変換のチェック")]
    public void I2CActionExtentions_Case001()
    {
        // Arrange
        var action = I2CAction.Read;

        // Act
        var result = I2CActionExtentions.ToString(action);

        // Assert
        Assert.Equal("read", result);
    }

    [Fact(DisplayName = "I2CActionExtentions No002_Write に対する文字列変換のチェック")]
    public void I2CActionExtentions_Case002()
    {
        // Arrange
        var action = I2CAction.Write;

        // Act
        var result = I2CActionExtentions.ToString(action);

        // Assert
        Assert.Equal("write", result);
    }

    [Fact(DisplayName = "I2CActionExtentions No003_Wait に対する文字列変換のチェック")]
    public void I2CActionExtentions_Case003()
    {
        // Arrange
        var action = I2CAction.Wait;

        // Act
        var result = I2CActionExtentions.ToString(action);

        // Assert
        Assert.Equal("wait", result);
    }

    [Fact(DisplayName = "I2CActionExtentions No004_未定義 に対する文字列変換のチェック")]
    public void I2CActionExtentions_Case004()
    {
        // Arrange
        var action = (I2CAction)3;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => I2CActionExtentions.ToString(action));
    }
}
