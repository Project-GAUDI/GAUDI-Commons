using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class LoggerFactory_GetLogger
{
    [Fact(DisplayName = "No306_正常系：isLoggerInitialized=trueの状態で、typeに任意の型を指定して、メソッドを呼び出す")]
    public void Case306()
    {
        SetFactoryInitialized(true);

        var logger = LoggerFactory.GetLogger(typeof(LoggerFactory_GetLogger));

        Assert.NotNull(logger);
        Assert.Equal("LoggerFactory_GetLogger", GetLoggingClassName(logger));
    }

    [Fact(DisplayName = "No307_異常系：isLoggerInitialized=trueの状態で、typeにnullを指定して、メソッドを呼び出す")]
    public void Case307()
    {
        SetFactoryInitialized(true);

        Assert.Throws<NullReferenceException>(() => LoggerFactory.GetLogger(null!));
    }

    [Fact(DisplayName = "No308_正常系：isLoggerInitialized=false、ENV:LogLevel=TRACEの状態でLoggerを取得する")]
    public void Case308()
    {
        SetFactoryInitialized(false);
        var originalLogLevel = Environment.GetEnvironmentVariable("LogLevel");
        Environment.SetEnvironmentVariable("LogLevel", "TRACE");

        try
        {
            var logger = LoggerFactory.GetLogger(typeof(LoggerFactory_GetLogger));
            Assert.NotNull(logger);
            Assert.Equal(ILogger.LogLevel.TRACE, logger.OutputLogLevel);
        }
        finally
        {
            Environment.SetEnvironmentVariable("LogLevel", originalLogLevel);
            SetFactoryInitialized(false);
        }
    }

    [Fact(DisplayName = "No309_正常系：isLoggerInitialized=false、ENV:LogLevel=DEBUGの状態でLoggerを取得する")]
    public void Case309()
    {
        SetFactoryInitialized(false);
        var originalLogLevel = Environment.GetEnvironmentVariable("LogLevel");
        Environment.SetEnvironmentVariable("LogLevel", "DEBUG");

        try
        {
            var logger = LoggerFactory.GetLogger(typeof(LoggerFactory_GetLogger));
            Assert.NotNull(logger);
            Assert.Equal(ILogger.LogLevel.DEBUG, logger.OutputLogLevel);
        }
        finally
        {
            Environment.SetEnvironmentVariable("LogLevel", originalLogLevel);
            SetFactoryInitialized(false);
        }
    }

    [Fact(DisplayName = "No310_正常系：isLoggerInitialized=false、ENV:LogLevel=INFOの状態でLoggerを取得する")]
    public void Case310()
    {
        SetFactoryInitialized(false);
        var originalLogLevel = Environment.GetEnvironmentVariable("LogLevel");
        Environment.SetEnvironmentVariable("LogLevel", "INFO");

        try
        {
            var logger = LoggerFactory.GetLogger(typeof(LoggerFactory_GetLogger));
            Assert.NotNull(logger);
            Assert.Equal(ILogger.LogLevel.INFO, logger.OutputLogLevel);
        }
        finally
        {
            Environment.SetEnvironmentVariable("LogLevel", originalLogLevel);
            SetFactoryInitialized(false);
        }
    }

    [Fact(DisplayName = "No311_正常系：isLoggerInitialized=false、ENV:LogLevel=WARNの状態でLoggerを取得する")]
    public void Case311()
    {
        SetFactoryInitialized(false);
        var originalLogLevel = Environment.GetEnvironmentVariable("LogLevel");
        Environment.SetEnvironmentVariable("LogLevel", "WARN");

        try
        {
            var logger = LoggerFactory.GetLogger(typeof(LoggerFactory_GetLogger));
            Assert.NotNull(logger);
            Assert.Equal(ILogger.LogLevel.WARN, logger.OutputLogLevel);
        }
        finally
        {
            Environment.SetEnvironmentVariable("LogLevel", originalLogLevel);
            SetFactoryInitialized(false);
        }
    }

    [Fact(DisplayName = "No312_正常系：isLoggerInitialized=false、ENV:LogLevel=ERRORの状態でLoggerを取得する")]
    public void Case312()
    {
        SetFactoryInitialized(false);
        var originalLogLevel = Environment.GetEnvironmentVariable("LogLevel");
        Environment.SetEnvironmentVariable("LogLevel", "ERROR");

        try
        {
            var logger = LoggerFactory.GetLogger(typeof(LoggerFactory_GetLogger));
            Assert.NotNull(logger);
            Assert.Equal(ILogger.LogLevel.ERROR, logger.OutputLogLevel);
        }
        finally
        {
            Environment.SetEnvironmentVariable("LogLevel", originalLogLevel);
            SetFactoryInitialized(false);
        }
    }

    [Fact(DisplayName = "No313_正常系：isLoggerInitialized=false、ENV:LogLevel未指定の状態でLoggerを取得する")]
    public void Case313()
    {
        SetFactoryInitialized(false);
        var originalLogLevel = Environment.GetEnvironmentVariable("LogLevel");
        Environment.SetEnvironmentVariable("LogLevel", null);

        try
        {
            var logger = LoggerFactory.GetLogger(typeof(LoggerFactory_GetLogger));
            Assert.NotNull(logger);
            Assert.Equal(ILogger.LogLevel.INFO, logger.OutputLogLevel);
        }
        finally
        {
            Environment.SetEnvironmentVariable("LogLevel", originalLogLevel);
            SetFactoryInitialized(false);
        }
    }

    [Fact(DisplayName = "No314_異常系：isLoggerInitialized=false、ENV:LogLevel=InValidの状態でLoggerを取得する")]
    public void Case314()
    {
        SetFactoryInitialized(false);
        var originalLogLevel = Environment.GetEnvironmentVariable("LogLevel");
        Environment.SetEnvironmentVariable("LogLevel", "InValid");

        try
        {
            var logger = LoggerFactory.GetLogger(typeof(LoggerFactory_GetLogger));
            Assert.NotNull(logger);
            Assert.Equal(ILogger.LogLevel.INFO, logger.OutputLogLevel);
        }
        finally
        {
            Environment.SetEnvironmentVariable("LogLevel", originalLogLevel);
            SetFactoryInitialized(false);
        }
    }

    [Fact(DisplayName = "No315_異常系：typeにnullを指定して、メソッドを呼び出す")]
    public void Case315()
    {
        SetFactoryInitialized(true);

        Assert.Throws<NullReferenceException>(() => LoggerFactory.GetLogger(null!));
    }

    private static void SetFactoryInitialized(bool initialized)
    {
        typeof(LoggerFactory)
            .GetField("isLoggerInitialized", BindingFlags.Static | BindingFlags.NonPublic)!
            .SetValue(null, initialized);
    }

    private static string GetLoggingClassName(ILogger logger)
    {
        var type = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
        return (string)type
            .GetProperty("LoggingClassName", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(logger)!;
    }
}
