using HarmonyLib;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// ApplicationEngineのメソッドをHarmonyで差し替えるため、他テストと並列実行されないコレクションに含める
[Collection(nameof(SharedLoggerState))]
public sealed class ApplicationEngine_Restart
{
    [Fact(DisplayName = "No708_異常系：異常動作の確認_applicationMainが未設定(null)の状態で、メソッドを呼び出す")]
    public async Task Case708()
    {
        var engine = new ApplicationEngine();

        var exception = await Assert.ThrowsAsync<Exception>(() => engine.Restart());

        Assert.Equal("Error: ApplicationMain not set.", exception.Message);
    }

    [Fact(DisplayName = "No709_正常系：正常動作の確認_applicationMain設定済み、Init()がtrueを返す、Ready()がtrueを返す状態で、メソッドを呼び出す")]
    public async Task Case709()
    {
        using var patch = new EnginePatch(initResult: true, readyResult: true);
        var engine = NewEngine();

        var result = await engine.Restart();

        Assert.True(result);
        Assert.Equal(new[] { "Term", "Init", "Ready" }, EnginePatch.Calls);
    }

    [Fact(DisplayName = "No710_異常系：異常動作の確認_applicationMain設定済み、Init()がfalseを返す状態で、メソッドを呼び出す")]
    public async Task Case710()
    {
        using var patch = new EnginePatch(initResult: false, readyResult: true);
        var engine = NewEngine();

        var exception = await Assert.ThrowsAsync<Exception>(() => engine.Restart());

        Assert.Equal("Error: Failed to change the ApplicationState to 'Ready'.", exception.Message);
        Assert.Equal(new[] { "Term", "Init" }, EnginePatch.Calls);
    }

    [Fact(DisplayName = "No711_異常系：異常動作の確認_applicationMain設定済み、Init()がtrueを返す、Ready()がfalseを返す状態で、メソッドを呼び出す")]
    public async Task Case711()
    {
        using var patch = new EnginePatch(initResult: true, readyResult: false);
        var engine = NewEngine();

        var exception = await Assert.ThrowsAsync<Exception>(() => engine.Restart());

        Assert.Equal("Error: Failed to change the ApplicationState to 'Ready'.", exception.Message);
        Assert.Equal(new[] { "Term", "Init", "Ready" }, EnginePatch.Calls);
    }

    private static ApplicationEngine NewEngine()
    {
        var engine = new ApplicationEngine();
        engine.SetApplication(new Mock<IApplicationMain>().Object);
        return engine;
    }

    private sealed class EnginePatch : IDisposable
    {
        private readonly Harmony harmony;

        private static bool initResult;

        private static bool readyResult;

        public static List<string> Calls { get; } = new();

        public EnginePatch(bool initResult, bool readyResult)
        {
            Calls.Clear();
            EnginePatch.initResult = initResult;
            EnginePatch.readyResult = readyResult;

            harmony = new Harmony($"{nameof(ApplicationEngine_Restart)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ApplicationEngine), nameof(ApplicationEngine.Term)),
                prefix: new HarmonyMethod(typeof(EnginePatch), nameof(TermPrefix)));
            harmony.Patch(
                AccessTools.Method(typeof(ApplicationEngine), nameof(ApplicationEngine.Init)),
                prefix: new HarmonyMethod(typeof(EnginePatch), nameof(InitPrefix)));
            harmony.Patch(
                AccessTools.Method(typeof(ApplicationEngine), nameof(ApplicationEngine.Ready)),
                prefix: new HarmonyMethod(typeof(EnginePatch), nameof(ReadyPrefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool TermPrefix(ref Task<bool> __result)
        {
            Calls.Add("Term");
            __result = Task.FromResult(true);
            return false;
        }

        private static bool InitPrefix(ref Task<bool> __result)
        {
            Calls.Add("Init");
            __result = Task.FromResult(initResult);
            return false;
        }

        private static bool ReadyPrefix(ref Task<bool> __result)
        {
            Calls.Add("Ready");
            __result = Task.FromResult(readyResult);
            return false;
        }
    }
}
