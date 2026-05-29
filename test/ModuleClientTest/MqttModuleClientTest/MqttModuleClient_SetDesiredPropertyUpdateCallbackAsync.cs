using Microsoft.Azure.Devices.Client;
using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest;

[Collection(nameof(MqttModuleClientTest))]
public class MqttModuleClient_SetDesiredPropertyUpdateCallbackAsync : MqttModuleClientTesterBase
{
    private readonly MqttModuleClient _client;

    public MqttModuleClient_SetDesiredPropertyUpdateCallbackAsync()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, _moduleId);

        _client = new MqttModuleClient(_sas, hostName: _gatewayHostName);
    }

    private DesiredPropertyUpdateCallback GetPatchCallback()
    {
        return GetFieldValue<DesiredPropertyUpdateCallback>(_client, FI_PATCH_CALLBACK)!;
    }

    private object GetPatchUserContext()
    {
        return GetFieldValue<object>(_client, "patchUserContext")!;
    }

    [Fact(DisplayName = "MqttModuleClient No031_正常に処理が終了する")]
    public async Task MqttModuleClient_Case031()
    {
        DesiredPropertyUpdateCallback callback = (_, _) => {
            // only definition
            return Task.CompletedTask;
        };
        object context = new();

        await _client.SetDesiredPropertyUpdateCallbackAsync(callback, context);
        Assert.Equal(callback, GetPatchCallback());
        Assert.Equal(context, GetPatchUserContext());
    }
}
