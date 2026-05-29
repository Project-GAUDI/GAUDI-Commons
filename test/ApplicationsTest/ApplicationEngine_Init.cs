using System;
using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_Init))]
public class ApplicationEngine_Init
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_Init(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No036_applicationMainがnullの状態で呼び出し")]
    public async void Applications_Case036()
    {
        ApplicationEngine applicationEngine = new();
        await Assert.ThrowsAsync<Exception>(applicationEngine.Init);
    }

    [Fact(DisplayName = "Applications No037_ステートを初期化状態にできない場合")]
    public async void Applications_Case037()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        await Assert.ThrowsAsync<Exception>(applicationEngine.Init);
    }

    [Fact(DisplayName = "Applications No038_MyModuleClientを生成し、初期化する処理中に予期せぬエラーが発生した場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case038()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No039_edgeHubに接続", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case039()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No040_環境変数からログレベルが取得できない場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case040()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No041_ログレベルの設定時に予期せぬエラーが発生した場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case041()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No042_desiredプロパティの取得する際に予期せぬエラーが発生した場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case042()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No043_環境変数からログレベルが取得でき、予期せぬエラーが発生しなかった場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case043()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No044_環境変数からログレベルが取得でき、予期せぬエラーが発生しなかった場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case044()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No045_環境変数からログレベルが取得でき、予期せぬエラーが発生しなかった場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case045()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No046_環境変数からログレベルが取得でき、予期せぬエラーが発生しなかった場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case046()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No047_環境変数からログレベルが取得でき、予期せぬエラーが発生しなかった場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case047()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No048_環境変数からログレベルが取得でき、予期せぬエラーが発生しなかった場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case048()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No049_すべての処理が正常に終了した場合", Skip = "MyModuleClientを作成している処理のモック化ができないことにより、メソッドの戻り値操作ができないためスキップ")]
    public void Applications_Case049()
    {
        Assert.True(true);
    }
}