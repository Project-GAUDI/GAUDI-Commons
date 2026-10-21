using HarmonyLib;
using Microsoft.Azure.Devices.Shared;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// ApplicationEngineのメソッド差し替えとLogger出力先の差し替えを行うため、他テストと並列実行されないコレクションに含める
[Collection(nameof(SharedLoggerState))]
public sealed class ApplicationEngine_ReceiveTwinAsync
{
    [Fact(DisplayName = "No728_正常系：正常動作の確認_Restart()がtrueを返す状態で、メソッドを呼び出す")]
    public async Task Case728()
    {
        using var patch = new EnginePatch(() => Task.FromResult(true));
        var engine = new ApplicationEngine();

        await engine.ReceiveTwinAsync(new TwinCollection(), null!);

        Assert.Equal(new[] { "Restart" }, EnginePatch.Calls);
    }

    [Fact(DisplayName = "No729_異常系：異常動作の確認_Restart()がfalseを返す状態で、メソッドを呼び出す")]
    public async Task Case729()
    {
        using var patch = new EnginePatch(() => Task.FromResult(false));
        var engine = new ApplicationEngine();

        await engine.ReceiveTwinAsync(new TwinCollection(), null!);

        Assert.Equal(new[] { "Restart", "Terminate(True)" }, EnginePatch.Calls);
    }

    [Fact(DisplayName = "No730_異常系：異常動作の確認_Restart()呼び出し中に例外が発生する状態で、メソッドを呼び出す")]
    public async Task Case730()
    {
        using var log = new LoggerOutputCapture();
        using var patch = new EnginePatch(() => Task.FromException<bool>(new InvalidOperationException("restart error")));
        var engine = new ApplicationEngine();

        await engine.ReceiveTwinAsync(new TwinCollection(), null!);

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

            harmony = new Harmony($"{nameof(ApplicationEngine_ReceiveTwinAsync)}.{Guid.NewGuid()}");
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
