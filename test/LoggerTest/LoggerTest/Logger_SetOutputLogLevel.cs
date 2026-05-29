using System;
using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.LoggerTest;

[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class Logger_SetOutputLogLevel : LoggerBase
{
    #region 既存テストで利用
    //ILogger log = LoggerFactory.GetLogger(typeof(Logger_SetOutputLogLevel));
    #endregion

    public Logger_SetOutputLogLevel(ITestOutputHelper output) : base(output)
    {
        #region 既存テストで利用
        //log.SetOutputLogLevel("INFO");
        //log.SetMandatoryLogLevel("INFO", 0);
        #endregion
    }

    private readonly Logger _log = new();
/*
    [Fact(DisplayName = "Logger No004_起動直後のoutputLogLevelプロパティをチェック")]
    public void Logger_Case004()
    {        
        Assert.Equal(ILogger.LogLevel.INFO, _log.OutputLogLevel);
    }
*/
    [Fact(DisplayName = "Logger No005_引数logLevelに'TRACE'を渡す")]
    public void Logger_Case005()
    {
        var LogLevel = ILogger.LogLevel.TRACE;
        _log.SetOutputLogLevel(LogLevel.ToString());
        Assert.Equal(LogLevel, _log.OutputLogLevel);
    }

    [Fact(DisplayName = "Logger No006_引数logLevelに'DEBUG'を渡す")]
    public void Logger_Case006()
    {
        var LogLevel = ILogger.LogLevel.DEBUG;
        _log.SetOutputLogLevel(LogLevel.ToString());
        Assert.Equal(LogLevel, _log.OutputLogLevel);
    }

    [Fact(DisplayName = "Logger No007_引数logLevelに'INFO'を渡す")]
    public void Logger_Case007()
    {
        var LogLevel = ILogger.LogLevel.INFO;
        _log.SetOutputLogLevel(LogLevel.ToString());
        Assert.Equal(LogLevel, _log.OutputLogLevel);
    }

    [Fact(DisplayName = "Logger No008_引数logLevelに'WARN'を渡す")]
    public void Logger_Case008()
    {
        var LogLevel = ILogger.LogLevel.WARN;
        _log.SetOutputLogLevel(LogLevel.ToString());
        Assert.Equal(LogLevel, _log.OutputLogLevel);
    }

    [Fact(DisplayName = "Logger No009_引数logLevelに'ERROR'を渡す")]
    public void Logger_Case009()
    {
        var LogLevel = ILogger.LogLevel.ERROR;
        _log.SetOutputLogLevel(LogLevel.ToString());
        Assert.Equal(LogLevel, _log.OutputLogLevel);
    }

    [Fact(DisplayName = "Logger No010_引数logLevelに小文字'trace'を渡す")]
    public void Logger_Case010()
    {
        var LogLevel = ILogger.LogLevel.TRACE;
        _log.SetOutputLogLevel(LogLevel.ToString().ToLower());
        Assert.Equal(LogLevel, _log.OutputLogLevel);
    }

    [Fact(DisplayName = "Logger No011_引数logLevelに大文字小文字混在'InFo'を渡す")]
    public void Logger_Case011()
    {
        _log.SetOutputLogLevel("InFo");
        Assert.Equal(ILogger.LogLevel.INFO, _log.OutputLogLevel);
    }

    [Fact(DisplayName = "Logger No012_引数logLevelに前後にスペースを入れ' INFO 'を渡す")]
    public void Logger_Case012()
    {
        var LogLevel = ILogger.LogLevel.TRACE;
        _log.OutputLogLevel = LogLevel;

        Assert.Throws<ArgumentException>(() => _log.SetOutputLogLevel($" INFO "));
        Assert.Equal(LogLevel, _log.OutputLogLevel);
    }

    [Fact(DisplayName = "Logger No013_引数logLevelに、無効な文字'INVALID'を渡す")]
    public void Logger_Case013()
    {
        var LogLevel = ILogger.LogLevel.TRACE;
        _log.OutputLogLevel = LogLevel;

        Assert.Throws<ArgumentException>(() => _log.SetOutputLogLevel("INVALID"));
        Assert.Equal(LogLevel, _log.OutputLogLevel);
    }

    [Fact(DisplayName = "Logger No014_引数logLevelに空文字を渡す")]
    public void Logger_Case014()
    {
        var LogLevel = ILogger.LogLevel.TRACE;
        _log.OutputLogLevel = LogLevel;

        Assert.Throws<ArgumentException>(() => _log.SetOutputLogLevel(""));
        Assert.Equal(LogLevel, _log.OutputLogLevel);
    }

    [Fact(DisplayName = "Logger No015_引数logLevelにnullを渡す")]
    public void Logger_Case015()
    {
        var LogLevel = ILogger.LogLevel.TRACE;
        _log.OutputLogLevel = LogLevel;

        Assert.ThrowsAny<Exception>(() => _log.SetOutputLogLevel(null));
        Assert.Equal(LogLevel, _log.OutputLogLevel);
    }

/*
    #region 既存テスト
    //ログ出力レベル設定のテスト
    //規定の文字列以外が引数に渡されたときにエラーが発生することを確認する。
    //規定の文字列については、WriteLogTest_XXXの中で確認する。
    [Fact(DisplayName = "ログ出力レベル設定不正")]
    public void SetOutputLogLevelErrorTest()
    {


        Assert.Throws<ArgumentException>(() => log.SetOutputLogLevel("XXXX"));

    }
    #endregion
*/
}
