using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_Terminate))]
public class ApplicationEngine_Terminate
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_Terminate(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No072_すべての処理が正常に終了した場合")]
    public async void Applications_Case072()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        bool ret = await applicationEngine.Terminate();

        ApplicationState? afterState = (ApplicationState?)stateControllerType!.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(stateController); ;

        Assert.True(ret);
        Assert.Equal(ApplicationState.End, afterState);
    }
}