using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.LoggerTest;

[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class Logger_Constructor : LoggerBase
{
    public Logger_Constructor(ITestOutputHelper output) : base(output) { }

    [Fact(DisplayName = "Logger No001_引数classNameに有効なクラス名を渡す")]
    public void Logger_Case001()
    {
        var className = "TestClass";
        var logger = new Logger(className);
        var loggingClassName = (string)logger.GetType().GetProperty("LoggingClassName", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(logger)!;

        Assert.Equal(className, loggingClassName);
    }

    [Fact(DisplayName = "Logger No002_引数classNameにnullを渡す")]
    public void Logger_Case002()
    {
        var logger = new Logger(null);
        var loggingClassName = (string?)logger.GetType().GetProperty("LoggingClassName", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(logger);

        Assert.Null(loggingClassName);
    }

    [Fact(DisplayName = "Logger No003_classNameに空文字を渡す")]
    public void Logger_Case003()
    {
        var className = "";
        var logger = new Logger(className);
        var loggingClassName = (string)logger.GetType().GetProperty("LoggingClassName", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(logger)!;

        Assert.Equal(className, loggingClassName);
    }
}