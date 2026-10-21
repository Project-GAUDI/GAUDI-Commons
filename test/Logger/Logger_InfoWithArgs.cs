using System.Reflection;
using System.Text;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class Logger_InfoWithArgs
{
    private static readonly AsyncLocal<StringWriter?> CurrentWriter = new();

    [Fact(DisplayName = "No220_正常系：OutputLogLevel=TRACEの状態で、message・args・memberNameを指定して、メソッドを呼び出す")]
    public void Case220()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.InfoWithArgs("message", 123, "Member", false);
        Assert.Contains("<5>", Output());
        Assert.Contains("[Member] message | args: 123", Output());
    }

    [Fact(DisplayName = "No221_正常系：OutputLogLevel=ERRORの状態で、メソッドを呼び出す")]
    public void Case221()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.ERROR;
        logger.InfoWithArgs("message", 123, "Member", false);
        Assert.Empty(Output());
    }

    [Fact(DisplayName = "No222_正常系：memberNameを指定せず、メソッドを呼び出す")]
    public void Case222()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.InfoWithArgs("message", 123);
        Assert.Contains("[Case222]", Output());
    }

    [Fact(DisplayName = "No223_正常系：memberNameにnullを指定して、メソッドを呼び出す")]
    public void Case223()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.InfoWithArgs("message", 123, null!, false);
        Assert.DoesNotContain("[null]", Output());
        Assert.DoesNotContain("[]", Output());
    }

    [Fact(DisplayName = "No224_正常系：memberNameに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case224()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.InfoWithArgs("message", 123, "Member", false);
        Assert.Contains("[Member]", Output());
    }

    [Fact(DisplayName = "No225_正常系：argsにnullを指定して、メソッドを呼び出す")]
    public void Case225()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.InfoWithArgs("message", null!, "Member", false);
        Assert.Contains("message | args: null", Output());
    }

    [Fact(DisplayName = "No226_正常系：argsにint型を指定して、メソッドを呼び出す")]
    public void Case226()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.InfoWithArgs("message", 123, "Member", false);
        Assert.Contains("message | args: 123", Output());
    }

    [Fact(DisplayName = "No227_正常系：argsにstring型を指定して、メソッドを呼び出す")]
    public void Case227()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.InfoWithArgs("message", "text", "Member", false);
        Assert.Contains("message | args: text", Output());
    }

    [Fact(DisplayName = "No228_正常系：argsにbool型を指定して、メソッドを呼び出す")]
    public void Case228()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.InfoWithArgs("message", true, "Member", false);
        Assert.Contains("message | args: True", Output());
    }

    [Fact(DisplayName = "No229_正常系：argsにarray型を指定して、メソッドを呼び出す")]
    public void Case229()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.InfoWithArgs("message", new[] { 1, 2 }, "Member", false);
        Assert.Contains("System.Int32", Output());
    }

    [Fact(DisplayName = "No230_正常系：argsにobject型を指定して、メソッドを呼び出す")]
    public void Case230()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.InfoWithArgs("message", new { Value = 1 }, "Member", false);
        Assert.Contains("message | args: { Value = 1 }", Output());
    }

    [Fact(DisplayName = "No231_正常系：argsに改行（CRLF、CR、LF）・区切り文字（\"|\"）を含む文字列を指定して、メソッドを呼び出す")]
    public void Case231()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.InfoWithArgs("message", "first\r\nsecond\rthird\nfourth|part", "Member", false);

        var output = Output();
        Assert.Contains("first second third fourthpart", output);
        Assert.DoesNotContain("first\r", output);
        Assert.DoesNotContain("second\r", output);
        Assert.DoesNotContain("third\n", output);
    }

    [Fact(DisplayName = "No232_正常系：isUploadを指定せず、メソッドを呼び出す")]
    public void Case232()
    {
        var logger = NewLogger();
        var client = ConnectedClient();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.SetModuleClient(client.Object);
        logger.InfoWithArgs("message", 123);
        client.Verify(value => value.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Never);
    }

    [Fact(DisplayName = "No233_正常系：isUpload=trueを指定して、メソッドを呼び出す")]
    public void Case233()
    {
        var logger = NewLogger();
        var client = new Mock<IModuleClient>();
        client.SetupGet(value => value.ConnectionStatus).Returns(IotConnectionStatus.Connected);
        string? outputName = null;
        IotMessage? sentMessage = null;
        client.Setup(value => value.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()))
            .Returns(Task.CompletedTask)
            .Callback<string, IotMessage>((name, message) =>
            {
                outputName = name;
                sentMessage = message;
            });

        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.SetModuleClient(client.Object);
        logger.InfoWithArgs("message", 123, isUpload: true);

        SpinWait.SpinUntil(
            () => client.Invocations.Any(i => i.Method.Name == nameof(IModuleClient.SendEventAsync)),
            TimeSpan.FromSeconds(2));
        SpinWait.SpinUntil(() => sentMessage != null, TimeSpan.FromSeconds(2));
        client.Verify(value => value.SendEventAsync("log", It.IsAny<IotMessage>()), Times.Once);
        Assert.Equal("log", outputName);
        Assert.NotNull(sentMessage);
        Assert.Equal("log", sentMessage!.GetProperty("type"));
        Assert.Equal("5", sentMessage.GetProperty("level"));

        var record = JsonMessage.DeserializeRecordInfo(sentMessage.GetBodyString());
        Assert.NotNull(record);
        Assert.NotNull(record!.RecordHeader);
        Assert.Single(record.RecordHeader);
        Assert.NotEmpty(record.RecordHeader[0]);
        Assert.NotNull(record.RecordData);
        Assert.Single(record.RecordData);
        Assert.Contains("message | args: 123", record.RecordData[0]?.ToString());
    }

    private static string Output()
    {
        return CurrentWriter.Value?.ToString() ?? string.Empty;
    }

    private static ILogger NewLogger()
    {
        var type = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
        foreach (var item in new[]
        {
            ("outputLogLevel", (object?)ILogger.LogLevel.INFO),
            ("mandatoryLoglevel", ILogger.LogLevel.INFO),
            ("isMandatoryLoglevelPermanent", false),
            ("mandatoryLoglevelTime", DateTime.Now.AddSeconds(-1)),
            ("myClient", null),
            ("outputName", "log")
        })
        {
            type.GetField(item.Item1, BindingFlags.Static | BindingFlags.NonPublic)!
                .SetValue(null, item.Item2);
        }

        var logger = (ILogger)Activator.CreateInstance(type, new object[] { "SpecClass" })!;
        CurrentWriter.Value = new StringWriter(new StringBuilder());
        logger.SetOutputTextWriter(CurrentWriter.Value);
        return logger;
    }

    private static Mock<IModuleClient> ConnectedClient()
    {
        var client = new Mock<IModuleClient>();
        client.SetupGet(value => value.ConnectionStatus).Returns(IotConnectionStatus.Connected);
        client.Setup(value => value.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()))
            .Returns(Task.CompletedTask);
        return client;
    }
}
