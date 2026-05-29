using System;
using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_OnConnectionStatusChangedAsync))]
public class ApplicationEngine_OnConnectionStatusChangedAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_OnConnectionStatusChangedAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No100_IotConnectionStatusがDisconnected以外の場合")]
    public async void Applications_Case100()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Running);

        await applicationEngine.OnConnectionStatusChangedAsync(IotConnectionStatus.Connected, IotConnectionStatusChangeReason.Communication_Error);
        var state = (ApplicationState?)stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(stateController);
        Assert.Equal(ApplicationState.Running, state);
    }

    [Fact(DisplayName = "Applications No102_Restartで予期せぬエラーが発生した場合")]
    public async void Applications_Case102()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        var applicationEngine = new ApplicationEngine();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateControllerField = applicationEngine.GetType().GetField(
            "stateController",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
        );
        var stateController = (StateController?)stateControllerField!.GetValue(applicationEngine);

        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField(
            "currentState",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
        )!.SetValue(stateController, ApplicationState.Ready);

        // Restartで例外を発生させるために1回目のIApplicationMain.TerminateAsyncでは、意図的に例外を発生させている
        applicationMainMock.
            SetupSequence(x => x.TerminateAsync()).
            Throws(new System.Exception()).
            ReturnsAsync(true);

        await applicationEngine.OnConnectionStatusChangedAsync(IotConnectionStatus.Disconnected, IotConnectionStatusChangeReason.Communication_Error);

        // Restart失敗時の強制終了処理でのApplicationEngine.Termでステート遷移でIgnoredが返されるが、
        // 強制フラグ(Termのforced)がtureになっているためIApplicationMain.TerminateAsyncが呼び出される
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Exactly(2));
    }

    [Fact(DisplayName = "Applications No103_Restartのが正常終了した場合", Skip = "MyModuleClientがnullであり、moduleClientFactoryのCreateAsyncがstaticで定義されておりMockでの対応ができなかっためスキップ")]
    public void Applications_Case103()
    {
        Assert.True(true);
    }
}