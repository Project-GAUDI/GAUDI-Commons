using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_ReceiveTwinAsync))]
public class ApplicationEngine_ReceiveTwinAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_ReceiveTwinAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No088_Restartを呼び出したときに予期せぬエラーが発生した場合", Skip = "MyModuleClientがnullであるため、ReceiveTwinAsync内のRestartができずテストできないためスキップ")]
    public void Applications_Case088()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No089_Restartを呼び出した際に結果が失敗で返ってきた場合", Skip = "MyModuleClientがnullであるため、ReceiveTwinAsync内のRestartができずテストできないためスキップ")]
    public void Applications_Case089()
    {
        Assert.True(true);
    }
}