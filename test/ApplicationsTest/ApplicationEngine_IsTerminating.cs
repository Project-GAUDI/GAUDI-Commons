using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_IsTerminating))]
public class ApplicationEngine_IsTerminating
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_IsTerminating(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No017_終了中の状態で呼び出し")]
    public async void Applications_Case017()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        await applicationEngine.Terminate();
        bool ret = applicationEngine.IsTerminating();
        Assert.True(ret);
    }

    [Fact(DisplayName = "Applications No018_終了中以外の状態で呼び出し")]
    public void Applications_Case018()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        bool ret = applicationEngine.IsTerminating();
        Assert.False(ret);
    }
}
