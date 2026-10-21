using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngine_SetApplicationRunningAsync
{
    [Fact(DisplayName = "No656_正常系：正常動作の確認_状態をReadyにして、メソッドを呼び出す")]
    public async Task Case656()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Ready);

        var result = await engine.SetApplicationRunningAsync();

        Assert.Equal(ApplicationStateChangeResult.Success, result);
        Assert.Equal(ApplicationState.Running, GetStateController(engine).CurrentState);
    }

    [Fact(DisplayName = "No657_異常系：異常動作の確認_状態をStartにして、メソッドを呼び出す")]
    public async Task Case657()
    {
        var engine = new ApplicationEngine();
        SetCurrentState(engine, ApplicationState.Start);

        var result = await engine.SetApplicationRunningAsync();

        Assert.Equal(ApplicationStateChangeResult.Ignored, result);
        Assert.Equal(ApplicationState.Start, GetStateController(engine).CurrentState);
    }

    internal static StateController GetStateController(ApplicationEngine engine)
    {
        return (StateController)typeof(ApplicationEngine)
            .GetField("stateController", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
    }

    internal static void SetCurrentState(ApplicationEngine engine, ApplicationState state)
    {
        typeof(StateController)
            .GetField("currentState", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(GetStateController(engine), state);
    }
}
