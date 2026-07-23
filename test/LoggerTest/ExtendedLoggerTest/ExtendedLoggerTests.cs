using System;
using System.IO;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public class LoggerTests
{
    [Fact]
    public void Constructor_CreatesInstanceWithDefaultLevel()
    {
        var logger = new Logger(nameof(LoggerTests));

        logger.SetMandatoryLogLevel("INFO", ILogger.SecondsDefinition_CANCEL);
        logger.SetOutputLogLevel("INFO");

        Assert.Equal(ILogger.LogLevel.INFO, logger.OutputLogLevel);
    }

    [Fact]
    public void Info_AddsCallerMemberNameToMessage()
    {
        var logger = CreateLogger(ILogger.LogLevel.INFO, out var writer);

        logger.Info("message");

        string output = writer.ToString();
        Assert.Contains("<5>", output);
        Assert.Contains("[LoggerTests][Info_AddsCallerMemberNameToMessage] message", output);
    }

    [Theory]
    [InlineData(ILogger.LogLevel.TRACE, "<7>")]
    [InlineData(ILogger.LogLevel.DEBUG, "<6>")]
    [InlineData(ILogger.LogLevel.WARN, "<4>")]
    [InlineData(ILogger.LogLevel.ERROR, "<3>")]
    public void BasicMethods_MapRemainingLevels(ILogger.LogLevel level, string expectedTag)
    {
        var logger = CreateLogger(ILogger.LogLevel.TRACE, out var writer);

        WriteBasicLog(logger, level, "Member");

        string output = writer.ToString();
        Assert.Contains(expectedTag, output);
        Assert.Contains("[LoggerTests][Member] message", output);
    }

    [Fact]
    public void InfoWithArgs_FormatsAndSanitizesArguments()
    {
        var logger = CreateLogger(ILogger.LogLevel.INFO, out var writer);

        logger.InfoWithArgs("message", new { Text = "a|b\r\nc", Count = 2 });

        string output = writer.ToString();
        Assert.Contains("[LoggerTests][InfoWithArgs_FormatsAndSanitizesArguments] message | args: { Text = ab c, Count = 2 }", output);
    }

    [Theory]
    [InlineData(ILogger.LogLevel.TRACE, "<7>")]
    [InlineData(ILogger.LogLevel.DEBUG, "<6>")]
    [InlineData(ILogger.LogLevel.WARN, "<4>")]
    [InlineData(ILogger.LogLevel.ERROR, "<3>")]
    public void WithArgsMethods_MapRemainingLevels(ILogger.LogLevel level, string expectedTag)
    {
        var logger = CreateLogger(ILogger.LogLevel.TRACE, out var writer);

        WriteLogWithArgs(logger, level, new { Text = "a|b\r\nc", Count = 2 }, "Member");

        string output = writer.ToString();
        Assert.Contains(expectedTag, output);
        Assert.Contains("[LoggerTests][Member] message | args: { Text = ab c, Count = 2 }", output);
    }

    [Fact]
    public void TraceMethodEntry_DoesNotWriteWhenTraceIsDisabled()
    {
        var logger = CreateLogger(ILogger.LogLevel.INFO, out var writer);

        logger.TraceMethodEntry(new { Value = 1 });

        Assert.Equal(string.Empty, writer.ToString());
    }

    [Fact]
    public void TraceMethodEntryAndExit_WriteWhenTraceIsEnabled()
    {
        var logger = CreateLogger(ILogger.LogLevel.TRACE, out var writer);

        logger.TraceMethodEntry(new { Value = 1 }, "EntryMember");
        logger.TraceMethodExit(new { Value = 2 }, "ExitMember");

        string output = writer.ToString();
        Assert.Contains("<7>", output);
        Assert.Contains("[LoggerTests][EntryMember] Enter args: { Value = 1 }", output);
        Assert.Contains("[LoggerTests][ExitMember] Exit result: { Value = 2 }", output);
    }

    [Fact]
    public void LoggerContract_AppliesConfiguration()
    {
        var logger = CreateLogger(ILogger.LogLevel.INFO, out var writer);
        var moduleClient = new StubModuleClient();

        logger.SetOutputLogLevel("DEBUG");
        logger.SetModuleClient(moduleClient);
        logger.SetMandatoryLogLevel("TRACE", ILogger.SecondsDefinition_UNLIMITED);

        Assert.Equal(ILogger.LogLevel.TRACE, logger.OutputLogLevel);
        Assert.Same(moduleClient, logger.MyClient);

        logger.SetMandatoryLogLevel("INFO", ILogger.SecondsDefinition_CANCEL);
        Assert.Equal(ILogger.LogLevel.DEBUG, logger.OutputLogLevel);

        logger.Debug("delegation-check", "Member");
        Assert.Contains("[LoggerTests][Member] delegation-check", writer.ToString());
    }

    private static Logger CreateLogger(ILogger.LogLevel outputLevel, out StringWriter writer)
    {
        writer = new StringWriter();
        var logger = new Logger(nameof(LoggerTests));
        logger.SetMandatoryLogLevel("INFO", ILogger.SecondsDefinition_CANCEL);
        logger.SetOutputTextWriter(writer);
        logger.OutputLogLevel = outputLevel;
        return logger;
    }

    private static void WriteBasicLog(Logger logger, ILogger.LogLevel level, string memberName)
    {
        switch (level)
        {
            case ILogger.LogLevel.TRACE:
                logger.Trace("message", memberName);
                break;
            case ILogger.LogLevel.DEBUG:
                logger.Debug("message", memberName);
                break;
            case ILogger.LogLevel.WARN:
                logger.Warn("message", memberName);
                break;
            case ILogger.LogLevel.ERROR:
                logger.Error("message", memberName);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }

    private static void WriteLogWithArgs(Logger logger, ILogger.LogLevel level, object args, string memberName)
    {
        switch (level)
        {
            case ILogger.LogLevel.TRACE:
                logger.TraceWithArgs("message", args, memberName);
                break;
            case ILogger.LogLevel.DEBUG:
                logger.DebugWithArgs("message", args, memberName);
                break;
            case ILogger.LogLevel.WARN:
                logger.WarnWithArgs("message", args, memberName);
                break;
            case ILogger.LogLevel.ERROR:
                logger.ErrorWithArgs("message", args, memberName);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
}
