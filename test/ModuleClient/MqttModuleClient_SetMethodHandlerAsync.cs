using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class MqttModuleClient_SetMethodHandlerAsync
{
    [Fact(DisplayName = "No497_異常系：異常動作の確認_methodName、methodHandler、userContextを指定して、メソッドを呼び出す")]
    public async Task Case497()
    {
        var client = NewMqttModuleClient();
        Task<MethodResponse> Handler(MethodRequest request, object context) => Task.FromResult(new MethodResponse(200));

        await Assert.ThrowsAsync<NotImplementedException>(
            () => client.SetMethodHandlerAsync("method", Handler, new object()));
    }

    [Fact(DisplayName = "No498_異常系：異常動作の確認_methodName、methodHandler、userContext、transportTopicを指定して、メソッドを呼び出す")]
    public async Task Case498()
    {
        var client = NewMqttModuleClient();
        Task<MethodResponse> Handler(MethodRequest request, object context) => Task.FromResult(new MethodResponse(200));

        await Assert.ThrowsAsync<NotImplementedException>(
            () => client.SetMethodHandlerAsync("method", Handler, new object(), TransportTopic.Iothub));
    }

    private static MqttModuleClient NewMqttModuleClient()
    {
        return new MqttModuleClient("sas-token", "localhost", "device1");
    }
}
