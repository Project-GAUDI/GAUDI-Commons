using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngine_IsTerminating
{
    [Fact(DisplayName = "No663_正常系：正常動作の確認_IsTerminatingがfalseの状態で、メソッドを呼び出す")]
    public void Case663()
    {
        var engine = new ApplicationEngine();
        GetStateController(engine).IsTerminating = false;

        var result = engine.IsTerminating();

        Assert.False(result);
    }

    [Fact(DisplayName = "No664_正常系：正常動作の確認_IsTerminatingがtrueの状態で、メソッドを呼び出す")]
    public void Case664()
    {
        var engine = new ApplicationEngine();
        GetStateController(engine).IsTerminating = true;

        var result = engine.IsTerminating();

        Assert.True(result);
    }

    private static StateController GetStateController(ApplicationEngine engine)
    {
        return (StateController)typeof(ApplicationEngine)
            .GetField("stateController", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
    }
}
