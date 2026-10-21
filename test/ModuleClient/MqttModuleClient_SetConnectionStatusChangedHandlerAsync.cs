using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class MqttModuleClient_SetConnectionStatusChangedHandlerAsync
{
    [Fact(DisplayName = "No501_正常系：正常動作の確認_statusChangeHandlerを指定して、メソッドを呼び出す")]
    public async Task Case501()
    {
        var client = NewMqttModuleClient();
        Task Handler(IotConnectionStatus status, IotConnectionStatusChangeReason reason) => Task.CompletedTask;

        var task = client.SetConnectionStatusChangedHandlerAsync(Handler);
        await task;

        Assert.True(task.IsCompletedSuccessfully);
    }

    private static MqttModuleClient NewMqttModuleClient()
    {
        return new MqttModuleClient("sas-token", "localhost", "device1");
    }
}
