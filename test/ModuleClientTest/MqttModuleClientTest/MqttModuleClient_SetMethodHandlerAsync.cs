using Microsoft.Azure.Devices.Client;
using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest;

[Collection(nameof(MqttModuleClientTest))]
public class MqttModuleClient_SetMethodHandlerAsync : MqttModuleClientTesterBase
{
    private readonly MqttModuleClient _client;

    public MqttModuleClient_SetMethodHandlerAsync()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, _moduleId);

        _client = new MqttModuleClient(_sas, hostName: _gatewayHostName);
    }

    [Fact(DisplayName = "MqttModuleClient No036_正常に処理が終了する")]
    public async Task MqttModuleClient_Case036()
    {
        var methodName = Guid.NewGuid().ToString();
        MethodCallback handler = (_, _) => {
            // only definition
            return Task.FromResult(new MethodResponse(200));
        };
        object context = new();

        await Assert.ThrowsAnyAsync<Exception>(() => _client.SetMethodHandlerAsync(methodName, handler, context));
    }
}
