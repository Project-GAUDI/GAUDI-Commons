using System.Reflection;
using System.Text;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json.Linq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// Loggerの出力先（静的状態）を差し替えるため、Logger系テストと直列化する
[Collection(nameof(SharedLoggerState))]
public sealed class ApplicationEngine_ReceiveMethodAsync
{
    [Fact(DisplayName = "No731_正常系：正常動作の確認_CurrentStateがReady（Runningへ遷移可能）、対応するハンドラがDirectMethodResponseを返す状態で、userContextに登録済みのメソッド名を指定して、メソッドを呼び出す")]
    public async Task Case731()
    {
        var engine = NewEngine(ApplicationState.Ready);
        var received = new List<(string MethodName, DirectMethodRequest Request, object Context)>();
        var userContext = new object();
        RegisterHandler(engine, "method1", userContext, (name, req, ctx) =>
        {
            received.Add((name, req, ctx));
            return Task.FromResult(new DirectMethodResponse(200, "Result", "OK"));
        });
        var request = new MethodRequest("method1", Encoding.UTF8.GetBytes("{\"key\":\"value\"}"));

        var result = await engine.ReceiveMethodAsync(request, "method1");

        Assert.Equal(200, result.Status);
        Assert.Equal("OK", JObject.Parse(result.ResultAsJson)["Result"]!.ToString());
        Assert.Equal(ApplicationState.Ready, GetStateController(engine).CurrentState);
        var call = Assert.Single(received);
        Assert.Equal("method1", call.MethodName);
        Assert.Equal("method1", call.Request.MethodName);
        Assert.Equal("{\"key\":\"value\"}", call.Request.RequestJson);
        Assert.Same(userContext, call.Context);
    }

    [Fact(DisplayName = "No732_異常系：異常動作の確認_現在のステートがStart（Runningへ遷移不可）の状態で、メソッドを呼び出す")]
    public async Task Case732()
    {
        using var log = new LoggerOutputCapture();
        var engine = NewEngine(ApplicationState.Start);
        var called = false;
        RegisterHandler(engine, "method1", null, (name, req, ctx) =>
        {
            called = true;
            return Task.FromResult(new DirectMethodResponse(200));
        });

        var result = await engine.ReceiveMethodAsync(new MethodRequest("method1"), "method1");

        const string expected = "Calling method1 method failed.(Failed to change the ApplicationState to 'Running')";
        Assert.Equal(-1, result.Status);
        Assert.Equal(expected, JObject.Parse(result.ResultAsJson)["Error"]!.ToString());
        Assert.False(called);
        Assert.Contains(expected, log.Output);
    }

    [Fact(DisplayName = "No733_正常系：正常動作の確認_CurrentStateがReady（Runningへ遷移可能）、対応するハンドラがDirectMethodResponseを返す状態で、userContextに未登録のメソッド名を指定して、メソッドを呼び出す")]
    public async Task Case733()
    {
        using var log = new LoggerOutputCapture();
        var engine = NewEngine(ApplicationState.Ready);
        RegisterHandler(engine, "method1", null, (name, req, ctx) => Task.FromResult(new DirectMethodResponse(200)));

        var result = await engine.ReceiveMethodAsync(new MethodRequest("unknown"), "unknown");

        Assert.Equal(-1, result.Status);
        Assert.StartsWith("Calling unknown method failed.", JObject.Parse(result.ResultAsJson)["Error"]!.ToString());
        Assert.Equal(ApplicationState.Ready, GetStateController(engine).CurrentState);
        Assert.Contains("Calling unknown method failed.", log.Output);
    }

    [Fact(DisplayName = "No734_異常系：異常動作の確認_CurrentStateがReady（Runningへ遷移可能）、対応するハンドラ呼び出し中に例外が発生する状態で、userContextに登録済みのメソッド名を指定して、メソッドを呼び出す")]
    public async Task Case734()
    {
        using var log = new LoggerOutputCapture();
        var engine = NewEngine(ApplicationState.Ready);
        RegisterHandler(engine, "method1", null, (name, req, ctx) => throw new InvalidOperationException("handler error"));

        var result = await engine.ReceiveMethodAsync(new MethodRequest("method1"), "method1");

        var error = JObject.Parse(result.ResultAsJson)["Error"]!.ToString();
        Assert.Equal(-1, result.Status);
        Assert.StartsWith("Calling method1 method failed.", error);
        Assert.Contains("handler error", error);
        Assert.Equal(ApplicationState.Ready, GetStateController(engine).CurrentState);
        Assert.Contains("Calling method1 method failed.", log.Output);
    }

    private static ApplicationEngine NewEngine(ApplicationState state)
    {
        var engine = new ApplicationEngine();
        typeof(StateController)
            .GetField("currentState", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(GetStateController(engine), state);
        return engine;
    }

    private static void RegisterHandler(ApplicationEngine engine, string methodName, object? userContext, DirectMethodHandler handler)
    {
        var eventData = (Dictionary<string, MethodEventData>)typeof(ApplicationEngine)
            .GetField("methodRequestEventData", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
        eventData[methodName] = new MethodEventData(methodName, userContext!, handler);
    }

    private static StateController GetStateController(ApplicationEngine engine)
    {
        return (StateController)typeof(ApplicationEngine)
            .GetField("stateController", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
    }
}
