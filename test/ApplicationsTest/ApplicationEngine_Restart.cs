using System;
using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_Restart))]
public class ApplicationEngine_Restart
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_Restart(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No063_applicationMainがnullの状態で呼び出し")]
    public async void Applications_Case063()
    {
        ApplicationEngine applicationEngine = new();
        await Assert.ThrowsAsync<Exception>(applicationEngine.Restart);
    }

    // 現段階ではIOT Edgeに接続できずRestartの中のInitで予期せぬエラーが発生するが、IOT Edgeにつながるようになったらエラーがでなくなる。
    [Fact(DisplayName = "Applications No066_Initの処理が正常終了でない場合")]
    public async void Applications_Case066()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        await Assert.ThrowsAsync<Exception>(applicationEngine.Restart);
    }

    [Fact(DisplayName = "Applications No068_Readyの処理が正常終了でない場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case068()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No069_すべての処理が正常に終了した場合", Skip = "MyModuleClientがnullであり、moduleClientFactoryのCreateAsyncがstaticで定義されておりMockでの対応ができなかっためスキップ")]
    public void Applications_Case069()
    {
        Assert.True(true);
    }
}