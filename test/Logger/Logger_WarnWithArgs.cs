using System.Reflection;
using System.Text;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class Logger_WarnWithArgs
{
    private static readonly AsyncLocal<StringWriter?> CurrentWriter = new();

    [Fact(DisplayName = "No234_正常系：OutputLogLevel=TRACEの状態で、message・args・memberNameを指定して、メソッドを呼び出す")]
    public void Case234()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WarnWithArgs("message", 123, "Member", false);
        Assert.Contains("<4>", Output());
        Assert.Contains("[Member] message | args: 123", Output());
    }

    [Fact(DisplayName = "No235_正常系：OutputLogLevel=ERRORの状態で、メソッドを呼び出す")]
    public void Case235()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.ERROR;
        logger.WarnWithArgs("message", 123, "Member", false);
        Assert.Empty(Output());
    }

    [Fact(DisplayName = "No236_正常系：memberNameを指定せず、メソッドを呼び出す")]
    public void Case236()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WarnWithArgs("message", 123);
        Assert.Contains("[Case236]", Output());
    }

    [Fact(DisplayName = "No237_正常系：memberNameにnullを指定して、メソッドを呼び出す")]
    public void Case237()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WarnWithArgs("message", 123, null!, false);
        Assert.DoesNotContain("[null]", Output());
        Assert.DoesNotContain("[]", Output());
    }

    [Fact(DisplayName = "No238_正常系：memberNameに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case238()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WarnWithArgs("message", 123, "Member", false);
        Assert.Contains("[Member]", Output());
    }

    [Fact(DisplayName = "No239_正常系：argsにnullを指定して、メソッドを呼び出す")]
    public void Case239()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WarnWithArgs("message", null!, "Member", false);
        Assert.Contains("message | args: null", Output());
    }

    [Fact(DisplayName = "No240_正常系：argsにint型を指定して、メソッドを呼び出す")]
    public void Case240()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WarnWithArgs("message", 123, "Member", false);
        Assert.Contains("message | args: 123", Output());
    }

    [Fact(DisplayName = "No241_正常系：argsにstring型を指定して、メソッドを呼び出す")]
    public void Case241()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WarnWithArgs("message", "text", "Member", false);
        Assert.Contains("message | args: text", Output());
    }

    [Fact(DisplayName = "No242_正常系：argsにbool型を指定して、メソッドを呼び出す")]
    public void Case242()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WarnWithArgs("message", true, "Member", false);
        Assert.Contains("message | args: True", Output());
    }

    [Fact(DisplayName = "No243_正常系：argsにarray型を指定して、メソッドを呼び出す")]
    public void Case243()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WarnWithArgs("message", new[] { 1, 2 }, "Member", false);
        Assert.Contains("System.Int32", Output());
    }

    [Fact(DisplayName = "No244_正常系：argsにobject型を指定して、メソッドを呼び出す")]
    public void Case244()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WarnWithArgs("message", new { Value = 1 }, "Member", false);
        Assert.Contains("message | args: { Value = 1 }", Output());
    }

    [Fact(DisplayName = "No245_正常系：argsに改行（CRLF、CR、LF）・区切り文字（\"|\"）を含む文字列を指定して、メソッドを呼び出す")]
    public void Case245()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WarnWithArgs("message", "first\r\nsecond\rthird\nfourth|part", "Member", false);

        var output = Output();
        Assert.Contains("first second third fourthpart", output);
        Assert.DoesNotContain("first\r", output);
        Assert.DoesNotContain("second\r", output);
        Assert.DoesNotContain("third\n", output);
    }

    [Fact(DisplayName = "No246_正常系：isUploadを指定せず、メソッドを呼び出す")]
    public void Case246()
    {
        var logger = NewLogger();
        var client = ConnectedClient();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.SetModuleClient(client.Object);
        logger.WarnWithArgs("message", 123);
        client.Verify(value => value.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Never);
    }

    [Fact(DisplayName = "No247_正常系：isUpload=trueを指定して、メソッドを呼び出す")]
    public void Case247()
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
        logger.WarnWithArgs("message", 123, isUpload: true);

        SpinWait.SpinUntil(
            () => client.Invocations.Any(i => i.Method.Name == nameof(IModuleClient.SendEventAsync)),
            TimeSpan.FromSeconds(2));
        SpinWait.SpinUntil(() => sentMessage != null, TimeSpan.FromSeconds(2));
        client.Verify(value => value.SendEventAsync("log", It.IsAny<IotMessage>()), Times.Once);
        Assert.Equal("log", outputName);
        Assert.NotNull(sentMessage);
        Assert.Equal("log", sentMessage!.GetProperty("type"));
        Assert.Equal("4", sentMessage.GetProperty("level"));

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
