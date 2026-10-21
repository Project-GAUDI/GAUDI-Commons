using System.Reflection;
using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class Logger_TraceMethodEntry
{
    private static readonly AsyncLocal<StringWriter?> CurrentWriter = new();

    [Fact(DisplayName = "No261_正常系：OutputLogLevel=TRACEの状態で、args・memberNameを指定して、メソッドを呼び出す")]
    public void Case261()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry(123, "Member");
        Assert.Contains("<7>", Output());
        Assert.Contains("[Member] Enter args: 123", Output());
    }

    [Fact(DisplayName = "No262_正常系：OutputLogLevel=INFOの状態で、メソッドを呼び出す")]
    public void Case262()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        logger.TraceMethodEntry(123, "Member");
        Assert.Empty(Output());
    }

    [Fact(DisplayName = "No263_正常系：memberNameを指定せず、メソッドを呼び出す")]
    public void Case263()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry(123);
        Assert.Contains("[Case263] Enter args: 123", Output());
    }

    [Fact(DisplayName = "No264_正常系：memberNameにnullを指定して、メソッドを呼び出す")]
    public void Case264()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry(123, null!);

        var output = Output();
        Assert.DoesNotContain("[null]", output);
        Assert.DoesNotContain("[]", output);
        Assert.Contains("Enter args: 123", output);
    }

    [Fact(DisplayName = "No265_正常系：memberNameに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case265()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry(123, "Member");
        Assert.Contains("[Member] Enter args: 123", Output());
    }

    [Fact(DisplayName = "No266_正常系：argsを指定せず、メソッドを呼び出す")]
    public void Case266()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry(memberName: "Member");

        var output = Output();
        Assert.Contains("[Member] Enter", output);
        Assert.DoesNotContain("args:", output);
    }

    [Fact(DisplayName = "No267_正常系：argsにnullを指定して、メソッドを呼び出す")]
    public void Case267()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry(null, "Member");

        var output = Output();
        Assert.Contains("[Member] Enter", output);
        Assert.DoesNotContain("args:", output);
    }

    [Fact(DisplayName = "No268_正常系：argsにint型を指定して、メソッドを呼び出す")]
    public void Case268()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry(123, "Member");
        Assert.Contains("[Member] Enter args: 123", Output());
    }

    [Fact(DisplayName = "No269_正常系：argsにstring型を指定して、メソッドを呼び出す")]
    public void Case269()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry("text", "Member");
        Assert.Contains("[Member] Enter args: text", Output());
    }

    [Fact(DisplayName = "No270_正常系：argsにbool型を指定して、メソッドを呼び出す")]
    public void Case270()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry(true, "Member");
        Assert.Contains("[Member] Enter args: True", Output());
    }

    [Fact(DisplayName = "No271_正常系：argsにarray型を指定して、メソッドを呼び出す")]
    public void Case271()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry(new[] { 1, 2 }, "Member");
        Assert.Contains("[Member] Enter args:", Output());
        Assert.Contains("System.Int32", Output());
    }

    [Fact(DisplayName = "No272_正常系：argsにobject型を指定して、メソッドを呼び出す")]
    public void Case272()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry(new { Value = 1 }, "Member");
        Assert.Contains("[Member] Enter args: { Value = 1 }", Output());
    }

    [Fact(DisplayName = "No273_正常系：argsに改行（CRLF、CR、LF）・区切り文字（\"|\"）を含む文字列を指定して、メソッドを呼び出す")]
    public void Case273()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodEntry("first\r\nsecond\rthird\nfourth|part", "Member");

        var output = Output();
        Assert.Contains("[Member] Enter args: first second third fourthpart", output);
        Assert.DoesNotContain("first\r", output);
        Assert.DoesNotContain("second\r", output);
        Assert.DoesNotContain("third\n", output);
    }

    private static string Output()
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
}
