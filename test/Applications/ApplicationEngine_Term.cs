using System.Reflection;
using Microsoft.Azure.Devices.Client;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// TermはLoggerの静的なModuleClientをクリアするため、Logger系テストと直列化する
[Collection(nameof(SharedLoggerState))]
public sealed class ApplicationEngine_Term
{
    [Fact(DisplayName = "No701_正常系：正常動作の確認_CurrentStateがReady（Terminateへ遷移可能）、applicationMain・MyModuleClientが設定済みの状態で、メソッドを呼び出す")]
    public async Task Case701()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Ready);
        var app = NewApplicationMain();
        var client = new Mock<IModuleClient>();
        engine.SetApplication(app.Object);
        SetField(engine, "MyModuleClient", client.Object);
        AddEventData(engine);

        var result = await engine.Term();

        Assert.True(result);
        app.Verify(a => a.TerminateAsync(), Times.Once);
        client.Verify(c => c.SetInputMessageHandlerAsync("input1", null!, null!), Times.Once);
        client.Verify(c => c.SetMethodHandlerAsync("method1", null!, null!), Times.Once);
        client.Verify(c => c.SetDesiredPropertyUpdateCallbackAsync(null!, null!), Times.Once);
        client.Verify(c => c.CloseAsync(), Times.Once);
        client.Verify(c => c.Dispose(), Times.Once);
        Assert.Null(GetField(engine, "MyModuleClient"));
        Assert.Equal(ApplicationState.Terminate, GetStateController(engine).CurrentState);
    }

    [Fact(DisplayName = "No702_正常系：正常動作の確認_CurrentStateがReady（Terminateへ遷移可能）、applicationMainが未設定(null)の状態で、メソッドを呼び出す")]
    public async Task Case702()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Ready);
        var client = new Mock<IModuleClient>();
        engine.SetApplication(null!);
        SetField(engine, "MyModuleClient", client.Object);

        var result = await engine.Term();

        Assert.True(result);
        client.Verify(c => c.CloseAsync(), Times.Once);
        client.Verify(c => c.Dispose(), Times.Once);
        Assert.Null(GetField(engine, "MyModuleClient"));
    }

    [Fact(DisplayName = "No703_正常系：正常動作の確認_CurrentStateがReady（Terminateへ遷移可能）、MyModuleClientが未設定(null)の状態で、メソッドを呼び出す")]
    public async Task Case703()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Ready);
        var app = NewApplicationMain();
        engine.SetApplication(app.Object);
        SetField(engine, "MyModuleClient", null);
        AddEventData(engine);

        var result = await engine.Term();

        Assert.True(result);
        app.Verify(a => a.TerminateAsync(), Times.Once);
        Assert.Null(GetField(engine, "MyModuleClient"));
        // ModuleClient解放処理内で行われるイベントデータのクリアもスキップされる
        Assert.Single(GetMessageInputEventData(engine));
        Assert.Single(GetMethodRequestEventData(engine));
    }

    [Fact(DisplayName = "No704_正常系：正常動作の確認_CurrentStateがReady（Terminateへ遷移可能）、messageInputEventData、methodRequestEventDataにそれぞれ1件以上登録済みの状態で、メソッドを呼び出す")]
    public async Task Case704()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Ready);
        engine.SetApplication(NewApplicationMain().Object);
        SetField(engine, "MyModuleClient", new Mock<IModuleClient>().Object);
        AddEventData(engine);

        var result = await engine.Term();

        Assert.True(result);
        Assert.Empty(GetMessageInputEventData(engine));
        Assert.Empty(GetMethodRequestEventData(engine));
    }

    [Fact(DisplayName = "No705_正常系：正常動作の確認_CurrentStateがStart（Terminateへ遷移不可）の状態で、forced=trueを指定してメソッドを呼び出す")]
    public async Task Case705()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Start);
        var app = NewApplicationMain();
        var client = new Mock<IModuleClient>();
        engine.SetApplication(app.Object);
        SetField(engine, "MyModuleClient", client.Object);

        var result = await engine.Term(true);

        Assert.True(result);
        app.Verify(a => a.TerminateAsync(), Times.Once);
        client.Verify(c => c.Dispose(), Times.Once);
        Assert.Equal(ApplicationState.Start, GetStateController(engine).CurrentState);
    }

    [Fact(DisplayName = "No706_異常系：異常動作の確認_CurrentStateがStart（Terminateへ遷移不可）の状態で、forced=falseを指定してメソッドを呼び出す")]
    public async Task Case706()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Start);
        var app = NewApplicationMain();
        var client = new Mock<IModuleClient>();
        engine.SetApplication(app.Object);
        SetField(engine, "MyModuleClient", client.Object);

        var exception = await Assert.ThrowsAsync<Exception>(() => engine.Term(false));

        Assert.Equal("Error: Failed to change the ApplicationState to 'Terminate'.", exception.Message);
        app.Verify(a => a.TerminateAsync(), Times.Never);
        client.Verify(c => c.Dispose(), Times.Never);
        Assert.Same(client.Object, GetField(engine, "MyModuleClient"));
    }

    [Fact(DisplayName = "No707_異常系：異常動作の確認_CurrentStateがStart（Terminateへ遷移不可）の状態で、forcedを指定せず（デフォルト：false）、メソッドを呼び出す")]
    public async Task Case707()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Start);

        var exception = await Assert.ThrowsAsync<Exception>(() => engine.Term());

        Assert.Equal("Error: Failed to change the ApplicationState to 'Terminate'.", exception.Message);
    }

    private static Mock<IApplicationMain> NewApplicationMain()
    {
        var app = new Mock<IApplicationMain>();
        app.Setup(a => a.TerminateAsync()).ReturnsAsync(true);
        return app;
    }

    private static void AddEventData(ApplicationEngine engine)
    {
        GetMessageInputEventData(engine)["input1"] = new MessageEventData("input1", null!, null!);
        GetMethodRequestEventData(engine)["method1"] = new MethodEventData("method1", null!, null!);
    }

    private static Dictionary<string, MessageEventData> GetMessageInputEventData(ApplicationEngine engine)
    {
        return (Dictionary<string, MessageEventData>)GetField(engine, "messageInputEventData")!;
    }

    private static Dictionary<string, MethodEventData> GetMethodRequestEventData(ApplicationEngine engine)
    {
        return (Dictionary<string, MethodEventData>)GetField(engine, "methodRequestEventData")!;
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
}
