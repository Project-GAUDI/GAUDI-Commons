using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_SetApplicationRestartAsync))]
public class ApplicationEngine_SetApplicationRestartAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_SetApplicationRestartAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No015_呼び出し後、Restart()を呼び出し、再起動に成功した場合", Skip = "MyModuleClientがnullのためRestartできないためスキップ")]
    public async void Applications_Case015()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        ApplicationStateChangeResult applicationpplicationStateChangeResult = await applicationEngine.SetApplicationRestartAsync();
        Assert.Equal(ApplicationStateChangeResult.Success, applicationpplicationStateChangeResult);
    }

    [Fact(DisplayName = "Applications No016_呼び出し後、Restart()を呼び出し、再起動に失敗した場合")]
    public async void Applications_Case016()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        ApplicationStateChangeResult applicationpplicationStateChangeResult = await applicationEngine.SetApplicationRestartAsync();
        Assert.Equal(ApplicationStateChangeResult.Ignored, applicationpplicationStateChangeResult);
    }
}
