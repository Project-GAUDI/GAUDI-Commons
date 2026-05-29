using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_SetApplicationRunningAsync))]
public class ApplicationEngine_SetApplicationRunningAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_SetApplicationRunningAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No011_ステートの状態を実行状態にできた場合")]
    public async void Applications_Case011()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        ApplicationStateChangeResult applicationpplicationStateChangeResult = await applicationEngine.SetApplicationRunningAsync();
        Assert.Equal(ApplicationStateChangeResult.Success, applicationpplicationStateChangeResult);
    }

    [Fact(DisplayName = "Applications No012_ステートの状態を実行状態にできなかった場合")]
    public async void Applications_Case012()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        ApplicationStateChangeResult applicationpplicationStateChangeResult = await applicationEngine.SetApplicationRunningAsync();
        Assert.Equal(ApplicationStateChangeResult.Ignored, applicationpplicationStateChangeResult);
    }
}
