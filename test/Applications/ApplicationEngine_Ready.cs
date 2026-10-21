using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngine_Ready
{
    [Fact(DisplayName = "No694_正常系：正常動作の確認_CurrentStateがInitialize（Readyへ遷移可能）の状態で、メソッドを呼び出す")]
    public async Task Case694()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Initialize);

        var result = await engine.Ready();

        Assert.True(result);
        Assert.Equal(ApplicationState.Ready, GetStateController(engine).CurrentState);
    }

    [Fact(DisplayName = "No695_異常系：異常動作の確認_CurrentStateがStart（Readyへ遷移不可）の状態で、メソッドを呼び出す")]
    public async Task Case695()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Start);

        var exception = await Assert.ThrowsAsync<Exception>(() => engine.Ready());

        Assert.Equal("Error: Failed to change the ApplicationState to 'Ready'.", exception.Message);
        Assert.Equal(ApplicationState.Start, GetStateController(engine).CurrentState);
    }

    private static StateController GetStateController(ApplicationEngine engine)
    {
        return (StateController)typeof(ApplicationEngine)
            .GetField("stateController", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
    }

    private static void SetCurrentState(ApplicationEngine engine, ApplicationState state)
    {
        typeof(StateController)
            .GetField("currentState", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(GetStateController(engine), state);
    }
}
