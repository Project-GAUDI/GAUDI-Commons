using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotHubModuleClient_DefaultConnectionStatusChangeHandler
{
    [Fact(DisplayName = "No439_正常系：正常動作の確認_status、reasonを指定して、メソッドを呼び出す")]
    public async Task Case439()
    {
        var client = NewIotHubModuleClient();

        var task = client.DefaultConnectionStatusChangeHandler(IotConnectionStatus.Connected, IotConnectionStatusChangeReason.Connection_Ok);
        await task;

        Assert.True(task.IsCompletedSuccessfully);
    }

    private static IotHubModuleClient NewIotHubModuleClient()
    {
        return (IotHubModuleClient)Activator.CreateInstance(
            typeof(IotHubModuleClient),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: Array.Empty<object>(),
            culture: null)!;
    }
}
