using Xunit;
using Xunit.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_GetCancelWaitTask))]
public class ApplicationEngine_GetCancelWaitTask
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_GetCancelWaitTask(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No105_cancellationTokenがキャンセルされた場合")]
    public async void Applications_Case105()
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        //3秒後にキャンセルしてGetCancelWaitTaskTestの待機状態を抜ければテストOKとする。
        _ = Task.Run(() =>
        {
            Thread.Sleep(3000);
            cancellationTokenSource.Cancel();
        });

        ApplicationEngine applicationEngine = new();
        var info = applicationEngine.GetType().GetMethod("GetCancelWaitTask", BindingFlags.NonPublic | BindingFlags.Static);
        try
        {
            await (Task)info!.Invoke(null, [cancellationTokenSource.Token])!;
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }
    }
}