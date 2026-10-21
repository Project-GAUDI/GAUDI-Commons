using System.Reflection;
using System.Text;
using TICO.GAUDI.Commons;
using Xunit;
namespace TICO.GAUDI.Commons.Test;
[Collection(nameof(SharedLoggerState))]
public sealed class Logger_SetMandatoryLogLevel
{
    [Fact(DisplayName = "No287_正常系：強制ログレベルを解除する")]
    public void Case287()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        logger.SetMandatoryLogLevel("ERROR", 0);
        Assert.Equal(ILogger.LogLevel.INFO, logger.OutputLogLevel);
    }
    [Fact(DisplayName = "No288_正常系：指定秒数の間TRACEレベルを強制する")]
    public void Case288()
    {
        var logger = NewLogger();
        logger.SetMandatoryLogLevel("TRACE", 10);
        Assert.Equal(ILogger.LogLevel.TRACE, logger.OutputLogLevel);
    }
    [Fact(DisplayName = "No289_正常系：強制ログレベルの期限切れ後に通常レベルへ戻る")]
    public void Case289()
    {
        var logger = NewLogger();
        logger.SetMandatoryLogLevel("TRACE", 1);
        SetStatic("mandatoryLoglevelTime", DateTime.Now.AddSeconds(-1));
        Assert.Equal(ILogger.LogLevel.INFO, logger.OutputLogLevel);
    }
    [Fact(DisplayName = "No290_正常系：強制ログレベルを無期限に設定する")]
    public void Case290()
    {
        var logger = NewLogger();
        logger.SetMandatoryLogLevel("DEBUG", -1);
        Assert.Equal(ILogger.LogLevel.DEBUG, logger.OutputLogLevel);
    }
    [Fact(DisplayName = "No291_異常系：不正な秒数を指定するとエラーになる")]
    public void Case291()
    {
        var logger = NewLogger();
        Assert.Throws<ArgumentException>(() => logger.SetMandatoryLogLevel("TRACE", -2));
    }
    [Fact(DisplayName = "No292_異常系：不正なログレベルを指定するとエラーになる")]
    public void Case292()
    {
        var logger = NewLogger();
        Assert.Throws<ArgumentException>(() => logger.SetMandatoryLogLevel("INVALID", 10));
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