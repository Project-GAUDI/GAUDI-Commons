using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest;

[Collection(nameof(MqttModuleClientTest))]
public class MqttModuleClient_Dispose : MqttModuleClientTesterBase
{
    private readonly MqttModuleClient _client;

    public MqttModuleClient_Dispose()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, _moduleId);

        _client = new MqttModuleClient(_sas, hostName: _gatewayHostName);
    }

    private void SetInputNames(List<string> inputs)
    {
        SetFieldValue(_client, FI_INPUT_NAMES, inputs);
    }

    private List<string>? GetInputNames()
    {
        return GetFieldValue<List<string>>(_client, FI_INPUT_NAMES);
    }

    [Fact(DisplayName = "MqttModuleClient No018_正常に処理が終了する")]
    public void MqttModuleClient_Case018()
    {
        var before = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid().ToString()).ToList();
        SetInputNames(before);

        _client.Dispose();
        var after = GetInputNames();
        Assert.NotNull(after);
        Assert.Empty(after);
    }
}
