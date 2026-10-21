using System.Reflection;
using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class Logger_SetOutputTextWriter
{
    [Fact(DisplayName = "No136_正常系：正常動作の確認_textwriterにTextWriterインスタンスを指定する")]
    public void Case136()
    {
        var logger = NewLogger();
        var output = new StringWriter();
        logger.SetOutputTextWriter(output);
        logger.WriteLog(ILogger.LogLevel.INFO, "message");
        Assert.Contains("message", output.ToString());
    }
    [Fact(DisplayName = "No137_正常系：正常動作の確認_textwriterにnullを指定する")]
    public void Case137()
    {
        var logger = NewLogger();
        var original = logger.OutTextWriter;
        logger.SetOutputTextWriter(null!);
        Assert.Same(original, logger.OutTextWriter);
    }

    private static ILogger NewLogger()
    {
        var type = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
        type.GetField("outputLogLevel", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, ILogger.LogLevel.INFO);
        var logger = (ILogger)Activator.CreateInstance(type, new object[] { "SpecClass" })!;
        logger.SetOutputTextWriter(new StringWriter(new StringBuilder())); return logger;
    }
}