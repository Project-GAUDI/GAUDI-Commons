using System.Reflection;
using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// Loggerの出力先（静的状態）を差し替えるため、Logger系テストと直列化する
[Collection(nameof(SharedLoggerState))]
public sealed class ApplicationEngine_ReceiveMessageAsync
{
    [Fact(DisplayName = "No723_正常系：正常動作の確認_CurrentStateがReady（Runningへ遷移可能）、対応するハンドラがtrueを返す状態で、userContextに登録済みのInput名を指定して、メソッドを呼び出す")]
    public async Task Case723()
    {
        var engine = NewEngine(ApplicationState.Ready);
        var received = new List<(string InputName, IotMessage Message, object Context)>();
        var userContext = new object();
        RegisterHandler(engine, "input1", userContext, (name, msg, ctx) =>
        {
            received.Add((name, msg, ctx));
            return Task.FromResult(true);
        });
        var message = new IotMessage("body");

        var result = await engine.ReceiveMessageAsync(message, "input1");

        Assert.Equal(MessageResponse.Completed, result);
        Assert.Equal(ApplicationState.Ready, GetStateController(engine).CurrentState);
        var call = Assert.Single(received);
        Assert.Equal("input1", call.InputName);
        Assert.Same(message, call.Message);
        Assert.Same(userContext, call.Context);
    }

    [Fact(DisplayName = "No724_異常系：異常動作の確認_現在のステートがStart（Runningへ遷移不可）の状態で、メソッドを呼び出す")]
    public async Task Case724()
    {
        using var log = new LoggerOutputCapture();
        var engine = NewEngine(ApplicationState.Start);
        var called = false;
        RegisterHandler(engine, "input1", null, (name, msg, ctx) =>
        {
            called = true;
            return Task.FromResult(true);
        });

        var result = await engine.ReceiveMessageAsync(new IotMessage("body"), "input1");

        Assert.Equal(MessageResponse.None, result);
        Assert.False(called);
        Assert.Contains("Failed to change the ApplicationState to 'Running'.", log.Output);
    }

    [Fact(DisplayName = "No725_正常系：正常動作の確認_CurrentStateがReady（Runningへ遷移可能）の状態で、userContextに未登録のInput名を指定して、メソッドを呼び出す")]
    public async Task Case725()
    {
        using var log = new LoggerOutputCapture();
        var engine = NewEngine(ApplicationState.Ready);

        var result = await engine.ReceiveMessageAsync(new IotMessage("body"), "unknown");

        Assert.Equal(MessageResponse.Abandoned, result);
        Assert.Equal(ApplicationState.Ready, GetStateController(engine).CurrentState);
        Assert.Contains("ReceiveMessageAsync failed.", log.Output);
    }

    [Fact(DisplayName = "No726_正常系：正常動作の確認_CurrentStateがReady（Runningへ遷移可能）、対応するハンドラがfalseを返す状態で、userContextに登録済みのInput名を指定して、メソッドを呼び出す")]
    public async Task Case726()
    {
        var engine = NewEngine(ApplicationState.Ready);
        RegisterHandler(engine, "input1", null, (name, msg, ctx) => Task.FromResult(false));

        var result = await engine.ReceiveMessageAsync(new IotMessage("body"), "input1");

        Assert.Equal(MessageResponse.None, result);
        Assert.Equal(ApplicationState.Ready, GetStateController(engine).CurrentState);
    }

    [Fact(DisplayName = "No727_異常系：異常動作の確認_CurrentStateがReady（Runningへ遷移可能）、対応するハンドラ呼び出し中に例外が発生する状態で、userContextに登録済みのInput名を指定して、メソッドを呼び出す")]
    public async Task Case727()
    {
        using var log = new LoggerOutputCapture();
        var engine = NewEngine(ApplicationState.Ready);
        RegisterHandler(engine, "input1", null, (name, msg, ctx) => throw new InvalidOperationException("handler error"));

        var result = await engine.ReceiveMessageAsync(new IotMessage("body"), "input1");

        Assert.Equal(MessageResponse.Abandoned, result);
        Assert.Equal(ApplicationState.Ready, GetStateController(engine).CurrentState);
        Assert.Contains("ReceiveMessageAsync failed.", log.Output);
        Assert.Contains("handler error", log.Output);
    }

    private static ApplicationEngine NewEngine(ApplicationState state)
    {
        var engine = new ApplicationEngine();
        typeof(StateController)
            .GetField("currentState", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(GetStateController(engine), state);
        return engine;
    }

    private static void RegisterHandler(ApplicationEngine engine, string inputName, object? userContext, MessageEventHandler handler)
    {
        var eventData = (Dictionary<string, MessageEventData>)typeof(ApplicationEngine)
            .GetField("messageInputEventData", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
        eventData[inputName] = new MessageEventData(inputName, userContext!, handler);
    }

    private static StateController GetStateController(ApplicationEngine engine)
    {
        return (StateController)typeof(ApplicationEngine)
            .GetField("stateController", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
    }
}
