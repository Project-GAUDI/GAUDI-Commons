using System;
using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_Term))]
public class ApplicationEngine_Term
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_Term(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No057_ステートを開放処理状態にできない場合、かつ、強制終了フラグがfalseの場合")]
    public async void Applications_Case057()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        await Assert.ThrowsAsync<Exception>(async () => await applicationEngine.Term());
        await Assert.ThrowsAsync<Exception>(async () => await applicationEngine.Term(false));
    }

    [Fact(DisplayName = "Applications No058_ステートを開放処理状態にできた場合")]
    public async void Applications_Case058()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);
        try
        {
            await applicationEngine.Term();
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }
    }

    [Fact(DisplayName = "Applications No113_ステートを開放処理状態にできた場合、もしくは、強制終了フラグがtrueの場合")]
    public async void Applications_Case113()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(null);
        try
        {
            await applicationEngine.Term(true);
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }
    }

    [Fact(DisplayName = "Applications No059_applicationMainがnullではない場合")]
    public async void Applications_Case059()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        await applicationEngine.Term(true);
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Once);
    }

    [Fact(DisplayName = "Applications No060_MyModuleClientがnullではない場合")]
    public async void Applications_Case060()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        await applicationEngine.Term(false);
        Assert.Null(moduleClient);
    }

    [Fact(DisplayName = "Applications No062_すべての処理が正常に終了した場合")]
    public async void Applications_Case062()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        bool ret = await applicationEngine.Term();
        Assert.True(ret);
    }
}