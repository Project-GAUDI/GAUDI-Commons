using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class Logger_GetLogger
{
    [Fact(DisplayName = "No120_正常系：正常動作の確認_typeに任意の型を指定してメソッドを呼び出す")]
    public void Case120()
    {
        var created = LoggerFactory.GetLogger(typeof(Logger_GetLogger));
        Assert.NotNull(created);
        Assert.Equal("Logger_GetLogger", GetLoggingClassName(created));
    }

    [Fact(DisplayName = "No121_異常系：異常動作の確認_typeにnullを指定してメソッドを呼び出す")]
    public void Case121()
    {
        Assert.Throws<NullReferenceException>(() => LoggerFactory.GetLogger(null!));
    }

    private static string GetLoggingClassName(ILogger logger)
    {
        var type = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
        return (string)type.GetProperty("LoggingClassName", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(logger)!;
    }
}