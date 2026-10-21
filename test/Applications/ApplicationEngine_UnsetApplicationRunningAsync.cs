using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngine_UnsetApplicationRunningAsync
{
    [Fact(DisplayName = "No658_正常系：正常動作の確認_状態をRunningにして、メソッドを呼び出す")]
    public async Task Case658()
    {
        var engine = new ApplicationEngine();
        ApplicationEngine_SetApplicationRunningAsync.SetCurrentState(engine, ApplicationState.Running);

        var result = await engine.UnsetApplicationRunningAsync();

        Assert.Equal(ApplicationStateChangeResult.Success, result);
        Assert.Equal(ApplicationState.Ready, ApplicationEngine_SetApplicationRunningAsync.GetStateController(engine).CurrentState);
    }

    [Fact(DisplayName = "No659_異常系：異常動作の確認_状態をStartにして、メソッドを呼び出す")]
    public async Task Case659()
    {
        var engine = new ApplicationEngine();
        ApplicationEngine_SetApplicationRunningAsync.SetCurrentState(engine, ApplicationState.Start);

        var result = await engine.UnsetApplicationRunningAsync();

        Assert.Equal(ApplicationStateChangeResult.Ignored, result);
        Assert.Equal(ApplicationState.Start, ApplicationEngine_SetApplicationRunningAsync.GetStateController(engine).CurrentState);
    }
}
