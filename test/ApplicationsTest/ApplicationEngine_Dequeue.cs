using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_Dequeue))]
public class ApplicationEngine_Dequeue
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_Dequeue(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No075_現在の状態がStart場合")]
    public void Applications_Case075()
    {
        ApplicationEngine applicationEngine = new();
        ApplicationStateChangeResult ret = applicationEngine.Dequeue();
        Assert.Equal(ApplicationStateChangeResult.Success, ret);
    }

    [Fact(DisplayName = "Applications No076_現在の状態がInitializeの状態の場合")]
    public void Applications_Case076()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Initialize);

        ApplicationStateChangeResult ret = applicationEngine.Dequeue();
        Assert.Equal(ApplicationStateChangeResult.Success, ret);
    }

    [Fact(DisplayName = "Applications No077_現在の状態がReadyの状態の場合")]
    public void Applications_Case077()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        ApplicationStateChangeResult ret = applicationEngine.Dequeue();
        Assert.Equal(ApplicationStateChangeResult.Success, ret);
    }

    [Fact(DisplayName = "Applications No078_現在の状態がRunningの状態の場合")]
    public void Applications_Case078()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Running);

        ApplicationStateChangeResult ret = applicationEngine.Dequeue();
        Assert.Equal(ApplicationStateChangeResult.Success, ret);
    }

    [Fact(DisplayName = "Applications No079_現在の状態がTerminateの状態の場合")]
    public void Applications_Case079()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Terminate);

        ApplicationStateChangeResult ret = applicationEngine.Dequeue();
        Assert.Equal(ApplicationStateChangeResult.Success, ret);
    }

    [Fact(DisplayName = "Applications No080_現在の状態がEndの状態の場合")]
    public void Applications_Case080()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.End);

        ApplicationStateChangeResult ret = applicationEngine.Dequeue();
        Assert.Equal(ApplicationStateChangeResult.Success, ret);
    }

    [Fact(DisplayName = "Applications No081_現在の状態が上記以外の場合")]
    public void Applications_Case081()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, (ApplicationState)99);

        ApplicationStateChangeResult ret = applicationEngine.Dequeue();
        Assert.Equal(ApplicationStateChangeResult.Ignored, ret);
    }
}