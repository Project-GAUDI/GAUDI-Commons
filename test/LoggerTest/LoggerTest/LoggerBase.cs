using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.LoggerTest;

public abstract class LoggerBase
{
    protected const string LOG_PATTERN = "(<[0-9]>)( [0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{3} \\+[0-9]{2}:[0-9]{2} )(\\[.*\\])(.*)";

    protected readonly ITestOutputHelper _output;

    public LoggerBase(ITestOutputHelper output)
    {
        _output = output;
        Environment.SetEnvironmentVariable("LogLevel", null);
    }

    // テスト用 ModuleClient
    protected IModuleClient CreateModuleClient()
    {
        return new MqttModuleClient("TestConnectionString", "127.0.0.0");
    }

    protected string GetTag(ILogger.LogLevel level)
    {
        string tag = string.Empty;
        switch (level)
        {
            case ILogger.LogLevel.TRACE:
                tag = "7";
                break;
            case ILogger.LogLevel.DEBUG:
                tag = "6";
                break;
            case ILogger.LogLevel.INFO:
                tag = "5";
                break;
            case ILogger.LogLevel.WARN:
                tag = "4";
                break;
            case ILogger.LogLevel.ERROR:
                tag = "3";
                break;
        }
        return tag;
    }

    protected void VerfyLogMessage(string checkLine, string tag, string className, string logMessage)
    {
        var match = Regex.Match(checkLine, LOG_PATTERN);
        Assert.True(match.Success);
        Assert.Equal($"<{tag}>", match.Groups[1].ToString());
        Assert.Equal($"[{className}]", match.Groups[3].ToString());
        Assert.Equal(logMessage, match.Groups[4].ToString());
    }
}