using System.Text;
using DotNetty.Transport.Channels;
using Microsoft.VisualBasic;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.LoggerTest;

[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class LoggerFactory_GetLogger : LoggerBase
{
    public LoggerFactory_GetLogger(ITestOutputHelper output) : base(output) { }

    private readonly LoggerFactory _factry = new();

    private readonly Type type = typeof(LoggerFactory_GetLogger);
    
    [Fact(DisplayName = "LoggerFactory No001_引数typeに有効なクラス名を渡す")]
    public void LoggerFactory_Case001()
    {
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;
        var loggingClassName = (string)logger.GetType().GetProperty("LoggingClassName", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(logger)!;

        Assert.Equal(type.Name, loggingClassName);
    }

    [Fact(DisplayName = "LoggerFactory No002_引数typeにnullを渡す")]
    public void LoggerFactory_Case002()
    {
        Assert.ThrowsAny<Exception>(() => (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [null])!);
    }

    [Fact(DisplayName = "LoggerFactory No003_同一の型を複数回渡す")]
    public void LoggerFactory_Case003()
    {
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;
        var logger2 = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;

        Assert.NotEqual(logger, logger2);
    }

    [Fact(DisplayName = "LoggerFactory No004_有効なクラス型でILoggerを取得し、SetOutputLogLevelメソッドを呼び出す")]
    public void LoggerFactory_Case004()
    {
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;
        logger.SetOutputLogLevel(ILogger.LogLevel.DEBUG.ToString());

        Assert.Equal(ILogger.LogLevel.DEBUG, logger.OutputLogLevel);
        logger.SetOutputLogLevel(ILogger.LogLevel.INFO.ToString());
    }

    [Fact(DisplayName = "LoggerFactory No005_有効なクラス型でILoggerを取得し、SetModuleClientメソッドを呼び出す")]
    public void LoggerFactory_Case005()
    {
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;

        var myClient = CreateModuleClient();
        logger.SetModuleClient(myClient);

        Assert.Equal(myClient, logger.MyClient);
    }

    [Fact(DisplayName = "LoggerFactory No006_有効なクラス型でILoggerを取得し、SetOutputTextWriterメソッドを呼び出す", Skip = "StreamWriter解放により他のテストがエラーになってしまうため、スキップ")]
    public void LoggerFactory_Case006()
    {
        var filePath = "LoggerFactory_Case006.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;

        var encoding = Encoding.Unicode;
        using (var stream = new StreamWriter(filePath, true, encoding))
        {
            logger.SetOutputTextWriter(stream);
        }
        Assert.Equal(encoding, logger.OutTextWriter.Encoding);

        File.Delete(filePath);
    }

    [Fact(DisplayName = "LoggerFactory No007_有効なクラス型でILoggerを取得し、WriteLogメソッドを呼び出す")]
    public void LoggerFactory_Case007()
    {
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;
        logger.SetOutputLogLevel(ILogger.LogLevel.INFO.ToString());

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logLevel = ILogger.LogLevel.INFO;
        var logMessage = "message";
        logger.WriteLog(logLevel, logMessage);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        VerfyLogMessage(consoleOutput, GetTag(logLevel), type.Name, logMessage);
    }

    [Fact(DisplayName = "LoggerFactory No008_有効なクラス型でILoggerを取得し、SetMandatoryLogLevelメソッドを呼び出す")]
    public async void LoggerFactory_Case008()
    {
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;

        logger.SetOutputLogLevel(ILogger.LogLevel.INFO.ToString());
        _output.WriteLine($"before loglevel: {logger.OutputLogLevel}");

        int seconds = 10;
        logger.SetMandatoryLogLevel(ILogger.LogLevel.DEBUG.ToString(), seconds);

        Assert.Equal(ILogger.LogLevel.DEBUG, logger.OutputLogLevel);
        _output.WriteLine($"changed loglevel: {logger.OutputLogLevel}");
        var isMandatoryLoglevelPermanent = (bool)logger.GetType().GetField("isMandatoryLoglevelPermanent", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
        Assert.False(isMandatoryLoglevelPermanent);

        // 強制ログレベル解除待ち
        await Task.Delay(11000);
    }

    [Fact(DisplayName = "LoggerFactory No009_有効なクラス型でILoggerを取得し、IsLogLevelToOutputメソッドを呼び出す")]
    public void LoggerFactory_Case009()
    {
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;

        var logLevel = ILogger.LogLevel.INFO;
        logger.SetOutputLogLevel(logLevel.ToString());
        _output.WriteLine($"loglevel: {logger.OutputLogLevel}");

        var response = logger.IsLogLevelToOutput(logLevel);
        Assert.True(response);
    }
    
    [Fact(DisplayName = "LoggerFactory No010_環境変数：LogLevel=TRACE")]
    public void LoggerFactory_Case010()
    {
        var loggerFactoryType = _factry!.GetType();
        loggerFactoryType.GetField("isLoggerInitialized", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, false);

        Environment.SetEnvironmentVariable("LogLevel", "TRACE");
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;
        Assert.Equal(ILogger.LogLevel.TRACE, logger.OutputLogLevel);
        Environment.SetEnvironmentVariable("LogLevel", null);
        logger.SetOutputLogLevel(ILogger.LogLevel.INFO.ToString());
    }
    
    [Fact(DisplayName = "LoggerFactory No011_環境変数：LogLevel=空")]
    public void LoggerFactory_Case011()
    {
        var loggerFactoryType = _factry!.GetType();
        loggerFactoryType.GetField("isLoggerInitialized", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, false);

        Environment.SetEnvironmentVariable("LogLevel", null);
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;
        Assert.Equal(ILogger.LogLevel.INFO, logger.OutputLogLevel);
    }

    [Fact(DisplayName = "LoggerFactory No012_環境変数：LogLevel=不正値")]
    public void LoggerFactory_Case012()
    {
        var loggerFactoryType = _factry!.GetType();
        loggerFactoryType.GetField("isLoggerInitialized", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, false);

        Environment.SetEnvironmentVariable("LogLevel", "test");
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;
        Assert.Equal(ILogger.LogLevel.INFO, logger.OutputLogLevel);
        Environment.SetEnvironmentVariable("LogLevel", null);
    }
    
    [Fact(DisplayName = "LoggerFactory No013_初期化フラグ=True（環境変数より取得しない）")]
    public void LoggerFactory_Case013()
    {
        var loggerFactoryType = _factry!.GetType();
        loggerFactoryType.GetField("isLoggerInitialized", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, true);

        Environment.SetEnvironmentVariable("LogLevel", "warn");
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;
        Assert.Equal(ILogger.LogLevel.INFO, logger.OutputLogLevel);
        Environment.SetEnvironmentVariable("LogLevel", null);
    }
    
    [Fact(DisplayName = "LoggerFactory No014_初期化フラグ=false（環境変数より取得する）")]
    public void LoggerFactory_Case014()
    {
        var loggerFactoryType = _factry!.GetType();
        loggerFactoryType.GetField("isLoggerInitialized", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, false);

        Environment.SetEnvironmentVariable("LogLevel", "warn");
        var logger = (ILogger)_factry.GetType().GetMethod("GetLogger")!.Invoke(null, [type])!;
        Assert.Equal(ILogger.LogLevel.WARN, logger.OutputLogLevel);
        Environment.SetEnvironmentVariable("LogLevel", null);
        logger.SetOutputLogLevel(ILogger.LogLevel.INFO.ToString());
    }
}
