using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Runtime.Serialization.Json;
using System.IO;

namespace TICO.GAUDI.Commons.Test.UtilitiesTest;

[Collection(nameof(I2CCommandsMessage_SerializeJsonBytes))]
public class I2CCommandsMessage_SerializeJsonBytes
{
    private readonly ITestOutputHelper _output;

    public I2CCommandsMessage_SerializeJsonBytes(ITestOutputHelper output)
    {
        _output = output;
    }
            
    [Fact(DisplayName = "I2CCommandsMessage No011_有効な I2CCommandsMessage をバイト配列に変換する")]
    public void I2CCommandsMessage_Case011()
    {
        // Arrange
        var input = new I2CCommandsMessage
        {
            CommandList = new List<I2CCommand>
            {
                new I2CCommand
                {
                    Action = "Read",
                    Address = "0x1A",
                    Command = "CMD1",
                    Data = "FF-AA-BB-CC",
                    Filter = "None",
                    Length = 4,
                    Interval = 10
                },
                new I2CCommand
                {
                    Action = "Write",
                    Address = "0x1B",
                    Command = "CMD2",
                    Data = "AA-BB-CC-DD",
                    Filter = "High",
                    Length = 8,
                    Interval = 20
                }
            }
        };

        // 期待される JSON
        // I2CCoomandクラスのプロパティの順序が担保できない（昇順になる）ため、LengthとIntervalの順番を入れ替えたデータと比較
        var expectedJson = @"
        {
            ""CommandList"": [
                {
                    ""Action"": ""Read"",
                    ""Address"": ""0x1A"",
                    ""Command"": ""CMD1"",
                    ""Data"": ""FF-AA-BB-CC"",
                    ""Filter"": ""None"",
                    ""Interval"": 10,
                    ""Length"": 4
                },
                {
                    ""Action"": ""Write"",
                    ""Address"": ""0x1B"",
                    ""Command"": ""CMD2"",
                    ""Data"": ""AA-BB-CC-DD"",
                    ""Filter"": ""High"",
                    ""Interval"": 20,
                    ""Length"": 8
                }
            ]
        }";
        
        // JSON のフォーマットを標準化してバイト配列に変換
        var expectedJsonBytes = Encoding.UTF8.GetBytes(expectedJson.Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim());

        // Act
        var result = I2CCommandsMessage.SerializeJsonBytes(input);

        // Assert
        Assert.NotNull(result); // バイト配列が生成されたことを確認
        Assert.Equal(expectedJsonBytes, result); // バイト配列の内容が一致するかを確認
    }

    [Fact(DisplayName = "I2CCommandsMessage No012_null をバイト配列に変換する")]
    public void I2CCommandsMessage_Case012()
    {
        // Arrange
        I2CCommandsMessage? input = null;

        // Act
        var result = I2CCommandsMessage.SerializeJsonBytes(input);

        // Assert
        Assert.Null(result); // 入力が null の場合は null を返すべき
    }
}