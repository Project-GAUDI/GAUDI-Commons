using System.Reflection;
using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class Logger_SetOutputLogLevel
{
    [Fact(DisplayName = "No122_正常系：正常動作の確認_logLevel=trace")]
    public void Case122() => AssertLevel("trace", ILogger.LogLevel.TRACE);
    [Fact(DisplayName = "No123_正常系：正常動作の確認_logLevel=TRACE")]
    public void Case123() => AssertLevel("TRACE", ILogger.LogLevel.TRACE);
    [Fact(DisplayName = "No124_正常系：正常動作の確認_logLevel=debug")]
    public void Case124() => AssertLevel("debug", ILogger.LogLevel.DEBUG);
    [Fact(DisplayName = "No125_正常系：正常動作の確認_logLevel=DEBUG")]
    public void Case125() => AssertLevel("DEBUG", ILogger.LogLevel.DEBUG);
    [Fact(DisplayName = "No126_正常系：正常動作の確認_logLevel=info")]
    public void Case126() => AssertLevel("info", ILogger.LogLevel.INFO);
    [Fact(DisplayName = "No127_正常系：正常動作の確認_logLevel=INFO")]
    public void Case127() => AssertLevel("INFO", ILogger.LogLevel.INFO);
    [Fact(DisplayName = "No128_正常系：正常動作の確認_logLevel=warn")]
    public void Case128() => AssertLevel("warn", ILogger.LogLevel.WARN);
    [Fact(DisplayName = "No129_正常系：正常動作の確認_logLevel=WARN")]
    public void Case129() => AssertLevel("WARN", ILogger.LogLevel.WARN);
    [Fact(DisplayName = "No130_正常系：正常動作の確認_logLevel=error")]
    public void Case130() => AssertLevel("error", ILogger.LogLevel.ERROR);
    [Fact(DisplayName = "No131_正常系：正常動作の確認_logLevel=ERROR")]
    public void Case131() => AssertLevel("ERROR", ILogger.LogLevel.ERROR);
    [Fact(DisplayName = "No132_異常系：異常動作の確認_logLevelに空文字を指定する")]
    public void Case132()
    {
        var logger = NewLogger();
        Assert.Throws<ArgumentException>(() => logger.SetOutputLogLevel(string.Empty));
    }
    [Fact(DisplayName = "No133_異常系：異常動作の確認_logLevelに不正値を指定する")]
    public void Case133()
    {
        var logger = NewLogger();
        Assert.Throws<ArgumentException>(() => logger.SetOutputLogLevel("invalid"));
    }

    private static void AssertLevel(string value, ILogger.LogLevel expected)
    {
        var logger = NewLogger();
        logger.SetOutputLogLevel(value);
        Assert.Equal(expected, logger.OutputLogLevel);
    }

    private static ILogger NewLogger()
    {
        var loggerType = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
        SetStatic("outputLogLevel", ILogger.LogLevel.INFO);
        SetStatic("mandatoryLoglevel", ILogger.LogLevel.INFO);
        SetStatic("isMandatoryLoglevelPermanent", false);
        SetStatic("mandatoryLoglevelTime", DateTime.Now.AddSeconds(-1));
        SetStatic("myClient", null);
        SetStatic("outputName", "log");
        var logger = (ILogger)Activator.CreateInstance(loggerType, new object[] { "SpecClass" })!;
        logger.SetOutputTextWriter(new StringWriter(new StringBuilder()));
        return logger;
    }

    private static void SetStatic(string name, object? value)
    {
        var loggerType = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
        loggerType.GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, value);
    }
}