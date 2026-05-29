using Xunit;
using Xunit.Abstractions;
using System;

namespace TICO.GAUDI.Commons.Test.UtilitiesTest;

[Collection(nameof(Util_HexStringToByte))]
public class Util_HexStringToByte
{
    private readonly ITestOutputHelper _output;

    public Util_HexStringToByte(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Util No008_正しい16進文字列入力がある場合のチェック")]
    public void Util_Case008()
    {
        // Arrange
        string hex = "1F";

        // Act
        byte result = Util.HexStringToByte(hex);

        // Assert
        Assert.Equal(0x1F, result);
    }

    [Fact(DisplayName = "Util No009_入力文字列の長さが不適切な場合のチェック")]
    public void Util_Case009()
    {
        // Arrange
        string invalidHex = "1";

        // Act & Assert
        Assert.Throws<Exception>(() => Util.HexStringToByte(invalidHex));
    }

    [Fact(DisplayName = "Util No010_不正な16進文字列の場合のチェック")]
    public void Util_Case010()
    {
        // Arrange
        string invalidHex = "ZZ";

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => Util.HexStringToByte(invalidHex));
    }
}
