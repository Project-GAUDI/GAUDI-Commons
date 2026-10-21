using System.Reflection;
using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class Logger_TraceMethodExit
{
    private static readonly AsyncLocal<StringWriter?> CurrentWriter = new();

    [Fact(DisplayName = "No274_正常系：OutputLogLevel=TRACEの状態で、result・memberNameを指定して、メソッドを呼び出す")]
    public void Case274()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit(123, "Member");
        Assert.Contains("<7>", Output());
        Assert.Contains("[Member] Exit result: 123", Output());
    }

    [Fact(DisplayName = "No275_正常系：OutputLogLevel=INFOの状態で、メソッドを呼び出す")]
    public void Case275()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        logger.TraceMethodExit(123, "Member");
        Assert.Empty(Output());
    }

    [Fact(DisplayName = "No276_正常系：memberNameを指定せず、メソッドを呼び出す")]
    public void Case276()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit(123);
        Assert.Contains("[Case276] Exit result: 123", Output());
    }

    [Fact(DisplayName = "No277_正常系：memberNameにnullを指定して、メソッドを呼び出す")]
    public void Case277()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit(123, null!);

        var output = Output();
        Assert.DoesNotContain("[null]", output);
        Assert.DoesNotContain("[]", output);
        Assert.Contains("Exit result: 123", output);
    }

    [Fact(DisplayName = "No278_正常系：memberNameに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case278()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit(123, "Member");
        Assert.Contains("[Member] Exit result: 123", Output());
    }

    [Fact(DisplayName = "No279_正常系：resultを指定せず、メソッドを呼び出す")]
    public void Case279()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit(memberName: "Member");

        var output = Output();
        Assert.Contains("[Member] Exit", output);
        Assert.DoesNotContain("result:", output);
    }

    [Fact(DisplayName = "No280_正常系：resultにnullを指定して、メソッドを呼び出す")]
    public void Case280()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit(null, "Member");

        var output = Output();
        Assert.Contains("[Member] Exit", output);
        Assert.DoesNotContain("result:", output);
    }

    [Fact(DisplayName = "No281_正常系：resultにint型を指定して、メソッドを呼び出す")]
    public void Case281()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit(123, "Member");
        Assert.Contains("[Member] Exit result: 123", Output());
    }

    [Fact(DisplayName = "No282_正常系：resultにstring型を指定して、メソッドを呼び出す")]
    public void Case282()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit("text", "Member");
        Assert.Contains("[Member] Exit result: text", Output());
    }

    [Fact(DisplayName = "No283_正常系：resultにbool型を指定して、メソッドを呼び出す")]
    public void Case283()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit(true, "Member");
        Assert.Contains("[Member] Exit result: True", Output());
    }

    [Fact(DisplayName = "No284_正常系：resultにarray型を指定して、メソッドを呼び出す")]
    public void Case284()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit(new[] { 1, 2 }, "Member");
        Assert.Contains("[Member] Exit result:", Output());
        Assert.Contains("System.Int32", Output());
    }

    [Fact(DisplayName = "No285_正常系：resultにobject型を指定して、メソッドを呼び出す")]
    public void Case285()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit(new { Value = 1 }, "Member");
        Assert.Contains("[Member] Exit result: { Value = 1 }", Output());
    }

    [Fact(DisplayName = "No286_正常系：resultに改行（CRLF、CR、LF）・区切り文字（\"|\"）を含む文字列を指定して、メソッドを呼び出す")]
    public void Case286()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        logger.TraceMethodExit("first\r\nsecond\rthird\nfourth|part", "Member");

        var output = Output();
        Assert.Contains("[Member] Exit result: first second third fourthpart", output);
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
