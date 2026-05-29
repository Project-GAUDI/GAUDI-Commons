using TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest.Stub;
using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest;

[Collection(nameof(MqttModuleClientTest))]
public class MqttModuleClient_OpenAsync : MqttModuleClientTesterBase
{
    [Fact(DisplayName = "MqttModuleClient No019_MqttModuleClientインスタンス作成時にモジュールIDが設定されていない")]
    public async Task MqttModuleClient_Case019()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_IOTHUBHOSTNAME, _iothubHostName);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);

        var client = new MqttModuleClient(_sas, _gatewayHostName);

        MqttClientPatch.PatchConnectMethod(_harmony);
        await client.OpenAsync();

        var username = $"{_iothubHostName}/{_deviceId}/?api-version=2018-06-30";
        MqttClientProxy.ReviewConnectArguments(_deviceId, username, _sas);
    }

    [Fact(DisplayName = "MqttModuleClient No020_MqttModuleClientインスタンス作成時にモジュールIDが設定されている")]
    public async Task MqttModuleClient_Case020()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_IOTHUBHOSTNAME, _iothubHostName);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, _moduleId);

        var client = new MqttModuleClient(_sas, _gatewayHostName);

        MqttClientPatch.PatchConnectMethod(_harmony);
        await client.OpenAsync();

        var clientId = $"{_deviceId}/{_moduleId}";
        var username = $"{_iothubHostName}/{_deviceId}/{_moduleId}/?api-version=2018-06-30";
        MqttClientProxy.ReviewConnectArguments(clientId, username, _sas);
    }

    [Fact(DisplayName = "MqttModuleClient No021_MqttClientの接続に失敗する")]
    public async Task MqttModuleClient_Case021()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_IOTHUBHOSTNAME, _iothubHostName);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, _moduleId);

        var client = new MqttModuleClient(_sas, _gatewayHostName);

        MqttClientPatch.PatchConnectMethod(_harmony, false);

        await Assert.ThrowsAnyAsync<Exception>(client.OpenAsync);
    }
}
