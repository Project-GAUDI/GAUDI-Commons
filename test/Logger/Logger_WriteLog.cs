using System.Reflection;
using System.Text;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class Logger_WriteLog
{
    private static readonly AsyncLocal<StringWriter?> CurrentWriter = new();

    [Fact(DisplayName = "No138_正常系：正常動作の確認_OutputLogLevel=INFO、level=TRACE")]
    public void Case138()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        logger.WriteLog(ILogger.LogLevel.TRACE, "message");
        Assert.Empty(Output(logger));
    }

    [Fact(DisplayName = "No139_正常系：正常動作の確認_OutputLogLevel=INFO、level=DEBUG")]
    public void Case139()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        logger.WriteLog(ILogger.LogLevel.DEBUG, "message");
        Assert.Empty(Output(logger));
    }
    [Fact(DisplayName = "No140_正常系：正常動作の確認_OutputLogLevel=INFO、level=INFO")]
    public void Case140()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        logger.WriteLog(ILogger.LogLevel.INFO, "message");
        Assert.Contains("<5>", Output(logger));
    }
    [Fact(DisplayName = "No141_正常系：正常動作の確認_OutputLogLevel=INFO、level=WARN")]
    public void Case141()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        logger.WriteLog(ILogger.LogLevel.WARN, "message");
        Assert.Contains("<4>", Output(logger));
    }
    [Fact(DisplayName = "No142_正常系：正常動作の確認_OutputLogLevel=INFO、level=ERROR")]
    public void Case142()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        logger.WriteLog(ILogger.LogLevel.ERROR, "message");
        Assert.Contains("<3>", Output(logger));
    }
    [Fact(DisplayName = "No143_正常系：正常動作の確認_OutputLogLevel=TRACE、level=TRACE")]
    public void Case143()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WriteLog(ILogger.LogLevel.TRACE, "message");
        Assert.Contains("<7>", Output(logger));
    }
    [Fact(DisplayName = "No144_正常系：正常動作の確認_OutputLogLevel=TRACE、level=DEBUG")]
    public void Case144()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.WriteLog(ILogger.LogLevel.DEBUG, "message");
        Assert.Contains("<6>", Output(logger));
    }
    [Fact(DisplayName = "No145_正常系：正常動作の確認_messageに任意の文字列を指定する")]
    public void Case145()
    {
        var logger = NewLogger();
        logger.WriteLog(ILogger.LogLevel.INFO, "message");
        Assert.Contains("SpecClass", Output(logger));
        Assert.Contains("message", Output(logger));
    }
    [Fact(DisplayName = "No146_正常系：正常動作の確認_messageにnullを指定する")]
    public void Case146()
    {
        var logger = NewLogger();
        logger.WriteLog(ILogger.LogLevel.INFO, null!);
        Assert.Contains("SpecClass", Output(logger));
    }
    [Fact(DisplayName = "No147_正常系：正常動作の確認_messageに空文字を指定する")]
    public void Case147()
    {
        var logger = NewLogger();
        logger.WriteLog(ILogger.LogLevel.INFO, string.Empty);
        Assert.Contains("SpecClass", Output(logger));
    }
    [Fact(DisplayName = "No148_正常系：メッセージ形式の確認_messageにCRLFを含む")]
    public void Case148()
    {
        var logger = NewLogger();
        logger.WriteLog(ILogger.LogLevel.INFO, "first\r\nsecond");
        var lines = Output(logger).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.All(lines, line => Assert.Contains("[SpecClass]", line));
    }
    [Fact(DisplayName = "No149_正常系：メッセージ形式の確認_messageにCRを含む")]
    public void Case149()
    {
        var logger = NewLogger();
        logger.WriteLog(ILogger.LogLevel.INFO, "first\rsecond");
        var lines = Output(logger).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.All(lines, line => Assert.Contains("[SpecClass]", line));
    }
    [Fact(DisplayName = "No150_正常系：メッセージ形式の確認_messageにLFを含む")]
    public void Case150()
    {
        var logger = NewLogger();
        logger.WriteLog(ILogger.LogLevel.INFO, "first\nsecond");
        var lines = Output(logger).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.All(lines, line => Assert.Contains("[SpecClass]", line));
    }
    [Fact(DisplayName = "No151_異常系：異常動作の確認_クローズ済みTextWriterを設定する")]
    public void Case151()
    {
        var logger = NewLogger();
        logger.SetOutputTextWriter(new StringWriter());
        logger.OutTextWriter.Dispose();
        Assert.ThrowsAny<Exception>(() => logger.WriteLog(ILogger.LogLevel.INFO, "message"));
    }
    [Fact(DisplayName = "No152_正常系：正常動作の確認_isUploadを指定しない")]
    public void Case152()
    {
        var logger = NewLogger();
        logger.WriteLog(ILogger.LogLevel.INFO, "message");
        Assert.Null(logger.MyClient);
    }
    [Fact(DisplayName = "No153_正常系：正常動作の確認_isUpload=false")]
    public void Case153()
    {
        var logger = NewLogger();
        logger.WriteLog(ILogger.LogLevel.INFO, "message", false);
        Assert.Null(logger.MyClient);
    }
    [Fact(DisplayName = "No154_正常系：正常動作の確認_MyClient=nullでisUpload=true")]
    public void Case154()
    {
        var logger = NewLogger();
        logger.WriteLog(ILogger.LogLevel.INFO, "message", true);
        Assert.Contains("Can't upload log because Logger doesn't have module client.", Output(logger));
    }
    [Fact(DisplayName = "No155_正常系：正常動作の確認_未接続でisUpload=true")]
    public void Case155()
    {
        var logger = NewLogger();
        var client = new Mock<IModuleClient>();
        client.SetupGet(value => value.ConnectionStatus).Returns(IotConnectionStatus.Disconnected);
        logger.SetModuleClient(client.Object);
        logger.WriteLog(ILogger.LogLevel.INFO, "message", true);
        Assert.Contains("Can't upload log because Logger doesn't have module client.", Output(logger));
    }
    [Fact(DisplayName = "No156_正常系：正常動作の確認_送信完了まで待機する")]
    public void Case156()
    {
        var logger = NewLogger();
        var client = ConnectedClient();
        logger.SetModuleClient(client.Object);
        logger.WriteLog(ILogger.LogLevel.INFO, "message", true);
        SpinWait.SpinUntil(
            () => client.Invocations.Any(i => i.Method.Name == nameof(IModuleClient.SendEventAsync)),
            TimeSpan.FromSeconds(2));
        client.Verify(value => value.SendEventAsync("log", It.IsAny<IotMessage>()), Times.Once);
    }
    [Fact(DisplayName = "No157_異常系：異常動作の確認_SendEventAsyncがエラーになる")]
    public void Case157()
    {
        var logger = NewLogger();
        var client = ConnectedClient();
        client.Setup(value => value.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()))
            .ThrowsAsync(new InvalidOperationException("send failure"));
        logger.SetModuleClient(client.Object);
        logger.WriteLog(ILogger.LogLevel.INFO, "message", true);
        SpinWait.SpinUntil(() => Output(logger).Contains("Log upload failed"), TimeSpan.FromSeconds(2));
        Assert.Contains("Log upload failed", Output(logger));
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