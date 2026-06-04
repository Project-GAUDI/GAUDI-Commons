using Xunit;
using Xunit.Abstractions;
using System;

namespace TICO.GAUDI.Commons.Test.UtilitiesTest;

[Collection(nameof(Util_HexStringToBytes))]
public class Util_HexStringToBytes
{
    private readonly ITestOutputHelper _output;

    public Util_HexStringToBytes(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Util No011_正しい16進文字列入力がある場合のチェック")]
    public void Util_Case011()
    {
        // Arrange
        string hexString = "1F-2A-3C";

        // Act
        byte[] result = Util.HexStringToBytes(hexString);

        // Assert
        Assert.Equal(new byte[] { 0x1F, 0x2A, 0x3C }, result);
    }

    [Fact(DisplayName = "Util No012_一部の16進文字列が不正な場合のチェック")]
    public void Util_Case012()
    {
        // Arrange
        string invalidHexString = "1F-ZZ-3C";

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => Util.HexStringToBytes(invalidHexString));
    }

    [Fact(DisplayName = "Util No013_入力文字列が空文字の場合のチェック")]
    public void Util_Case013()
    {
        // Arrange
        string emptyHex = ""; // 引数を空文字を設定

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => Util.HexStringToBytes(emptyHex));
    }

    [Fact(DisplayName = "Util No014_入力文字列がnullの場合のチェック")]
    public void Util_Case014()
    {
        // Arrange
        string? nullHex = null; // 引数をnullに設定

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => Util.HexStringToBytes(nullHex));
    }
}
