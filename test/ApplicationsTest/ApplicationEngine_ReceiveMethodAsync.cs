using Xunit;
using Xunit.Abstractions;
using Microsoft.Azure.Devices.Client;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_ReceiveMethodAsync))]
public class ApplicationEngine_ReceiveMethodAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_ReceiveMethodAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No090_ReceiveMethodAsyncを呼び出し")]
    public async void Applications_Case090()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        MethodResponse methodResponse = await applicationEngine.ReceiveMethodAsync(null, "methodName");
        Assert.Equal(-1, methodResponse.Status);
    }

    [Fact(DisplayName = "Applications No092_Dequeueを呼び出した後の処理", Skip = "MyModuleClientがnullであるため、messageInputEventDataが取得できずテストできないためスキップ")]
    public void Applications_Case092()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No094_eventDataがnullの場合", Skip = "MyModuleClientがnullであるため、messageInputEventDataが取得できずテストできないためスキップ")]
    public void Applications_Case094()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No095_userContextに基づいてメソッドハンドラデータを取得する際に予期せぬエラーが発生した場合")]
    public async void Applications_Case095()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        MethodResponse methodResponse = await applicationEngine.ReceiveMethodAsync(null, "methodName");
        Assert.Equal(-1, methodResponse.Status);
    }

    [Fact(DisplayName = "Applications No096_ステータスを待機状態に変更できない場合", Skip = "MyModuleClientがnullであるため、messageInputEventDataが取得できずテストできないためスキップ")]
    public void Applications_Case096()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No098_すべての処理が正常に終了した場合", Skip = "MyModuleClientがnullであるため、messageInputEventDataが取得できずテストできないためスキップ")]
    public void Applications_Case098()
    {
        Assert.True(true);
    }
}