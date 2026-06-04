using System;
using Xunit;
using Xunit.Abstractions;
using System.Threading;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_End))]
public class ApplicationEngine_End
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_End(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No053_ステートを終了状態にできない場合、かつ、強制終了フラグがfalseの場合")]
    public async void Applications_Case053()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        await Assert.ThrowsAsync<Exception>(async () => await applicationEngine.End());
        await Assert.ThrowsAsync<Exception>(async () => await applicationEngine.End(false));
    }

    [Fact(DisplayName = "Applications No054_engineCancellerがnullではない場合、かつ、強制終了フラグは指定しない場合")]
    public async void Applications_Case054()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController?.GetType();
        stateControllerType!.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Terminate);

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        var engineCancellerField = applicationEngine.GetType().GetField("engineCanceller", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        engineCancellerField!.SetValue(applicationEngine, cancellationTokenSource);

        bool ret = await applicationEngine.End(true);
        Assert.True(ret);

        var engineCanceller = (CancellationTokenSource?)engineCancellerField.GetValue(applicationEngine);
        Assert.Null(engineCanceller);
    }

    [Fact(DisplayName = "Applications No056_engineCancellerがnullではない場合、かつ、強制終了フラグにtrueを指定した場合")]
    public async void Applications_Case056()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Terminate);

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        var engineCancellerField = applicationEngine.GetType().GetField("engineCanceller", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        engineCancellerField!.SetValue(applicationEngine, cancellationTokenSource);

        bool ret = await applicationEngine.End(true);
        Assert.True(ret);

        var engineCanceller = (CancellationTokenSource?)engineCancellerField!.GetValue(applicationEngine);
        Assert.Null(engineCanceller);
    }
}