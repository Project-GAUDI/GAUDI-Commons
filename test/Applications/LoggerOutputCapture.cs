using System.Reflection;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test;

/// <summary>Loggerの静的な出力先とログレベルを一時的に差し替え、出力内容を捕捉する。</summary>
internal sealed class LoggerOutputCapture : IDisposable
{
    private static readonly Type LoggerType = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
    private readonly StringWriter writer = new();
    private readonly object? originalWriter;
    private readonly object? originalLogLevel;
    private readonly object? originalPermanent;

    public string Output => writer.ToString();

    public LoggerOutputCapture()
    {
        originalWriter = GetStatic("outTextWriter");
        originalLogLevel = GetStatic("outputLogLevel");
        originalPermanent = GetStatic("isMandatoryLoglevelPermanent");

        SetStatic("outTextWriter", TextWriter.Synchronized(writer));
        SetStatic("outputLogLevel", ILogger.LogLevel.INFO);
        SetStatic("isMandatoryLoglevelPermanent", false);
    }

    public void Dispose()
    {
        SetStatic("outTextWriter", originalWriter);
        SetStatic("outputLogLevel", originalLogLevel);
        SetStatic("isMandatoryLoglevelPermanent", originalPermanent);
        writer.Dispose();
    }

    private static object? GetStatic(string name)
    {
        return LoggerType.GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
    }

    private static void SetStatic(string name, object? value)
    {
        LoggerType.GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, value);
    }
}
