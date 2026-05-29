using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;
using System;

namespace TICO.GAUDI.Commons.Test.UtilitiesTest;

[Collection(nameof(I2CActionExtentions_ToI2CAction))]
public class I2CActionExtentions_ToI2CAction
{
    private readonly ITestOutputHelper _output;

    public I2CActionExtentions_ToI2CAction(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "I2CActionExtentions No005_小文字のread変換のチェック")]
    public void I2CActionExtentions_Case005()
    {
        // Act
        var result = "read".ToI2CAction();

        // Assert
        Assert.Equal(I2CAction.Read, result);
    }

    [Fact(DisplayName = "I2CActionExtentions No006_大文字のREAD変換のチェック")]
    public void I2CActionExtentions_Case006()
    {
        // Act
        var result = "READ".ToI2CAction();

        // Assert
        Assert.Equal(I2CAction.Read, result);
    }

    [Fact(DisplayName = "I2CActionExtentions No007_混在のReAd変換のチェック")]
    public void I2CActionExtentions_Case007()
    {
        // Act
        var result = "ReAd".ToI2CAction();

        // Assert
        Assert.Equal(I2CAction.Read, result);
    }

    [Fact(DisplayName = "I2CActionExtentions No008_write変換のチェック")]
    public void I2CActionExtentions_Case008()
    {
        // Act
        var result = "write".ToI2CAction();

        // Assert
        Assert.Equal(I2CAction.Write, result);
    }

    [Fact(DisplayName = "I2CActionExtentions No009_wait変換のチェック")]
    public void I2CActionExtentions_Case009()
    {
        // Act
        var result = "wait".ToI2CAction();

        // Assert
        Assert.Equal(I2CAction.Wait, result);
    }

    [Fact(DisplayName = "I2CActionExtentions No010_無効な値変換のチェック")]
    public void I2CActionExtentions_Case010()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => "unknown".ToI2CAction());
    }

    [Fact(DisplayName = "I2CActionExtentions No011_null値変換のチェック")]
    public void I2CActionExtentions_Case011()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => ((string?)null).ToI2CAction());
    }

    [Fact(DisplayName = "I2CActionExtentions No012_空文字変換のチェック")]
    public void I2CActionExtentions_Case012()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => "".ToI2CAction());
    }
}
