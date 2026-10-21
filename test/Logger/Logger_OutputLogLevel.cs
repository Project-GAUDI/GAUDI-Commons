using System.Reflection;
using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class Logger_OutputLogLevel
{
    [Fact(DisplayName = "No117_正常系：正常動作の確認_isMandatoryLoglevelPermanent=trueの状態でメソッドを呼び出す")]
    public void Case117()
    {
        var logger = NewLogger();
        SetStatic("mandatoryLoglevel", ILogger.LogLevel.ERROR);
        SetStatic("isMandatoryLoglevelPermanent", true);
        Assert.Equal(ILogger.LogLevel.ERROR, logger.OutputLogLevel);
    }

    [Fact(DisplayName = "No118_正常系：正常動作の確認_強制ログレベルの期限内にメソッドを呼び出す")]
    public void Case118()
    {
        var logger = NewLogger();
        SetStatic("mandatoryLoglevel", ILogger.LogLevel.WARN);
        SetStatic("mandatoryLoglevelTime", DateTime.Now.AddSeconds(10));
        Assert.Equal(ILogger.LogLevel.WARN, logger.OutputLogLevel);
    }

    [Fact(DisplayName = "No119_正常系：正常動作の確認_強制ログレベルの期限切れ後にメソッドを呼び出す")]
    public void Case119()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.DEBUG;
        SetStatic("mandatoryLoglevelTime", DateTime.Now.AddSeconds(-1));
        Assert.Equal(ILogger.LogLevel.DEBUG, logger.OutputLogLevel);
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