using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngine_Dequeue
{
    [Fact(DisplayName = "No717_正常系：正常動作の確認_CurrentStateがStartの状態で、メソッドを呼び出す")]
    public void Case717()
    {
        AssertDequeueSuccess(ApplicationState.Start);
    }

    [Fact(DisplayName = "No718_正常系：正常動作の確認_CurrentStateがInitializeの状態で、メソッドを呼び出す")]
    public void Case718()
    {
        AssertDequeueSuccess(ApplicationState.Initialize);
    }

    [Fact(DisplayName = "No719_正常系：正常動作の確認_CurrentStateがReadyの状態で、メソッドを呼び出す")]
    public void Case719()
    {
        AssertDequeueSuccess(ApplicationState.Ready);
    }

    [Fact(DisplayName = "No720_正常系：正常動作の確認_CurrentStateがRunningの状態で、メソッドを呼び出す")]
    public void Case720()
    {
        AssertDequeueSuccess(ApplicationState.Running);
    }

    [Fact(DisplayName = "No721_正常系：正常動作の確認_CurrentStateがTerminateの状態で、メソッドを呼び出す")]
    public void Case721()
    {
        AssertDequeueSuccess(ApplicationState.Terminate);
    }

    [Fact(DisplayName = "No722_正常系：正常動作の確認_CurrentStateがEndの状態で、メソッドを呼び出す")]
    public void Case722()
    {
        AssertDequeueSuccess(ApplicationState.End);
    }

    private static void AssertDequeueSuccess(ApplicationState state)
    {
        var engine = new ApplicationEngine();
        var stateController = (StateController)typeof(ApplicationEngine)
            .GetField("stateController", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
        typeof(StateController)
            .GetField("currentState", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(stateController, state);

        var result = engine.Dequeue();

        Assert.Equal(ApplicationStateChangeResult.Success, result);
        Assert.Equal(state, stateController.CurrentState);
    }
}
