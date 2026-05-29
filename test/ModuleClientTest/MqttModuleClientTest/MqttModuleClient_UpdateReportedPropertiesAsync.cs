using Microsoft.Azure.Devices.Shared;
using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest;

[Collection(nameof(MqttModuleClientTest))]
public class MqttModuleClient_UpdateReportedPropertiesAsync : MqttModuleClientTesterBase
{
    private readonly MqttModuleClient _client;

    public MqttModuleClient_UpdateReportedPropertiesAsync()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, _moduleId);

        _client = new MqttModuleClient(_sas, hostName: _gatewayHostName);
    }

    [Fact(DisplayName = "MqttModuleClient No037_正常に処理が終了する")]
    public async Task MqttModuleClient_Case037()
    {
        var collection = new TwinCollection();
        await Assert.ThrowsAnyAsync<Exception>(() => _client.UpdateReportedPropertiesAsync(collection));
    }
}
