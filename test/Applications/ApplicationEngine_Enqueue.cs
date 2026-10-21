using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// Loggerの出力先（静的状態）を差し替えるため、Logger系テストと直列化する
[Collection(nameof(SharedLoggerState))]
public sealed class ApplicationEngine_Enqueue
{
    [Fact(DisplayName = "No715_正常系：正常動作の確認_CurrentStateがStartの状態で、appStateにInitialize（遷移可能）を指定して、メソッドを呼び出す")]
    public void Case715()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Start);

        var result = engine.Enqueue(ApplicationState.Initialize);

        Assert.Equal(ApplicationStateChangeResult.Success, result);
        Assert.Equal(ApplicationState.Initialize, GetStateController(engine).CurrentState);
    }

    [Fact(DisplayName = "No716_異常系：異常動作の確認_CurrentStateがStartの状態で、appStateにRunning（遷移不可）を指定して、メソッドを呼び出す")]
    public void Case716()
    {
        using var log = new LogCapture();
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Start);

        var result = engine.Enqueue(ApplicationState.Running);

        Assert.Equal(ApplicationStateChangeResult.Ignored, result);
        Assert.Equal(ApplicationState.Start, GetStateController(engine).CurrentState);
        Assert.Contains("State change failed. Start -> Running", log.Output);
    }

    private static StateController GetStateController(ApplicationEngine engine)
    {
        return (StateController)typeof(ApplicationEngine)
            .GetField("stateController", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
    }

    private static void SetCurrentState(ApplicationEngine engine, ApplicationState state)
    {
        typeof(StateController)
            .GetField("currentState", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(GetStateController(engine), state);
    }

    private sealed class LogCapture : IDisposable
    {
        private static readonly Type LoggerType = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
        private readonly StringWriter writer = new();
        private readonly object? originalWriter;
        private readonly object? originalLogLevel;
        private readonly object? originalPermanent;

        public string Output => writer.ToString();

        public LogCapture()
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
}
