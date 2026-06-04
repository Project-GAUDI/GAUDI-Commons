using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_UnsetApplicationRunningAsync))]
public class ApplicationEngine_UnsetApplicationRunningAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_UnsetApplicationRunningAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No013_ステートの状態を待機状態にできた場合")]
    public async void Applications_Case013()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Initialize);

        ApplicationStateChangeResult applicationpplicationStateChangeResult = await applicationEngine.UnsetApplicationRunningAsync();
        Assert.Equal(ApplicationStateChangeResult.Success, applicationpplicationStateChangeResult);
    }

    [Fact(DisplayName = "Applications No014_ステートの状態を実行状態にできなかった場合")]
    public async void Applications_Case014()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        ApplicationStateChangeResult applicationpplicationStateChangeResult = await applicationEngine.UnsetApplicationRunningAsync();
        Assert.Equal(ApplicationStateChangeResult.Ignored, applicationpplicationStateChangeResult);
    }
}
