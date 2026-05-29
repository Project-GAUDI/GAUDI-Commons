using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;
using System;
using System.Collections.Generic;

namespace TICO.GAUDI.Commons.Test.UtilitiesTest;

[Collection(nameof(I2CCommandsMessage_SerializeJson))]
public class I2CCommandsMessage_SerializeJson
{
    private readonly ITestOutputHelper _output;

    public I2CCommandsMessage_SerializeJson(ITestOutputHelper output)
    {
        _output = output;
    }
            
    [Fact(DisplayName = "I2CCommandsMessage No008_有効な I2CCommandsMessage オブジェクトをシリアライズする")]
    public void I2CCommandsMessage_Case008()
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

        // Act
        var result = I2CCommandsMessage.SerializeJson(input);

        // Assert
        Assert.NotNull(result); // 結果が null でないことを確認

        // 期待されるJSON文字列をオブジェクトに変換
        var expectedObject = new I2CCommandsMessage
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

        // 結果をデシリアライズしてオブジェクトとして比較
        var resultObject = I2CCommandsMessage.DeserializeJson(result);
        Assert.NotNull(resultObject);
        Assert.Equal(expectedObject.CommandList.Count, resultObject.CommandList.Count);

        for (int i = 0; i < expectedObject.CommandList.Count; i++)
        {
            Assert.Equal(expectedObject.CommandList[i].Action, resultObject.CommandList[i].Action);
            Assert.Equal(expectedObject.CommandList[i].Address, resultObject.CommandList[i].Address);
            Assert.Equal(expectedObject.CommandList[i].Command, resultObject.CommandList[i].Command);
            Assert.Equal(expectedObject.CommandList[i].Data, resultObject.CommandList[i].Data);
            Assert.Equal(expectedObject.CommandList[i].Filter, resultObject.CommandList[i].Filter);
            Assert.Equal(expectedObject.CommandList[i].Length, resultObject.CommandList[i].Length);
            Assert.Equal(expectedObject.CommandList[i].Interval, resultObject.CommandList[i].Interval);
        }
    }

    [Fact(DisplayName = "I2CCommandsMessage No009_空の I2CCommandsMessage をシリアライズする")]
    public void I2CCommandsMessage_Case009()
    {
        // Arrange
        var input = new I2CCommandsMessage { CommandList = new List<I2CCommand>() };

        // Act
        var result = I2CCommandsMessage.SerializeJson(input);

        // Assert
        Assert.NotNull(result); // 結果が null でないことを確認

        // 期待されるオブジェクト
        var expectedObject = new I2CCommandsMessage { CommandList = new List<I2CCommand>() };

        // 結果をデシリアライズしてオブジェクトとして比較
        var resultObject = I2CCommandsMessage.DeserializeJson(result);
        Assert.NotNull(resultObject);

        // CommandList の内容が一致していることを確認
        Assert.Equal(expectedObject.CommandList.Count, resultObject.CommandList.Count);
    }

    [Fact(DisplayName = "I2CCommandsMessage No010_null をシリアライズする")]
    public void I2CCommandsMessage_Case010()
    {
        // Arrange
        I2CCommandsMessage? input = null;

        // Act
        var result = I2CCommandsMessage.SerializeJson(input);

        // Assert
        Assert.Null(result); // null を返すべき
    }
}