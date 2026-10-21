using HarmonyLib;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// ApplicationEngineのメソッド差し替えとLogger出力先の差し替えを行うため、他テストと並列実行されないコレクションに含める
[Collection(nameof(SharedLoggerState))]
public sealed class ApplicationEngine_OnConnectionStatusChangedAsync
{
    [Fact(DisplayName = "No735_正常系：正常動作の確認_statusにConnectedを指定して、メソッドを呼び出す")]
    public async Task Case735()
    {
        using var patch = new EnginePatch(() => Task.FromResult(true));
        var engine = new ApplicationEngine();

        await engine.OnConnectionStatusChangedAsync(IotConnectionStatus.Connected, IotConnectionStatusChangeReason.Connection_Ok);

        Assert.Empty(EnginePatch.Calls);
    }

    [Fact(DisplayName = "No736_正常系：正常動作の確認_statusにDisabledを指定して、メソッドを呼び出す")]
    public async Task Case736()
    {
        using var patch = new EnginePatch(() => Task.FromResult(true));
        var engine = new ApplicationEngine();

        await engine.OnConnectionStatusChangedAsync(IotConnectionStatus.Disabled, IotConnectionStatusChangeReason.Communication_Error);

        Assert.Empty(EnginePatch.Calls);
    }

    [Fact(DisplayName = "No737_正常系：正常動作の確認_statusにDisconnected、reasonに再接続可能な理由を指定し、Restart()がtrueを返す状態で、メソッドを呼び出す")]
    public async Task Case737()
    {
        using var patch = new EnginePatch(() => Task.FromResult(true));
        var engine = new ApplicationEngine();

        await engine.OnConnectionStatusChangedAsync(IotConnectionStatus.Disconnected, IotConnectionStatusChangeReason.Communication_Error);

        Assert.Equal(new[] { "Restart" }, EnginePatch.Calls);
    }

    [Fact(DisplayName = "No738_正常系：正常動作の確認_statusにDisconnected、reasonに再接続不可能な理由を指定して、メソッドを呼び出す")]
    public async Task Case738()
    {
        using var patch = new EnginePatch(() => Task.FromResult(true));
        var engine = new ApplicationEngine();

        await engine.OnConnectionStatusChangedAsync(IotConnectionStatus.Disconnected, IotConnectionStatusChangeReason.Client_Close);

        Assert.Empty(EnginePatch.Calls);
    }

    [Fact(DisplayName = "No739_正常系：正常動作の確認_statusにDisconnected_Retryingを指定して、メソッドを呼び出す")]
    public async Task Case739()
    {
        using var patch = new EnginePatch(() => Task.FromResult(true));
        var engine = new ApplicationEngine();

        await engine.OnConnectionStatusChangedAsync(IotConnectionStatus.Disconnected_Retrying, IotConnectionStatusChangeReason.Communication_Error);

        Assert.Empty(EnginePatch.Calls);
    }

    [Fact(DisplayName = "No740_異常系：異常動作の確認_statusにDisconnected、reasonに再接続可能な理由を指定し、Restart()がfalseを返す状態で、メソッドを呼び出す")]
    public async Task Case740()
    {
        using var patch = new EnginePatch(() => Task.FromResult(false));
        var engine = new ApplicationEngine();

        await engine.OnConnectionStatusChangedAsync(IotConnectionStatus.Disconnected, IotConnectionStatusChangeReason.Communication_Error);

        Assert.Equal(new[] { "Restart", "Terminate(True)" }, EnginePatch.Calls);
    }

    [Fact(DisplayName = "No741_異常系：異常動作の確認_statusにDisconnected、reasonに再接続可能な理由を指定し、Restart()呼び出し中に例外が発生する状態で、メソッドを呼び出す")]
    public async Task Case741()
    {
        using var log = new LoggerOutputCapture();
        using var patch = new EnginePatch(() => Task.FromException<bool>(new InvalidOperationException("restart error")));
        var engine = new ApplicationEngine();

        await engine.OnConnectionStatusChangedAsync(IotConnectionStatus.Disconnected, IotConnectionStatusChangeReason.Communication_Error);

        Assert.Equal(new[] { "Restart", "Terminate(True)" }, EnginePatch.Calls);
        Assert.Contains("Restart() failed.", log.Output);
    }

    private sealed class EnginePatch : IDisposable
    {
        private readonly Harmony harmony;

        private static Func<Task<bool>> restartResult = () => Task.FromResult(true);

        public static List<string> Calls { get; } = new();

        public EnginePatch(Func<Task<bool>> restartResult)
        {
            Calls.Clear();
            EnginePatch.restartResult = restartResult;

            harmony = new Harmony($"{nameof(ApplicationEngine_OnConnectionStatusChangedAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ApplicationEngine), nameof(ApplicationEngine.Restart)),
                prefix: new HarmonyMethod(typeof(EnginePatch), nameof(RestartPrefix)));
            harmony.Patch(
                AccessTools.Method(typeof(ApplicationEngine), nameof(ApplicationEngine.Terminate)),
                prefix: new HarmonyMethod(typeof(EnginePatch), nameof(TerminatePrefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool RestartPrefix(ref Task<bool> __result)
        {
            Calls.Add("Restart");
            __result = restartResult();
            return false;
        }

        private static bool TerminatePrefix(bool __0, ref Task<bool> __result)
        {
            Calls.Add($"Terminate({__0})");
            __result = Task.FromResult(true);
            return false;
        }
    }
}
