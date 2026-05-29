using Microsoft.Azure.Devices.Client;
using Microsoft.VisualBasic;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.LoggerTest;

[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class Logger_WriteLog : LoggerBase
{
    #region 既存テストで利用
    // ILogger log = LoggerFactory.GetLogger(typeof(Logger_WriteLog));
    #endregion

    public Logger_WriteLog(ITestOutputHelper output) : base(output)
    {
        #region 既存テストで利用
        // log.SetOutputLogLevel("INFO");
        // log.SetMandatoryLogLevel("INFO", 0);
        #endregion
    }

    private readonly Mock<IModuleClient> _moduleClientMock = new();

    private readonly string _testClassName = "testClass";

    private readonly string _uploadFailedMsg = "- UpdateReportedProperties failed:";

    private readonly string _moduleClientNotFoundMsg = "- Can't Update Reported Properties because Logger dosen't have module client.";

    private void SetupModuleClientMock(string tag, string className, string logMessage)
    {
        _moduleClientMock.
            Setup(x => x.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>())).
            Callback((string outputName, IotMessage message) =>
                {
                    _output.WriteLine("--- IModuleClient.SendEventAsync --");

                    Assert.Equal("log", outputName);
                    _output.WriteLine($"outputName: {outputName}");

                    var body = message.GetBodyString();
                    var prop = message.GetProperties();

                    Assert.Equal("log", prop["type"]);
                    Assert.Equal(tag, prop["level"]);
                    _output.WriteLine($"Properties: {JsonSerializer.Serialize(prop)}");

                    var recordInfo = JsonMessage.DeserializeRecordInfo(body);
                    _output.WriteLine($"RecordInfo: {JsonMessage.SerializeRecordInfo(recordInfo)}");

                    var match = Regex.Match(recordInfo.RecordHeader.First(), "[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{3} \\+[0-9]{2}:[0-9]{2}");
                    Assert.True(match.Success);

                    // RecordDataの1件目に該当ログがあることを前提としてチェックしている
                    VerfyLogMessage(recordInfo.RecordData.First(), tag, className, logMessage);
                }).
            Returns(Task.CompletedTask);
    }

    private bool VerifyPatterns(List<string> lines, List<string> patternList)
    {
        foreach (var pattern in patternList)
        {
            bool isMatchFound = false;
            foreach (var str in lines)
            {
                if (Regex.IsMatch(str, pattern))
                {
                    isMatchFound = true;
                    break;
                }
            }

            if (!isMatchFound)
            {
                return false;
            }
        }
        return true;
    }

    [Fact(DisplayName = "Logger No033_設定されたログレベルが等しい")]
    public void Logger_Case033()
    {
        var logger = new Logger(_testClassName);
        var logLevel = ILogger.LogLevel.INFO;
        logger.SetOutputLogLevel(logLevel.ToString());

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Information message";
        logger.WriteLog(logLevel, logMessage);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        VerfyLogMessage(consoleOutput, GetTag(logLevel), _testClassName, logMessage);
    }

    [Fact(DisplayName = "Logger No034_設定されたログレベルを上回る")]
    public void Logger_Case034()
    {
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.INFO.ToString());

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Warning message";
        var logLevel = ILogger.LogLevel.WARN;
        logger.WriteLog(logLevel, logMessage);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        VerfyLogMessage(consoleOutput, GetTag(logLevel), _testClassName, logMessage);
    }

    [Fact(DisplayName = "Logger No035_設定されたログレベルを下回る")]
    public void Logger_Case035()
    {
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.INFO.ToString());

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Debug message";
        var logLevel = ILogger.LogLevel.DEBUG;
        logger.WriteLog(logLevel, logMessage);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        Assert.Empty(consoleOutput);
    }

    [Fact(DisplayName = "Logger No036_設定されたログレベルが最高ログレベル")]
    public void Logger_Case036()
    {
        var logger = new Logger(_testClassName);
        var logLevel = ILogger.LogLevel.ERROR;
        logger.SetOutputLogLevel(logLevel.ToString());

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Error message";
        logger.WriteLog(logLevel, logMessage);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        VerfyLogMessage(consoleOutput, GetTag(logLevel), _testClassName, logMessage);
    }

    [Fact(DisplayName = "Logger No037_設定されたログレベルが最低ログレベル")]
    public void Logger_Case037()
    {
        var logger = new Logger(_testClassName);
        var logLevel = ILogger.LogLevel.TRACE;
        logger.SetOutputLogLevel(logLevel.ToString());

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Trace message";
        logger.WriteLog(logLevel, logMessage);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        VerfyLogMessage(consoleOutput, GetTag(logLevel), _testClassName, logMessage);
    }

    [Fact(DisplayName = "Logger No038_出力先を有効なストリームに変更し、メソッドを呼ぶ。")]
    public void Logger_Case038()
    {
        var filePath = "Logger_Case038.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var logLevel = ILogger.LogLevel.INFO;
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(logLevel.ToString());

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var consoleMessage = "console message";
        logger.WriteLog(ILogger.LogLevel.WARN, consoleMessage);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- StringWriter Output Log --");
        _output.WriteLine(consoleOutput);

        var logMessage = "Infomation message";
        using (var stream = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            stream.WriteLine("test message");
            logger.SetOutputTextWriter(stream);
            logger.WriteLog(logLevel, logMessage);
        }

        string output;
        using (var stream = new StreamReader(filePath, Encoding.UTF8))
        {
            output = stream.ReadToEnd().Trim();
            _output.WriteLine($"-- File: {filePath} --");
            _output.WriteLine(output);
        }

        VerfyLogMessage(output, GetTag(logLevel), _testClassName, logMessage);

        // 出力先が変更されたことのチェックとして、それぞれのメッセージが含まれていないことをチェック
        Assert.False(consoleOutput.Contains(logMessage)!);
        Assert.False(output.Contains(consoleMessage)!);

        File.Delete(filePath);
    }

    [Fact(DisplayName = "Logger No039_出力先を閉じたストリームに変更し、メソッドを呼ぶ")]
    public void Logger_Case039()
    {
        var filePath = "Logger_Case039.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var logLevel = ILogger.LogLevel.INFO;
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(logLevel.ToString());

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Infomation message";
        logger.WriteLog(logLevel, logMessage);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- StringWriter Output Log --");
        _output.WriteLine(consoleOutput);

        using (var stream = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            stream.WriteLine("test message");
            stream.Close();
            logger.SetOutputTextWriter(stream);
        }
        Assert.ThrowsAny<Exception>(() => logger.WriteLog(logLevel, logMessage));

        File.Delete(filePath);
    }

    [Fact(DisplayName = "Logger No040_出力先にnullを指定して、メソッドを呼ぶ")]
    public void Logger_Case040()
    {
        var logger = new Logger(_testClassName);
        var logLevel = ILogger.LogLevel.INFO;
        logger.SetOutputLogLevel(logLevel.ToString());

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Infomation message";
        logger.WriteLog(ILogger.LogLevel.WARN, logMessage);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- StringWriter Output Log --");
        _output.WriteLine(consoleOutput);

        logger.SetOutputTextWriter(null);
        logger.WriteLog(logLevel, logMessage);

        consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- StringWriter Output Log --");
        _output.WriteLine(consoleOutput);

        VerfyLogMessage(consoleOutput.Split(Environment.NewLine)[1], GetTag(logLevel), _testClassName, logMessage);
    }

    [Fact(DisplayName = "Logger No041_複数のスレッドからログ出力する")]
    public void Logger_Case041()
    {
        var filePath = "Logger_Case041.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.TRACE.ToString());

        var logMessages = new List<(ILogger.LogLevel, string)>();
        var patternList = new List<string>();
        for (var i = 0; i < 10; i++)
        {
            var rnd = new Random();
            int lev = rnd.Next(0, 4);

            logMessages.Add(((ILogger.LogLevel)lev, $"{Enum.GetName(typeof(ILogger.LogLevel), lev)} message {i}"));

            patternList.Add($"(<{GetTag(logMessages[i].Item1)}>)( [0-9]{{4}}-[0-9]{{2}}-[0-9]{{2}} [0-9]{{2}}:[0-9]{{2}}:[0-9]{{2}}.[0-9]{{3}} \\+[0-9]{{2}}:[0-9]{{2}} )(\\[.*\\])({logMessages[i].Item2})");
        }

        using (var stream = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            logger.SetOutputTextWriter(stream);

            Parallel.ForEach(logMessages, logMessage =>
                logger.WriteLog(logMessage.Item1, logMessage.Item2));
        }

        string output;
        using (var stream = new StreamReader(filePath, Encoding.UTF8))
        {
            output = stream.ReadToEnd();
            _output.WriteLine($"-- File: {filePath} --");
            _output.WriteLine(output);
        }

        var lines = output.Split(Environment.NewLine).ToList();

        var allMatch = VerifyPatterns(lines, patternList);
        Assert.True(allMatch);

        File.Delete(filePath);
    }

    [Fact(DisplayName = "Logger No042_長いログメッセージを出力する")]
    public void Logger_Case042()
    {
        var filePath = "Logger_Case042.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var logLevel = ILogger.LogLevel.INFO;
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(logLevel.ToString());

        var logMessage = string.Join("", Enumerable.Range(0, 10000).Select(x => (x % 10).ToString()));

        using (var stream = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            logger.SetOutputTextWriter(stream);
            logger.WriteLog(logLevel, logMessage);
        }

        string output;
        using (var stream = new StreamReader(filePath, Encoding.UTF8))
        {
            output = stream.ReadToEnd().Trim();
        }

        VerfyLogMessage(output, GetTag(logLevel), _testClassName, logMessage);

        File.Delete(filePath);
    }

    [Fact(DisplayName = "Logger No043_空文字のログメッセージを出力する")]
    public void Logger_Case043()
    {
        var filePath = "Logger_Case043.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var logLevel = ILogger.LogLevel.INFO;
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(logLevel.ToString());

        var logMessage = "";
        using (var stream = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            logger.SetOutputTextWriter(stream);
            logger.WriteLog(logLevel, logMessage);
        }

        string output;
        using (var stream = new StreamReader(filePath, Encoding.UTF8))
        {
            output = stream.ReadToEnd().Trim();
            _output.WriteLine($"-- File: {filePath} --");
            _output.WriteLine(output);
        }

        VerfyLogMessage(output, GetTag(logLevel), _testClassName, logMessage);

        File.Delete(filePath);
    }

    [Fact(DisplayName = "Logger No044_nullのログメッセージを出力する")]
    public void Logger_Case044()
    {
        var filePath = "Logger_Case044.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var logLevel = ILogger.LogLevel.INFO;
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(logLevel.ToString());

        string? logMessage = null;
        using (var stream = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            logger.SetOutputTextWriter(stream);
            logger.WriteLog(logLevel, logMessage);
        }

        string output;
        using (var stream = new StreamReader(filePath, Encoding.UTF8))
        {
            output = stream.ReadToEnd().Trim();
            _output.WriteLine($"-- File: {filePath} --");
            _output.WriteLine(output);
        }

        // null出力指定時は、空文字列想定
        VerfyLogMessage(output, GetTag(logLevel), _testClassName, "");

        File.Delete(filePath);
    }

    [Fact(DisplayName = "Logger No045_改行コード\r\nを含むログメッセージを出力する")]
    public void Logger_Case045()
    {
        var filePath = "Logger_Case045.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.INFO.ToString());

        var line1 = "Line1";
        var line2 = "Line2";
        var logMessage = $"{line1}\r\n{line2}";
        var logLevel = ILogger.LogLevel.INFO;
        using (var stream = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            logger.SetOutputTextWriter(stream);
            logger.WriteLog(logLevel, logMessage);
        }

        string output;
        using (var stream = new StreamReader(filePath, Encoding.UTF8))
        {
            output = stream.ReadToEnd();
            _output.WriteLine($"-- File: {filePath} --");
            _output.WriteLine(output);
        }

        var lines = output.Split(Environment.NewLine);
        VerfyLogMessage(lines[0], GetTag(logLevel), _testClassName, line1);
        VerfyLogMessage(lines[1], GetTag(logLevel), _testClassName, line2);

        File.Delete(filePath);
    }

    [Fact(DisplayName = "Logger No046_改行コード\nを含むログメッセージを出力する")]
    public void Logger_Case046()
    {
        var filePath = "Logger_Case046.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.INFO.ToString());

        var line1 = "Line1";
        var line2 = "Line2";
        var logMessage = $"{line1}\n{line2}";
        var logLevel = ILogger.LogLevel.INFO;
        using (var stream = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            logger.SetOutputTextWriter(stream);
            logger.WriteLog(logLevel, logMessage);
        }

        string output;
        using (var stream = new StreamReader(filePath, Encoding.UTF8))
        {
            output = stream.ReadToEnd();
            _output.WriteLine($"-- File: {filePath} --");
            _output.WriteLine(output);
        }

        var lines = output.Split(Environment.NewLine);
        VerfyLogMessage(lines[0], GetTag(logLevel), _testClassName, line1);
        VerfyLogMessage(lines[1], GetTag(logLevel), _testClassName, line2);

        File.Delete(filePath);
    }

    [Fact(DisplayName = "Logger No047_MyClientが有効で接続状態がConnectedの場合のTRACEログ出力")]
    public async void Logger_Case047()
    {
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.TRACE.ToString());

        _moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Connected);
        logger.SetModuleClient(_moduleClientMock.Object);

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Trace message";
        var logLevel = ILogger.LogLevel.TRACE;
        SetupModuleClientMock(GetTag(logLevel), _testClassName, logMessage);

        logger.WriteLog(logLevel, logMessage, true);

        await Task.Delay(1000);
        _moduleClientMock.
            Verify(x => x.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Once);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        var failed = consoleOutput.Contains(_uploadFailedMsg);
        Assert.False(failed);
    }

    [Fact(DisplayName = "Logger No048_MyClientが有効で接続状態がConnectedの場合のDEBUGログ出力")]
    public async void Logger_Case048()
    {
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.TRACE.ToString());

        _moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Connected);
        logger.SetModuleClient(_moduleClientMock.Object);

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Debug message";
        var logLevel = ILogger.LogLevel.DEBUG;
        SetupModuleClientMock(GetTag(logLevel), _testClassName, logMessage);

        logger.WriteLog(ILogger.LogLevel.DEBUG, logMessage, true);

        await Task.Delay(1000);
        _moduleClientMock.
            Verify(x => x.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Once);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        var failed = consoleOutput.Contains(_uploadFailedMsg);
        Assert.False(failed);
    }

    [Fact(DisplayName = "Logger No049_MyClientが有効で接続状態がConnectedの場合のINFOログ出力")]
    public async void Logger_Case049()
    {
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.TRACE.ToString());

        _moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Connected);
        logger.SetModuleClient(_moduleClientMock.Object);

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Infomation message";
        var logLevel = ILogger.LogLevel.INFO;
        SetupModuleClientMock(GetTag(logLevel), _testClassName, logMessage);

        logger.WriteLog(logLevel, logMessage, true);

        await Task.Delay(1000);
        _moduleClientMock.
            Verify(x => x.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Once);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        var failed = consoleOutput.Contains(_uploadFailedMsg);
        Assert.False(failed);
    }

    [Fact(DisplayName = "Logger No050_MyClientが有効で接続状態がConnectedの場合のWARNログ出力")]
    public async void Logger_Case050()
    {
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.TRACE.ToString());

        _moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Connected);
        logger.SetModuleClient(_moduleClientMock.Object);

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Warning message";
        var logLevel = ILogger.LogLevel.WARN;
        SetupModuleClientMock(GetTag(logLevel), _testClassName, logMessage);

        logger.WriteLog(logLevel, logMessage, true);

        await Task.Delay(1000);
        _moduleClientMock.
            Verify(x => x.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Once);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        var failed = consoleOutput.Contains(_uploadFailedMsg);
        Assert.False(failed);
    }

    [Fact(DisplayName = "Logger No051_MyClientが有効で接続状態がConnectedの場合のERRORログ出力")]
    public async void Logger_Case051()
    {
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.TRACE.ToString());

        _moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Connected);
        logger.SetModuleClient(_moduleClientMock.Object);

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Error message";
        var logLevel = ILogger.LogLevel.ERROR;
        SetupModuleClientMock(GetTag(logLevel), _testClassName, logMessage);

        logger.WriteLog(logLevel, logMessage, true);

        await Task.Delay(1000);
        _moduleClientMock.
            Verify(x => x.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Once);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        var failed = consoleOutput.Contains(_uploadFailedMsg);
        Assert.False(failed);
    }

    [Fact(DisplayName = "Logger No052_MyClientが有効で接続状態がConnectedの場合の複数スレッドからの複数アップロード")]
    public async void Logger_Case052()
    {
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.TRACE.ToString());

        _moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Connected);
        logger.SetModuleClient(_moduleClientMock.Object);

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessages = new List<(ILogger.LogLevel, string)>();
        for (var i = 0; i < 10; i++)
        {
            var rnd = new Random();
            int lev = rnd.Next(0, 4);

            logMessages.Add(((ILogger.LogLevel)lev, $"{Enum.GetName(typeof(ILogger.LogLevel), lev)} message {i} {nameof(Logger_Case052)}"));
        }

        _moduleClientMock.
            Setup(x => x.SendEventAsync(It.IsAny<string>(), It.Is<IotMessage>(x => x.GetBodyString().Contains(nameof(Logger_Case052))))).
            Returns(Task.CompletedTask);

        logMessages.ForEach(logMessage =>
            logger.WriteLog(logMessage.Item1, logMessage.Item2, true));

        await Task.Delay(2000);

        foreach (var logMessage in logMessages)
        {
            _moduleClientMock.
                Verify(x => x.SendEventAsync(It.IsAny<string>(), It.Is<IotMessage>(x => x.GetBodyString().Contains(logMessage.Item2))), Times.Once);
        }

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        var failed = consoleOutput.Contains(_uploadFailedMsg);
        Assert.False(failed);
    }

    [Fact(DisplayName = "Logger No053_MyClientがnull")]
    public void Logger_Case053()
    {
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.TRACE.ToString());

        logger.SetModuleClient(null);

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Trace message";
        logger.WriteLog(ILogger.LogLevel.TRACE, logMessage, true);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        var failed = consoleOutput.Contains(_moduleClientNotFoundMsg);
        Assert.True(failed);
    }

    [Fact(DisplayName = "Logger No054_MyClientが有効で接続状態がConnectedでない")]
    public void Logger_Case054()
    {
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.TRACE.ToString());

        _moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Disabled);
        logger.SetModuleClient(_moduleClientMock.Object);

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "Trace message";
        logger.WriteLog(ILogger.LogLevel.TRACE, logMessage, true);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        var failed = consoleOutput.Contains(_moduleClientNotFoundMsg);
        Assert.True(failed);
    }

    [Fact(DisplayName = "Logger No055_アップロード中に例外が発生する")]
    public async void Logger_Case055()
    {
        var logger = new Logger(_testClassName);
        logger.SetOutputLogLevel(ILogger.LogLevel.TRACE.ToString());

        _moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Connected);
        logger.SetModuleClient(_moduleClientMock.Object);

        var stringWriter = new StringWriter();
        logger.SetOutputTextWriter(stringWriter);

        var logMessage = "throw exeptionCase";
        _moduleClientMock.
            Setup(x => x.SendEventAsync(It.IsAny<string>(), It.Is<IotMessage>(x => x.GetBodyString().Contains(logMessage)))).
            Throws(new Exception("upload error"));

        logger.WriteLog(ILogger.LogLevel.INFO, logMessage, true);

        await Task.Delay(1000);
        _moduleClientMock.
            Verify(x => x.SendEventAsync(It.IsAny<string>(), It.Is<IotMessage>(x => x.GetBodyString().Contains(logMessage))), Times.Once);

        var consoleOutput = stringWriter.GetStringBuilder().ToString().Trim();
        _output.WriteLine("-- Output Log --");
        _output.WriteLine(consoleOutput);

        var failed = consoleOutput.Contains(_uploadFailedMsg);
        Assert.True(failed);
    }
/*
    #region 既存テスト

    // ログの出力テスト
    // ログが出力されない　その１
    // ログレベルが最低
    //     isUpload：false
    [Fact(DisplayName = "ログ不出：ログレベル最低")]
    public void WriteLogTest_001()
    {

        string inputMessage;

        var w = new System.IO.StringWriter();
        log.SetOutputTextWriter(w);

        //No1
        //Given
        log.SetOutputLogLevel("DEBUG");
        inputMessage = "出力されない";

        //When
        log.WriteLog(ILogger.LogLevel.TRACE, inputMessage);


        //Then
        var _consoleOutput = w.GetStringBuilder().ToString().Trim();
        Assert.Equal("", _consoleOutput);
    }

    // ログの出力テスト
    // ログが出力されない　その２
    // ログレベルが最高
    //     isUpload：true
    [Fact(DisplayName = "ログ不出：ログレベル最高")]
    public void WriteLogTest_002()
    {

        string inputMessage;

        var w = new System.IO.StringWriter();
        log.SetOutputTextWriter(w);

        //No2
        //Given
        log.SetOutputLogLevel("ERROR");
        inputMessage = "";

        //When
        log.WriteLog(ILogger.LogLevel.WARN, inputMessage);


        //Then
        var _consoleOutput = w.GetStringBuilder().ToString().Trim();
        Assert.Equal("", _consoleOutput);
    }

    // ログの出力テスト
    // ログが出力される　その１
    // ログレベルがTRACE(0)
    //     isUpload：true
    //     ModuleClient：テスト用ModuleClient
    [Fact(DisplayName = "ログ出力：ログレベルTrace(0)")]
    public void WriteLogTest_003()
    {

        string? inputMessage;

        var w = new System.IO.StringWriter();
        log.SetOutputTextWriter(w);

        // モジュールクライアント初期化
        IModuleClient client;
        //No3
        //Given
        log.SetOutputLogLevel("TRACE");
        inputMessage = null;
        client = new StubModuleClient();
        log.SetModuleClient(client);

        //When
        log.WriteLog(ILogger.LogLevel.TRACE, inputMessage, true);

        //Then
        var _consoleOutput = w.GetStringBuilder().ToString().Trim();
        // Assert.True(_consoleOutput.Contains("[TRC][Commons.Test.Logger_WriteLog] -"));
        Assert.StartsWith("<7>", _consoleOutput);
        Assert.Contains("[Logger_WriteLog]", _consoleOutput);

    }

    // ログの出力テスト
    // ログが出力される　その２
    // ログレベルがDEBUG(1)
    //     isUpload：false
    //     ModuleClient：テスト用ModuleClient
    [Fact(DisplayName = "ログ出力：ログレベルDEBUG(1)")]
    public void WriteLogTest_004()
    {

        string inputMessage;

        var w = new System.IO.StringWriter();
        log.SetOutputTextWriter(w);

        // モジュールクライアント初期化
        IModuleClient client;


        //No4
        //Given
        log.SetOutputLogLevel("DEBUG");
        inputMessage = "TestNo.4";
        client = new StubModuleClient();
        log.SetModuleClient(client);

        //When
        log.WriteLog(ILogger.LogLevel.INFO, inputMessage, false);

        //Then
        var _consoleOutput = w.GetStringBuilder().ToString().Trim();
        // Assert.True(_consoleOutput.Contains("[INF][Commons.Test.Logger_WriteLog] - TestNo.4"));
        Assert.StartsWith("<5>", _consoleOutput);
        Assert.Contains("[Logger_WriteLog]TestNo.4", _consoleOutput);

    }

    // ログの出力テスト
    // ログが出力される　その３
    //     ログレベルがINFO(2)
    // プロパティ更新不可の警告を出す
    //     isUpload：true
    //     ModuleClient：null
    [Fact(DisplayName = "ログ出力：ログレベルINFO(2)")]
    public void WriteLogTest_005()
    {

        string inputMessage;

        var w = new System.IO.StringWriter();
        log.SetOutputTextWriter(w);

        //No5
        //Given
        log.SetOutputLogLevel("INFO");
        inputMessage = "";
        log.SetModuleClient(null);

        //When
        log.WriteLog(ILogger.LogLevel.INFO, inputMessage, true);

        //Then
        var _consoleOutput = w.GetStringBuilder().ToString().Trim();

        // Assert.True(_consoleOutput.Contains("[INF][Commons.Test.Logger_WriteLog] - "));
        Assert.StartsWith("<5>", _consoleOutput);
        Assert.Contains("[Logger_WriteLog] - ", _consoleOutput);
        Assert.Contains("[WRN][Logger_WriteLog] - Can't Update Reported Properties because Logger dosen't have module client.", _consoleOutput);

    }

    // ログの出力テスト
    // ログが出力される　その４
    //     ログレベルがWARN(3)
    // プロパティ更新不可の警告を出さない
    //     isUpload：false
    //     ModuleClient：null
    [Fact(DisplayName = "ログ出力：ログレベルWARN(3)")]
    public void WriteLogTest_006()
    {

        string? inputMessage;

        var w = new System.IO.StringWriter();
        log.SetOutputTextWriter(w);

        //No6
        //Given
        log.SetOutputLogLevel("WARN");
        inputMessage = null;
        log.SetModuleClient(null);

        //When
        log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, false);

        //Then
        System.Threading.Thread.Sleep(5000);//非同期処理の例外が発生するのを待つため５秒間停止する。
        var _consoleOutput = w.GetStringBuilder().ToString().Trim();

        // Assert.True(_consoleOutput.Contains("[ERR][Commons.Test.Logger_WriteLog]"));
        Assert.StartsWith("<3>", _consoleOutput);
        Assert.Contains("[Logger_WriteLog]", _consoleOutput);


    }

    // ログの出力テスト
    // ログが出力される　その４
    //     ログレベルがERROR(4)
    // 非同期処理コール確認
    //     ※例外を発生させることで確認する
    //     isUpload：true
    //     ModuleClient：テスト用ModuleClient
    [Fact(DisplayName = "ログ出力：ログレベルERROR(4)")]
    public void WriteLogTest_007()
    {

        string inputMessage;

        var w = new System.IO.StringWriter();
        log.SetOutputTextWriter(w);

        // モジュールクライアント初期化
        IModuleClient client;
        //No7
        //Given
        log.SetOutputLogLevel("ERROR");
        inputMessage = "TestNo.7";
        client = new StubModuleClient();
        log.SetModuleClient(client);

        //When
        log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, true);

        //Then
        System.Threading.Thread.Sleep(5000);//非同期処理の例外が発生するのを待つため５秒間停止する。
        var _consoleOutput = w.GetStringBuilder().ToString().Trim();

        // Assert.True(_consoleOutput.Contains("[ERR][Commons.Test.Logger_WriteLog] - TestNo.7"));
        Assert.StartsWith("<3>", _consoleOutput);
        Assert.Contains("[Logger_WriteLog]TestNo.7", _consoleOutput);

        // 再現が出来ない為、一旦削除
        // Assert.True(_consoleOutput.Contains("[WRN][Commons.Test.Logger_WriteLog] - UpdateReportedProperties failed:"));

        //Assert.Throws<Exception>(() => log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, true));

    }

    // ログの出力テスト
    // ログが出力される　その5
    //     ログレベルがINFO(2)
    //     isUpload：false
    //StreamWriterの引数としてStream型のMemoeryStreamについて確認
    [Fact(DisplayName = "ログ出力：MemoryStream使用")]
    public void SetOutputStreamTest001()
    {
        var memoryStream = new MemoryStream();
        var msw = new StreamWriter(memoryStream);
        log.SetOutputTextWriter(msw);
        log.WriteLog(ILogger.LogLevel.INFO, "Test", false);
        msw.Flush();

        memoryStream.Position = 0; // ストリームの位置を初めに戻す
        using (var reader = new StreamReader(memoryStream))
        {
            var _consoleOutput = reader.ReadToEnd(); // ストリームの内容を文字列として読み取る
            Assert.StartsWith("<5>", _consoleOutput);
        }
    }

    // ログの出力テスト
    // ログが出力される　その6
    //     ログレベルがINFO(2)
    //     isUpload：false
    //StreamWriterの引数としてStream型のFileStreamについて確認
    [Fact(DisplayName = "ログ出力：FileStream使用")]
    public void SetOutputStreamTest002()
    {
        FileStream fs = new FileStream("Test.txt", FileMode.Create);
        var msw = new StreamWriter(fs);
        log.SetOutputTextWriter(msw);
        log.WriteLog(ILogger.LogLevel.INFO, "Test", false);
        msw.Flush();
        fs.Close();

        FileStream fsr = new FileStream("Test.txt", FileMode.Open);
        using (var reader = new StreamReader(fsr))
        {
            var _consoleOutput = reader.ReadToEnd(); // ストリームの内容を文字列として読み取る
            Assert.StartsWith("<5>", _consoleOutput);
        }
        fsr.Close();
    }

    // ログの出力テスト
    // ログが出力される　その７
    //     StubModuleClientで送信を確認
    // 非同期処理コール確認
    //     isUpload：true
    //     ModuleClient：StubModuleClient
    [Fact(DisplayName = "ログ出力：ModuleClientによる送信")]
    public async void MessageSendTest001()
    {
        string inputMessage;

        var w = new System.IO.StringWriter();
        log.SetOutputTextWriter(w);

        // モジュールクライアント初期化
        IModuleClient client;
        //No7
        //Given
        log.SetOutputLogLevel("ERROR");
        inputMessage = "MessageSendTest001";

        client = new StubModuleClient();
        log.SetModuleClient(client);

        IotMessageHandler handler = async (IotMessage message, Object context) =>
        {
            var body = message.GetBodyString();

            // Logger内でtry～catchしているので、ここでのテスト結果は、下部のDoesNotContainの部分で判定。
            Assert.Contains("\"RecordData\":[\"<3>", body);
            Assert.Contains("[Logger_WriteLog]MessageSendTest001", body);

            await Task.CompletedTask;
            return MessageResponse.Completed;
        };
        await client.SetInputMessageHandlerAsync("log", handler, null);

        //When
        log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, true);

        //Then
        System.Threading.Thread.Sleep(500);//非同期処理の例外が完了するのを待つため0.５秒間停止する。
        var _consoleOutput = w.GetStringBuilder().ToString().Trim();

        Assert.StartsWith("<3>", _consoleOutput);
        Assert.Contains("[Logger_WriteLog]MessageSendTest001", _consoleOutput);

        // 送信メッセージの内容チェックにエラーが無いか判定
        Assert.DoesNotContain("[WRN][Logger_WriteLog] - UpdateReportedProperties failed:", _consoleOutput);

        //Assert.Throws<Exception>(() => log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, true));

    }
    #endregion
*/
}