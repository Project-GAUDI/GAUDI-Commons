using Xunit;
using Xunit.Abstractions;
using System;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_RunAsync))]
public class ApplicationEngine_RunAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_RunAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No007_applicationMainがnullの状態で呼び出し")]
    public async void Applications_Case007()
    {
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(null);
        await Assert.ThrowsAsync<Exception>(applicationEngine.RunAsync);
    }

    [Fact(DisplayName = "Applications No008_エンジンの初期化で予期せぬエラーが発生", Skip = "アプリケーションループに入ったかが検知できないためスキップ")]
    public void Applications_Case008()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No009_エンジンを待機ステートへ遷移で予期せぬエラーが発生", Skip = "アプリケーションループに入ったかが検知できないためスキップ")]
    public void Applications_Case009()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No010_エンジンを待機ステートへ遷移で予期せぬエラーが発生", Skip = "アプリケーションループに入ったかが検知できないためスキップ")]
    public void Applications_Case010()
    {
        Assert.True(true);
    }
}
