using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;
using System;
using System.Text;

namespace TICO.GAUDI.Commons.Test.UtilitiesTest;

[Collection(nameof(I2CCommandsMessage_DesirializeJson))]
public class I2CCommandsMessage_DesirializeJson
{
    private readonly ITestOutputHelper _output;

    public I2CCommandsMessage_DesirializeJson(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "I2CCommandsMessage No001_正しいJSON文字列をデシリアライズする")]
    public void I2CCommandsMessage_Case001()
    {
        // Arrange
        var validJson = "{" +
            "\"CommandList\": [" +
                "{" +
                    "\"Action\": \"Read\"," +
                    "\"Address\": \"0x1A\"," +
                    "\"Command\": \"CMD1\"," +
                    "\"Data\": \"FF-AA-BB-CC\"," +
                    "\"Filter\": \"None\"," +
                    "\"Length\": 4," +
                    "\"Interval\": 10" +
                "}," +
                "{" +
                    "\"Action\": \"Write\"," +
                    "\"Address\": \"0x1B\"," +
                    "\"Command\": \"CMD2\"," +
                    "\"Data\": \"AA-BB-CC-DD\"," +
                    "\"Filter\": \"High\"," +
                    "\"Length\": 8," +
                    "\"Interval\": 20" +
                "}" +
            "]" +
        "}";

        // Act
        var message = I2CCommandsMessage.DeserializeJson(validJson);

        // Assert
        Assert.NotNull(message); // デシリアライズ結果が null でないことを確認
        Assert.IsType<I2CCommandsMessage>(message); // デシリアライズされたオブジェクトが期待する型であることを確認
        Assert.NotNull(message.CommandList); // CommandList が正しく初期化されていることを確認
        Assert.Equal(2, message.CommandList.Count); // データの数が 2 件であることを確認

        // 1つ目のコマンドを検証
        var command1 = message.CommandList[0];
        Assert.Equal("Read", command1.Action);
        Assert.Equal("0x1A", command1.Address);
        Assert.Equal("CMD1", command1.Command);
        Assert.Equal("FF-AA-BB-CC", command1.Data);
        Assert.Equal("None", command1.Filter);
        Assert.Equal(4, command1.Length);
        Assert.Equal(10, command1.Interval);

        // 2つ目のコマンドを検証
        var command2 = message.CommandList[1];
        Assert.Equal("Write", command2.Action);
        Assert.Equal("0x1B", command2.Address);
        Assert.Equal("CMD2", command2.Command);
        Assert.Equal("AA-BB-CC-DD", command2.Data);
        Assert.Equal("High", command2.Filter);
        Assert.Equal(8, command2.Length);
        Assert.Equal(20, command2.Interval);
    }

    [Fact(DisplayName = "I2CCommandsMessage No002_空のCommandListが含まれた正しいJSON文字列をデシリアライズする")]
    public void I2CCommandsMessage_Case002()
    {
        // Arrange
        var emptyCommandListJson = "{ \"CommandList\": [] }";

        // Act
        var message = I2CCommandsMessage.DeserializeJson(emptyCommandListJson);

        // Assert
        Assert.NotNull(message);
        Assert.IsType<I2CCommandsMessage>(message); // デシリアライズされたオブジェクトが期待する型であることを確認
        Assert.NotNull(message.CommandList);
        Assert.Empty(message.CommandList); // 空のリストが正しくマッピングされることを確認する
    }

    [Fact(DisplayName = "I2CCommandsMessage No003_不正なJSON文字列を入力する")]
    public void I2CCommandsMessage_Case003()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act
        var message = I2CCommandsMessage.DeserializeJson(invalidJson);

        // Assert
        Assert.Null(message); // 不正な JSON は null を返すべき
    }

    [Fact(DisplayName = "I2CCommandsMessage No004_nullを入力する")]
    public void I2CCommandsMessage_Case004()
    {
        // Arrange
        string? nullJson = null;

        // Act
        var message = I2CCommandsMessage.DeserializeJson(nullJson);

        // Assert
        Assert.Null(message); // null の入力値に対しても null を返すべき
    }

    [Fact(DisplayName = "I2CCommandsMessage No005_正しいJSONバイト配列をデシリアライズする")]
    public void I2CCommandsMessage_Case005()
    {
        // Arrange
        byte[] input = Encoding.UTF8.GetBytes(
            "{" +
                "\"CommandList\": [" +
                    "{" +
                        "\"Action\": \"Read\"," +
                        "\"Address\": \"0x1A\"," +
                        "\"Command\": \"CMD1\"," +
                        "\"Data\": \"FF-AA-BB-CC\"," +
                        "\"Filter\": \"None\"," +
                        "\"Length\": 4," +
                        "\"Interval\": 10" +
                    "}," +
                    "{" +
                        "\"Action\": \"Write\"," +
                        "\"Address\": \"0x1B\"," +
                        "\"Command\": \"CMD2\"," +
                        "\"Data\": \"AA-BB-CC-DD\"," +
                        "\"Filter\": \"High\"," +
                        "\"Length\": 8," +
                        "\"Interval\": 20" +
                    "}" +
                "]" +
            "}"
        );

        // Act
        var message = I2CCommandsMessage.DeserializeJson(input);

        // Assert
        Assert.NotNull(message); // デシリアライズ結果が null でないことを確認
        Assert.IsType<I2CCommandsMessage>(message); // デシリアライズされたオブジェクトが期待する型であることを確認
        Assert.NotNull(message.CommandList); // CommandList が正しく初期化されていることを確認
        Assert.Equal(2, message.CommandList.Count); // データの数が期待通り 2 件であることを確認

        // 1つ目のデータを検証
        var command1 = message.CommandList[0];
        Assert.Equal("Read", command1.Action);
        Assert.Equal("0x1A", command1.Address);
        Assert.Equal("CMD1", command1.Command);
        Assert.Equal("FF-AA-BB-CC", command1.Data);
        Assert.Equal("None", command1.Filter);
        Assert.Equal(4, command1.Length);
        Assert.Equal(10, command1.Interval);

        // 2つ目のデータを検証
        var command2 = message.CommandList[1];
        Assert.Equal("Write", command2.Action);
        Assert.Equal("0x1B", command2.Address);
        Assert.Equal("CMD2", command2.Command);
        Assert.Equal("AA-BB-CC-DD", command2.Data);
        Assert.Equal("High", command2.Filter);
        Assert.Equal(8, command2.Length);
        Assert.Equal(20, command2.Interval);
    }

    [Fact(DisplayName = "I2CCommandsMessage No006_空のバイト配列をデシリアライズする")]
    public void I2CCommandsMessage_Case006()
    {
        // Arrange
        byte[] input = new byte[] { };

        // Act
        var message = I2CCommandsMessage.DeserializeJson(input);

        // Assert
        Assert.Null(message); // 空の入力に対しては null を返すべき
    }

    [Fact(DisplayName = "I2CCommandsMessage No007_不正なJSONバイト配列をデシリアライズする")]
    public void I2CCommandsMessage_Case007()
    {
        // Arrange
        byte[] invalidInput = new byte[] { 255, 255, 255 };

        // Act
        var invalidMessage = I2CCommandsMessage.DeserializeJson(invalidInput);

        // Assert
        Assert.Null(invalidMessage); // 不正な JSON は null を返すべき
    }

    [Fact(DisplayName = "I2CCommandsMessage No013_nullをデシリアライズする")]
    public void I2CCommandsMessage_Case013()
    {
        // Arrange
        byte[]? nullInput = null;

        // Act
        var nullMessage = I2CCommandsMessage.DeserializeJson(nullInput);

        // Assert
        Assert.Null(nullMessage); // null 入力は null を返すべき
    }
}