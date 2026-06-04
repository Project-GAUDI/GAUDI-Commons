using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_Enqueue))]
public class ApplicationEngine_Enqueue
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_Enqueue(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No073_Enqueueを呼び出し")]
    public void Applications_Case073()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        ApplicationStateChangeResult ret = applicationEngine.Enqueue(ApplicationState.Running);
        Assert.Equal(ApplicationStateChangeResult.Success, ret);

        var state = (ApplicationState?)stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(stateController);
        Assert.Equal(ApplicationState.Running, state);
    }
}