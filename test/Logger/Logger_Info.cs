using System.Reflection;
using System.Text;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;
namespace TICO.GAUDI.Commons.Test;
[Collection(nameof(SharedLoggerState))]
public sealed class Logger_Info
{
    private static readonly AsyncLocal<StringWriter?> CurrentWriter = new();
    [Fact(DisplayName = "No172_正常系：正常動作の確認_OutputLogLevel=TRACEの状態で、メソッドを呼び出す")]
    public void Case172()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.Info("message", "Member", false);
        Assert.Contains("<5>", Output(logger));
        Assert.Contains("message", Output(logger));
    }

    [Fact(DisplayName = "No173_正常系：正常動作の確認_OutputLogLevel=ERRORの状態で、メソッドを呼び出す")]
    public void Case173()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.ERROR;
        logger.Info("message", "Member", false);
        Assert.Empty(Output(logger));
    }

    [Fact(DisplayName = "No174_正常系：memberNameを指定せず、メソッドを呼び出す")]
    public void Case174()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.Info("message");
        Assert.Contains("[Case174]", Output(logger));
    }

    [Fact(DisplayName = "No175_正常系：memberNameにnullを指定して、メソッドを呼び出す")]
    public void Case175()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.Info("message", null!, false);
        Assert.DoesNotContain("[null]", Output(logger));
        Assert.DoesNotContain("[]", Output(logger));
    }

    [Fact(DisplayName = "No176_正常系：memberNameに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case176()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.Info("message", "Member", false);
        Assert.Contains("[Member]", Output(logger));
    }

    [Fact(DisplayName = "No177_正常系：isUploadを指定せず、メソッドを呼び出す")]
    public void Case177()
    {
        var logger = NewLogger();
        var client = ConnectedClient();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.SetModuleClient(client.Object);
        logger.Info("message");
        client.Verify(value => value.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Never);
    }

    [Fact(DisplayName = "No178_正常系：isUpload=trueを指定して、メソッドを呼び出す")]
    public void Case178()
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
        logger.Info("message", isUpload: true);
        SpinWait.SpinUntil(() => client.Invocations.Any(i => i.Method.Name == nameof(IModuleClient.SendEventAsync)), TimeSpan.FromSeconds(2));
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
        Assert.Contains("message", record.RecordData[0]?.ToString());
    }

    private static string Output(ILogger logger)
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
            type.GetField(item.Item1, BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, item.Item2);
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