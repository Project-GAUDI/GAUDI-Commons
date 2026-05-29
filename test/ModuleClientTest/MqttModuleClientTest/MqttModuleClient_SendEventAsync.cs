using System.Text;
using TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest.Stub;
using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest;

[Collection(nameof(MqttModuleClientTest))]
public class MqttModuleClient_SendEventAsync : MqttModuleClientTesterBase
{
    private readonly string _outputName = Guid.NewGuid().ToString();

    private readonly byte[] _messageContent = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());

    private readonly Dictionary<string, string> _properties = new()
    {
        { "key1", "value1" },
        { "key2", "value2" }
    };

    private readonly IotMessage _message;

    public MqttModuleClient_SendEventAsync()
    {
        _message = new (_messageContent);
        _message.SetProperties(_properties);

        MqttClientPatch.PatchPublishMethod(_harmony);
    }

    [Fact(DisplayName = "MqttModuleClient No028_モジュールIDが指定されている")]
    public async Task MqttModuleClient_Case028()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, _moduleId);

        var client = new MqttModuleClient(_sas, _gatewayHostName);
        await client.SendEventAsync(_outputName, _message, TransportTopic.Iothub);

        var parameters = new Dictionary<string, string>();
        foreach (var prop in _properties)
        {
            parameters.Add(prop.Key, prop.Value);
        }
        parameters.Add("$.on", _outputName);
        var topic = $"devices/{_deviceId}/modules/{_moduleId}/messages/events/{string.Join("&", parameters.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"))}";
        Assert.Equal(MqttClientProxy.PublishedTopic, topic);
        Assert.Equal(MqttClientProxy.PublishedMessage, _messageContent);
    }

    [Fact(DisplayName = "MqttModuleClient No029_モジュールIDが指定されていない")]
    public async Task MqttModuleClient_Case029()
    {
        var client = new MqttModuleClient(_sas, _gatewayHostName, _deviceId);
        await client.SendEventAsync(_outputName, _message, TransportTopic.Iothub);

        var parameters = new Dictionary<string, string>();
        foreach (var prop in _properties)
        {
            parameters.Add(prop.Key, prop.Value);
        }
        parameters.Add("$.on", _outputName);
        var topic = $"devices/{_deviceId}/messages/events/{string.Join("&", parameters.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"))}";
        Assert.Equal(MqttClientProxy.PublishedTopic, topic);
        Assert.Equal(MqttClientProxy.PublishedMessage, _messageContent);
    }

    [Fact(DisplayName = "MqttModuleClient No030_Mqttトピック")]
    public async Task MqttModuleClient_Case030()
    {
        var client = new MqttModuleClient(_sas, _gatewayHostName, _deviceId);
        await client.SendEventAsync(_outputName, _message, TransportTopic.Mqtt);

        var topic = $"{_outputName}/{string.Join("&", _properties.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"))}";
        Assert.Equal(MqttClientProxy.PublishedTopic, topic);
        Assert.Equal(MqttClientProxy.PublishedMessage, _messageContent);
    }
}
