using System.Reflection;
using HarmonyLib;
using Microsoft.Azure.Devices.Shared;
using Moq;
using Newtonsoft.Json.Linq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// Init内でLoggerの静的なModuleClientを書き換えるため、Logger系テストと直列化する
[Collection(nameof(SharedLoggerState))]
public sealed class ApplicationEngine_RunAsync
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [Fact(DisplayName = "No651_正常系：正常動作の確認_正常動作するapplicationMainとMyModuleClientを設定してメソッドを呼び出し、その後、engineCancellerをキャンセルする")]
    public async Task Case651()
    {
        using var patch = new ModuleClientFactoryPatch();
        var engine = new ApplicationEngine();
        var app = NewApplicationMain();
        engine.SetApplication(app.Object);

        var runTask = engine.RunAsync();
        Assert.True(SpinWait.SpinUntil(() => GetField(engine, "engineCanceller") != null, Timeout));

        Assert.False(runTask.IsCompleted);
        Assert.Equal(ApplicationState.Ready, GetStateController(engine).CurrentState);
        app.Verify(a => a.InitializeAsync(), Times.Once);
        app.Verify(a => a.StartAsync(), Times.Once);
        app.Verify(a => a.TerminateAsync(), Times.Never);

        ((CancellationTokenSource)GetField(engine, "engineCanceller")!).Cancel();
        await runTask.WaitAsync(Timeout, TestContext.Current.CancellationToken);

        app.Verify(a => a.TerminateAsync(), Times.Once);
        patch.ModuleClient.Verify(c => c.OpenAsync(), Times.Once);
        patch.ModuleClient.Verify(c => c.CloseAsync(), Times.Once);
        patch.ModuleClient.Verify(c => c.Dispose(), Times.Once);
        Assert.Equal(ApplicationState.End, GetStateController(engine).CurrentState);
        Assert.Null(GetField(engine, "engineCanceller"));
    }

    [Fact(DisplayName = "No652_正常系：正常動作の確認_InitializeAsyncがfalseを返すapplicationMainを設定して、メソッドを呼び出す")]
    public async Task Case652()
    {
        using var patch = new ModuleClientFactoryPatch();
        var engine = new ApplicationEngine();
        var app = NewApplicationMain();
        app.Setup(a => a.InitializeAsync()).ReturnsAsync(false);
        engine.SetApplication(app.Object);

        await engine.RunAsync().WaitAsync(Timeout, TestContext.Current.CancellationToken);

        app.Verify(a => a.StartAsync(), Times.Never);
        app.Verify(a => a.TerminateAsync(), Times.Never);
        Assert.Equal(ApplicationState.Initialize, GetStateController(engine).CurrentState);
        Assert.Null(GetField(engine, "engineCanceller"));
    }

    [Fact(DisplayName = "No653_異常系：異常動作の確認_Init内で呼ばれるInitializeAsyncが例外を送出するapplicationMainを設定して、メソッドを呼び出す")]
    public async Task Case653()
    {
        using var patch = new ModuleClientFactoryPatch();
        var engine = new ApplicationEngine();
        var app = NewApplicationMain();
        app.Setup(a => a.InitializeAsync()).ThrowsAsync(new InvalidOperationException("initialize error"));
        engine.SetApplication(app.Object);

        var exception = await Record.ExceptionAsync(() => engine.RunAsync().WaitAsync(Timeout, TestContext.Current.CancellationToken));

        Assert.Null(exception);
        Assert.Equal(0, patch.CreateAsyncCallCount);
        Assert.Equal(ApplicationState.Initialize, GetStateController(engine).CurrentState);
        Assert.Null(GetField(engine, "engineCanceller"));
    }

    [Fact(DisplayName = "No654_異常系：異常動作の確認_applicationMainを設定せずにメソッドを呼び出す")]
    public async Task Case654()
    {
        var engine = new ApplicationEngine();

        await Assert.ThrowsAsync<Exception>(() => engine.RunAsync());
    }

    [Fact(DisplayName = "No655_異常系：異常動作の確認_Init成功後にReadyへの状態遷移が失敗する状態を設定して、メソッドを呼び出す")]
    public async Task Case655()
    {
        using var patch = new ModuleClientFactoryPatch();
        var engine = new ApplicationEngine();
        var app = NewApplicationMain();
        // 終了中フラグを立てると Initialize -> Ready の遷移が不可になる
        app.Setup(a => a.StartAsync())
            .Callback(() => GetStateController(engine).IsTerminating = true)
            .ReturnsAsync(true);
        engine.SetApplication(app.Object);

        var exception = await Record.ExceptionAsync(() => engine.RunAsync().WaitAsync(Timeout, TestContext.Current.CancellationToken));

        Assert.Null(exception);
        app.Verify(a => a.StartAsync(), Times.Once);
        app.Verify(a => a.TerminateAsync(), Times.Never);
        Assert.Equal(ApplicationState.Initialize, GetStateController(engine).CurrentState);
        Assert.Null(GetField(engine, "engineCanceller"));
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

    private static object? GetField(ApplicationEngine engine, string name)
    {
        return typeof(ApplicationEngine).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(engine);
    }

    private sealed class ModuleClientFactoryPatch : IDisposable
    {
        private readonly Harmony harmony;

        private static ModuleClientFactoryPatch? current;

        public Mock<IModuleClient> ModuleClient { get; } = new();

        public int CreateAsyncCallCount { get; private set; }

        public ModuleClientFactoryPatch()
        {
            ModuleClient.Setup(c => c.GetTwinAsync()).ReturnsAsync(new Twin());
            current = this;

            harmony = new Harmony($"{nameof(ApplicationEngine_RunAsync)}.{Guid.NewGuid()}");
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
            current!.CreateAsyncCallCount++;
            __result = Task.FromResult(current.ModuleClient.Object);
            return false;
        }
    }
}
