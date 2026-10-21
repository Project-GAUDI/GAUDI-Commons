using HarmonyLib;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// ApplicationEngineのメソッドをHarmonyで差し替えるため、他テストと並列実行されないコレクションに含める
[Collection(nameof(SharedLoggerState))]
public sealed class ApplicationEngine_Terminate
{
    [Fact(DisplayName = "No712_正常系：正常動作の確認_Term()・End()が正常終了する状態で、forced=falseを指定してメソッドを呼び出す")]
    public async Task Case712()
    {
        using var patch = new EnginePatch();
        var engine = new ApplicationEngine();

        var result = await engine.Terminate(false);

        Assert.True(result);
        Assert.Equal(new[] { "Term(False)", "End(False)" }, EnginePatch.Calls);
    }

    [Fact(DisplayName = "No713_正常系：正常動作の確認_Term()・End()が正常終了する状態で、forced=trueを指定してメソッドを呼び出す")]
    public async Task Case713()
    {
        using var patch = new EnginePatch();
        var engine = new ApplicationEngine();

        var result = await engine.Terminate(true);

        Assert.True(result);
        Assert.Equal(new[] { "Term(True)", "End(True)" }, EnginePatch.Calls);
    }

    [Fact(DisplayName = "No714_正常系：正常動作の確認_Term()・End()が正常終了する状態で、forcedを指定せず（デフォルト：false）、メソッドを呼び出す")]
    public async Task Case714()
    {
        using var patch = new EnginePatch();
        var engine = new ApplicationEngine();

        var result = await engine.Terminate();

        Assert.True(result);
        Assert.Equal(new[] { "Term(False)", "End(False)" }, EnginePatch.Calls);
    }

    private sealed class EnginePatch : IDisposable
    {
        private readonly Harmony harmony;

        public static List<string> Calls { get; } = new();

        public EnginePatch()
        {
            Calls.Clear();

            harmony = new Harmony($"{nameof(ApplicationEngine_Terminate)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ApplicationEngine), nameof(ApplicationEngine.Term)),
                prefix: new HarmonyMethod(typeof(EnginePatch), nameof(TermPrefix)));
            harmony.Patch(
                AccessTools.Method(typeof(ApplicationEngine), nameof(ApplicationEngine.End)),
                prefix: new HarmonyMethod(typeof(EnginePatch), nameof(EndPrefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool TermPrefix(bool __0, ref Task<bool> __result)
        {
            Calls.Add($"Term({__0})");
            __result = Task.FromResult(true);
            return false;
        }

        private static bool EndPrefix(bool __0, ref Task<bool> __result)
        {
            Calls.Add($"End({__0})");
            __result = Task.FromResult(true);
            return false;
        }
    }
}
