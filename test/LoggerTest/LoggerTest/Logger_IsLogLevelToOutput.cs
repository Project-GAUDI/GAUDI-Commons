using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.LoggerTest;

[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class Logger_IsLogLevelToOutput : LoggerBase
{

    #region 既存テストで利用
    //ILogger log = LoggerFactory.GetLogger(typeof(Logger_IsLogLevelToOutput));
    #endregion

    public Logger_IsLogLevelToOutput(ITestOutputHelper output) : base(output)
    {
        #region 既存テストで利用
        //log.SetOutputLogLevel("INFO");
        //log.SetMandatoryLogLevel("INFO", 0);
        #endregion
    }

    private readonly Logger _log = new();

    [Fact(DisplayName = "Logger No028_指定されたログレベルが等しい、INFO")]
    public void Logger_Case028()
    {
        var logLevel = ILogger.LogLevel.INFO;
        _log.SetOutputLogLevel(logLevel.ToString());
        _output.WriteLine($"loglevel: {_log.OutputLogLevel}");

        var response = _log.IsLogLevelToOutput(logLevel);
        Assert.True(response);
    }

    [Fact(DisplayName = "Logger No029_指定されたログレベルが高い、INFO < WARN")]
    public void Logger_Case029()
    {
        _log.SetOutputLogLevel(ILogger.LogLevel.INFO.ToString());
        _output.WriteLine($"loglevel: {_log.OutputLogLevel}");

        var response = _log.IsLogLevelToOutput(ILogger.LogLevel.WARN);
        Assert.True(response);
    }

    [Fact(DisplayName = "Logger No030_指定されたログレベルが低い、WARN > DEBUG")]
    public void Logger_Case030()
    {
        _log.SetOutputLogLevel(ILogger.LogLevel.WARN.ToString());
        _output.WriteLine($"loglevel: {_log.OutputLogLevel}");

        var response = _log.IsLogLevelToOutput(ILogger.LogLevel.DEBUG);
        Assert.False(response);
    }

    [Fact(DisplayName = "Logger No031_指定されたログレベルが最高レベル ERROR")]
    public void Logger_Case031()
    {
        var loglevel = ILogger.LogLevel.ERROR;
        _log.SetOutputLogLevel(loglevel.ToString());
        _output.WriteLine($"loglevel: {_log.OutputLogLevel}");

        var response = _log.IsLogLevelToOutput(loglevel);
        Assert.True(response);
    }

    [Fact(DisplayName = "Logger No032_指定されたログレベルが最低レベル TRACE")]
    public void Logger_Case032()
    {
        var loglevel = ILogger.LogLevel.TRACE;
        _log.SetOutputLogLevel(loglevel.ToString());
        _output.WriteLine($"loglevel: {_log.OutputLogLevel}");

        var response = _log.IsLogLevelToOutput(loglevel);
        Assert.True(response);
    }
/*
    #region 既存テスト

    //ログ出力可否判定テストその１
    //ログ出力可否判定がTrueの場合
    [Fact(DisplayName = "ログ出力可否判定1：True")]
    public void IsLogLevelToOutputTest_TrueReturned001()
    {
        log.SetOutputLogLevel("INFO");
        Assert.True(log.IsLogLevelToOutput(ILogger.LogLevel.WARN));
    }

    //ログ出力可否判定テストその２
    //ログ出力可否判定がTrueの場合（同値）
    [Fact(DisplayName = "ログ出力可否判定2：True")]
    public void IsLogLevelToOutputTest_TrueReturned002()
    {

        log.SetOutputLogLevel("INFO");
        Assert.True(log.IsLogLevelToOutput(ILogger.LogLevel.INFO));
    }

    //ログ出力可否判定テストその３
    //ログ出力可否判定がFalseの場合
    [Fact(DisplayName = "ログ出力可否判定3：False")]
    public void IsLogLevelToOutputTest_FalseReturned001()
    {
        log.SetOutputLogLevel("INFO");
        Assert.False(log.IsLogLevelToOutput(ILogger.LogLevel.TRACE));
    }
    #endregion
*/
}
