using System.Reflection;
using System.Text;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;
namespace TICO.GAUDI.Commons.Test;
[Collection(nameof(SharedLoggerState))]
public sealed class Logger_Trace
{
    private static readonly AsyncLocal<StringWriter?> CurrentWriter = new();
    [Fact(DisplayName = "No158_正常系：正常動作の確認_OutputLogLevel=TRACEでTRACEログを出力する")]
    public void Case158()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.Trace("message", "Member", false);
        Assert.Contains("message", Output(logger));
        Assert.Contains("<7>", Output(logger));
    }
    [Fact(DisplayName = "No159_正常系：正常動作の確認_OutputLogLevel=INFOでTRACEログを出力しない")]
    public void Case159()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        logger.Trace("message", "Member", false);
        Assert.Empty(Output(logger));
    }
    [Fact(DisplayName = "No160_正常系：memberNameを指定せず、メソッドを呼び出す")]
    public void Case160()
    {
        var logger = NewLogger(); logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.Trace("message");
        Assert.Contains("[Case160]", Output(logger));
    }
    [Fact(DisplayName = "No161_正常系：memberNameにnullを指定して、メソッドを呼び出す")]
    public void Case161()
    {
        var logger = NewLogger(); logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.Trace("message", null!, false);
        Assert.DoesNotContain("[Member]", Output(logger));
        Assert.Contains("message", Output(logger));
    }
    [Fact(DisplayName = "No162_正常系：memberNameに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case162()
    {
        var logger = NewLogger(); logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.Trace("message", "Member", false);
        Assert.Contains("[Member]", Output(logger));
    }
    [Fact(DisplayName = "No163_正常系：isUploadを指定せず、メソッドを呼び出す")]
    public void Case163()
    {
        var logger = NewLogger();
        var client = ConnectedClient();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.SetModuleClient(client.Object);

        logger.Trace("message");

        client.Verify(value => value.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Never);
    }
    [Fact(DisplayName = "No164_正常系：isUpload=trueを指定して、メソッドを呼び出す")]
    public void Case164()
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
        logger.Trace("message", isUpload: true);

        SpinWait.SpinUntil(() => client.Invocations.Any(i => i.Method.Name == nameof(IModuleClient.SendEventAsync)), TimeSpan.FromSeconds(2));
        SpinWait.SpinUntil(() => sentMessage != null, TimeSpan.FromSeconds(2));
        client.Verify(value => value.SendEventAsync("log", It.IsAny<IotMessage>()), Times.Once);
        Assert.Equal("log", outputName);
        Assert.NotNull(sentMessage);
        Assert.Equal("log", sentMessage!.GetProperty("type"));
        Assert.Equal("7", sentMessage.GetProperty("level"));

        var record = JsonMessage.DeserializeRecordInfo(sentMessage.GetBodyString());
        Assert.NotNull(record);
        Assert.NotNull(record!.RecordHeader);
        Assert.Single(record.RecordHeader);
        Assert.NotEmpty(record.RecordHeader[0]);
        Assert.NotNull(record.RecordData);
        Assert.Single(record.RecordData);
        Assert.Contains("message", record.RecordData[0]?.ToString());
    }

    private static string Output(ILogger logger) => CurrentWriter.Value?.ToString() ?? string.Empty;

    private static ILogger NewLogger()
    {
        var loggerType = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
        foreach (var item in new[] { ("outputLogLevel", (object?)ILogger.LogLevel.INFO), ("mandatoryLoglevel", ILogger.LogLevel.INFO), ("isMandatoryLoglevelPermanent", false), ("mandatoryLoglevelTime", DateTime.Now.AddSeconds(-1)), ("myClient", null), ("outputName", "log") })
            loggerType.GetField(item.Item1, BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, item.Item2);
        var logger = (ILogger)Activator.CreateInstance(loggerType, new object[] { "SpecClass" })!;
        CurrentWriter.Value = new StringWriter(new StringBuilder()); logger.SetOutputTextWriter(CurrentWriter.Value); return logger;
    }

    private static Mock<IModuleClient> ConnectedClient()
    {
        var client = new Mock<IModuleClient>();
        client.SetupGet(value => value.ConnectionStatus).Returns(IotConnectionStatus.Connected);
        client.Setup(value => value.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>())).Returns(Task.CompletedTask);
        return client;
    }
}