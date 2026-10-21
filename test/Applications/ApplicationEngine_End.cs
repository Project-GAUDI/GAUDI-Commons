using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngine_End
{
    [Fact(DisplayName = "No696_正常系：正常動作の確認_CurrentStateがTerminate（Endへ遷移可能）、engineCancellerが設定済みの状態で、メソッドを呼び出す")]
    public async Task Case696()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Terminate);
        using var canceller = new CancellationTokenSource();
        SetField(engine, "engineCanceller", canceller);

        var result = await engine.End();

        Assert.True(result);
        Assert.True(canceller.IsCancellationRequested);
        Assert.Null(GetField(engine, "engineCanceller"));
        Assert.Equal(ApplicationState.End, GetStateController(engine).CurrentState);
    }

    [Fact(DisplayName = "No697_正常系：正常動作の確認_CurrentStateがTerminate（Endへ遷移可能）、engineCancellerが未設定(null)の状態で、メソッドを呼び出す")]
    public async Task Case697()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Terminate);
        SetField(engine, "engineCanceller", null);

        var result = await engine.End();

        Assert.True(result);
        Assert.Null(GetField(engine, "engineCanceller"));
        Assert.Equal(ApplicationState.End, GetStateController(engine).CurrentState);
    }

    [Fact(DisplayName = "No698_正常系：正常動作の確認_CurrentStateがStart（Endへ遷移不可）の状態で、forced=trueを指定してメソッドを呼び出す")]
    public async Task Case698()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Start);
        using var canceller = new CancellationTokenSource();
        SetField(engine, "engineCanceller", canceller);

        var result = await engine.End(true);

        Assert.True(result);
        Assert.True(canceller.IsCancellationRequested);
        Assert.Null(GetField(engine, "engineCanceller"));
        Assert.Equal(ApplicationState.Start, GetStateController(engine).CurrentState);
    }

    [Fact(DisplayName = "No699_異常系：異常動作の確認_CurrentStateがStart（Endへ遷移不可）の状態で、forced=falseを指定してメソッドを呼び出す")]
    public async Task Case699()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Start);
        using var canceller = new CancellationTokenSource();
        SetField(engine, "engineCanceller", canceller);

        var exception = await Assert.ThrowsAsync<Exception>(() => engine.End(false));

        Assert.Equal("Error: Failed to change the ApplicationState to 'End'.", exception.Message);
        Assert.False(canceller.IsCancellationRequested);
        Assert.Same(canceller, GetField(engine, "engineCanceller"));
    }

    [Fact(DisplayName = "No700_異常系：異常動作の確認_CurrentStateがStart（Endへ遷移不可）の状態で、forcedを指定せず（デフォルト：false）、メソッドを呼び出す")]
    public async Task Case700()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Start);

        var exception = await Assert.ThrowsAsync<Exception>(() => engine.End());

        Assert.Equal("Error: Failed to change the ApplicationState to 'End'.", exception.Message);
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
