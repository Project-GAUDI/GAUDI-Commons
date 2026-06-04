using System;
using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_Ready))]
public class ApplicationEngine_Ready
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_Ready(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No050_ステートを待機状態にできない場合")]
    public async void Applications_Case050()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        await Assert.ThrowsAsync<Exception>(applicationEngine.Ready);
    }

    [Fact(DisplayName = "Applications No052_すべての処理が正常に終了した場合")]
    public async void Applications_Case052()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Initialize);

        bool ret = await applicationEngine.Ready();
        Assert.True(ret);
    }
}