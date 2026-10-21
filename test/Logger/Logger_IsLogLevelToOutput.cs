using System.Reflection;
using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class Logger_IsLogLevelToOutput
{
    [Fact(DisplayName = "No293_正常系：OutputLogLevel=TRACEの状態で、level=TRACEを指定して、メソッドを呼び出す")]
    public void Case293()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.TRACE);
        Assert.True(result);
    }

    [Fact(DisplayName = "No294_正常系：OutputLogLevel=TRACEの状態で、level=DEBUGを指定して、メソッドを呼び出す")]
    public void Case294()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.TRACE;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.DEBUG);
        Assert.True(result);
    }

    [Fact(DisplayName = "No295_正常系：OutputLogLevel=DEBUGの状態で、level=TRACEを指定して、メソッドを呼び出す")]
    public void Case295()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.DEBUG;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.TRACE);
        Assert.False(result);
    }

    [Fact(DisplayName = "No296_正常系：OutputLogLevel=DEBUGの状態で、level=DEBUGを指定して、メソッドを呼び出す")]
    public void Case296()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.DEBUG;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.DEBUG);
        Assert.True(result);
    }

    [Fact(DisplayName = "No297_正常系：OutputLogLevel=DEBUGの状態で、level=INFOを指定して、メソッドを呼び出す")]
    public void Case297()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.DEBUG;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.INFO);
        Assert.True(result);
    }

    [Fact(DisplayName = "No298_正常系：OutputLogLevel=INFOの状態で、level=DEBUGを指定して、メソッドを呼び出す")]
    public void Case298()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.DEBUG);
        Assert.False(result);
    }

    [Fact(DisplayName = "No299_正常系：OutputLogLevel=INFOの状態で、level=INFOを指定して、メソッドを呼び出す")]
    public void Case299()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.INFO);
        Assert.True(result);
    }

    [Fact(DisplayName = "No300_正常系：OutputLogLevel=INFOの状態で、level=WARNを指定して、メソッドを呼び出す")]
    public void Case300()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.INFO;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.WARN);
        Assert.True(result);
    }

    [Fact(DisplayName = "No301_正常系：OutputLogLevel=WARNの状態で、level=INFOを指定して、メソッドを呼び出す")]
    public void Case301()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.WARN;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.INFO);
        Assert.False(result);
    }

    [Fact(DisplayName = "No302_正常系：OutputLogLevel=WARNの状態で、level=WARNを指定して、メソッドを呼び出す")]
    public void Case302()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.WARN;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.WARN);
        Assert.True(result);
    }

    [Fact(DisplayName = "No303_正常系：OutputLogLevel=WARNの状態で、level=ERRORを指定して、メソッドを呼び出す")]
    public void Case303()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.WARN;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.ERROR);
        Assert.True(result);
    }

    [Fact(DisplayName = "No304_正常系：OutputLogLevel=ERRORの状態で、level=WARNを指定して、メソッドを呼び出す")]
    public void Case304()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.ERROR;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.WARN);
        Assert.False(result);
    }

    [Fact(DisplayName = "No305_正常系：OutputLogLevel=ERRORの状態で、level=ERRORを指定して、メソッドを呼び出す")]
    public void Case305()
    {
        var logger = NewLogger();
        logger.OutputLogLevel = ILogger.LogLevel.ERROR;
        var result = logger.IsLogLevelToOutput(ILogger.LogLevel.ERROR);
        Assert.True(result);
    }

    private static ILogger NewLogger()
    {
        var loggerType = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
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
            loggerType.GetField(item.Item1, BindingFlags.Static | BindingFlags.NonPublic)!
                .SetValue(null, item.Item2);
        }

        var logger = (ILogger)Activator.CreateInstance(loggerType, new object[] { "SpecClass" })!;
        logger.SetOutputTextWriter(new StringWriter(new StringBuilder()));
        return logger;
    }
}
