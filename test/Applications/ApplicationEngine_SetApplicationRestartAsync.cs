using System.Reflection;
using HarmonyLib;
using Microsoft.Azure.Devices.Shared;
using Moq;
using Newtonsoft.Json.Linq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// Restart内でLoggerの静的なModuleClientを書き換えるため、Logger系テストと直列化する
[Collection(nameof(SharedLoggerState))]
public sealed class ApplicationEngine_SetApplicationRestartAsync
{
    [Fact(DisplayName = "No660_正常系：正常動作の確認_再起動処理が成功するapplicationMainとMyModuleClientを設定し、再起動可能な状態でメソッドを呼び出す")]
    public async Task Case660()
    {
        using var patch = new ModuleClientFactoryPatch();
        var engine = new ApplicationEngine();
        var app = NewApplicationMain();
        var oldClient = new Mock<IModuleClient>();
        engine.SetApplication(app.Object);
        SetField(engine, "MyModuleClient", oldClient.Object);
        SetCurrentState(engine, ApplicationState.Ready);

        var result = await engine.SetApplicationRestartAsync();

        Assert.Equal(ApplicationStateChangeResult.Success, result);
        app.Verify(a => a.TerminateAsync(), Times.Once);
        oldClient.Verify(c => c.CloseAsync(), Times.Once);
        oldClient.Verify(c => c.Dispose(), Times.Once);
        app.Verify(a => a.InitializeAsync(), Times.Once);
        app.Verify(a => a.StartAsync(), Times.Once);
        Assert.Same(patch.ModuleClient.Object, GetField(engine, "MyModuleClient"));
        Assert.Equal(ApplicationState.Ready, GetStateController(engine).CurrentState);
    }

    [Fact(DisplayName = "No661_異常系：異常動作の確認_applicationMainをnullにして、メソッドを呼び出す")]
    public async Task Case661()
    {
        var engine = new ApplicationEngine();
        engine.SetApplication(null!);

        var result = await engine.SetApplicationRestartAsync();

        Assert.Equal(ApplicationStateChangeResult.Ignored, result);
    }

    [Fact(DisplayName = "No662_異常系：異常動作の確認_InitializeAsyncがfalseを返すapplicationMainを設定して、メソッドを呼び出す")]
    public async Task Case662()
    {
        using var patch = new ModuleClientFactoryPatch();
        var engine = new ApplicationEngine();
        var app = NewApplicationMain();
        app.Setup(a => a.InitializeAsync()).ReturnsAsync(false);
        engine.SetApplication(app.Object);
        SetField(engine, "MyModuleClient", new Mock<IModuleClient>().Object);
        SetCurrentState(engine, ApplicationState.Ready);

        var result = await engine.SetApplicationRestartAsync();

        Assert.Equal(ApplicationStateChangeResult.Ignored, result);
        app.Verify(a => a.InitializeAsync(), Times.Once);
        app.Verify(a => a.StartAsync(), Times.Never);
        Assert.Equal(ApplicationState.Initialize, GetStateController(engine).CurrentState);
    }

    private static Mock<IApplicationMain> NewApplicationMain()
    {
        var app = new Mock<IApplicationMain>();
        app.Setup(a => a.InitializeAsync()).ReturnsAsync(true);
        app.Setup(a => a.StartAsync()).ReturnsAsync(true);
        app.Setup(a => a.TerminateAsync()).ReturnsAsync(true);
        app.Setup(a => a.OnDesiredPropertiesReceivedAsync(It.IsAny<JObject>())).ReturnsAsync(true);
        return app;
    }

    private static StateController GetStateController(ApplicationEngine engine)
    {
        return (StateController)GetField(engine, "stateController")!;
    }

    private static void SetCurrentState(ApplicationEngine engine, ApplicationState state)
    {
        typeof(StateController)
            .GetField("currentState", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(GetStateController(engine), state);
    }

    private static object? GetField(ApplicationEngine engine, string name)
    {
        return typeof(ApplicationEngine).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(engine);
    }

    private static void SetField(ApplicationEngine engine, string name, object? value)
    {
        typeof(ApplicationEngine).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(engine, value);
    }

    private sealed class ModuleClientFactoryPatch : IDisposable
    {
        private readonly Harmony harmony;

        private static ModuleClientFactoryPatch? current;

        public Mock<IModuleClient> ModuleClient { get; } = new();

        public ModuleClientFactoryPatch()
        {
            ModuleClient.Setup(c => c.GetTwinAsync()).ReturnsAsync(new Twin());
            current = this;

            harmony = new Harmony($"{nameof(ApplicationEngine_SetApplicationRestartAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClientFactory), nameof(ModuleClientFactory.CreateAsync)),
                prefix: new HarmonyMethod(typeof(ModuleClientFactoryPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
            current = null;
        }

        private static bool Prefix(ref Task<IModuleClient> __result)
        {
            __result = Task.FromResult(current!.ModuleClient.Object);
            return false;
        }
    }
}
