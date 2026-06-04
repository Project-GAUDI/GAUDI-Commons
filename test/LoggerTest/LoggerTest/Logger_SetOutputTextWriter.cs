using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.LoggerTest;

[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class Logger_SetOutputTextWriter : LoggerBase
{
    public Logger_SetOutputTextWriter(ITestOutputHelper output) : base(output) { }

    private readonly Logger _log = new();

    [Fact(DisplayName = "Logger No018_引数textwriterに有効なストリームを渡す")]
    public void Logger_Case018()
    {
        var filePath = "Logger_Case018.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var encoding = Encoding.Unicode;
        using (var stream = new StreamWriter(filePath, true, encoding))
        {
            _log.SetOutputTextWriter(stream);
        }
        Assert.Equal(encoding, _log.OutTextWriter.Encoding);

        File.Delete(filePath);
    }

    [Fact(DisplayName = "Logger No019_引数textwriterにnullを渡す")]
    public void Logger_Case019()
    {
        var filePath = "Logger_Case019.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var encoding = Encoding.Unicode;
        using (var stream = new StreamWriter(filePath, true, encoding))
        {
            _log.SetOutputTextWriter(stream);
        }

        _log.SetOutputTextWriter(null);

        // 引数にnullを渡した際の処理前の状態であるチェック
        Assert.Equal(encoding, _log.OutTextWriter.Encoding);

        File.Delete(filePath);
    }

    [Fact(DisplayName = "Logger No020_引数textwriterに閉じたストリームを渡す")]
    public void Logger_Case020()
    {
        var filePath = "Logger_Case020.txt";
        if (File.Exists(filePath))
            File.Delete(filePath);

        var encoding = Encoding.Unicode;
        using (var stream = new StreamWriter(filePath, true, encoding))
        {
            stream.Close();
            _log.SetOutputTextWriter(stream);
        }
        Assert.Equal(encoding, _log.OutTextWriter.Encoding);

        File.Delete(filePath);
    }
}
