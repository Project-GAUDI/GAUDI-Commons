using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest.Stub;
using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest;

[Collection(nameof(MqttModuleClientTest))]
public class MqttModuleClient_SetInputMessageHandlerAsync : MqttModuleClientTesterBase
{
    private readonly MqttModuleClient _client;

    private readonly string _inputName = Guid.NewGuid().ToString();

    public MqttModuleClient_SetInputMessageHandlerAsync()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, _moduleId);

        _client = new MqttModuleClient(_sas, hostName: _gatewayHostName);
    }

    private List<string>? GetInputNames()
    {
        return GetFieldValue<List<string>>(_client, FI_INPUT_NAMES);
    }

    private Dictionary<string, CallbackProperties>? GetDictCallbackProps()
    {
        return GetFieldValue<Dictionary<string, CallbackProperties>>(_client, FI_DICT_CALLBACK_PROPS);
    }

    private void SetInputNames(List<string> inputs)
    {
        SetFieldValue(_client, FI_INPUT_NAMES, inputs);
    }

    private Task<MessageResponse> EmptyHandler(IotMessage message, object o)
    {
            return Task.FromResult(MessageResponse.None);
    }

    [Fact(DisplayName = "MqttModuleClient No032_IoTHubトピック向けのハンドラを登録する")]
    public async Task MqttModuleClient_Case032()
    {
        var inputNames = GetInputNames()!;
        var callbackProps = GetDictCallbackProps()!;
        Assert.Empty(inputNames);
        Assert.Empty(callbackProps);

        object context = new();
        await _client.SetInputMessageHandlerAsync(_inputName, EmptyHandler, context);

        Assert.Single(inputNames);
        Assert.Single(callbackProps);
        Assert.Equal(_inputName, inputNames[0]);
        Assert.NotNull(callbackProps[_inputName]);
        Assert.Equal(context, callbackProps[_inputName]!.messageUserContext);
    }

    [Fact(DisplayName = "MqttModuleClient No033_MQTTトピック向けにインプット名に\"#\"が含まれていないハンドラを登録する")]
    public async Task MqttModuleClient_Case033()
    {
        var inputNames = GetInputNames()!;
        var callbackProps = GetDictCallbackProps()!;
        Assert.Empty(inputNames);
        Assert.Empty(callbackProps);

        object context = new();
        MqttClientProxy.ClearSubscribeCallHistory();
        await _client.SetInputMessageHandlerAsync(_inputName, EmptyHandler, context, TransportTopic.Mqtt);

        Assert.Single(inputNames);
        Assert.Single(callbackProps);
        Assert.Equal(_inputName, inputNames[0]);
        Assert.NotNull(callbackProps[_inputName]);
        Assert.Equal(context, callbackProps[_inputName]!.messageUserContext);

        var history = MqttClientProxy.SubscribeMethodCalledHistoy;
        Assert.Single(history);
        Assert.Equal($"{_inputName}/#", history[0].Topics![0]);
    }

    [Fact(DisplayName = "MqttModuleClient No034_MQTTトピック向けにインプット名に\"#\"が含まれているハンドラを登録する")]
    public async Task MqttModuleClient_Case034()
    {
        var inputNames = GetInputNames()!;
        var callbackProps = GetDictCallbackProps()!;
        Assert.Empty(inputNames);
        Assert.Empty(callbackProps);

        var inputName = $"{_inputName}/#";
        object context = new();
        MqttClientProxy.ClearSubscribeCallHistory();
        await _client.SetInputMessageHandlerAsync(inputName, EmptyHandler, context, TransportTopic.Mqtt);

        Assert.Single(inputNames);
        Assert.Single(callbackProps);
        Assert.Equal(inputName, inputNames[0]);
        Assert.NotNull(callbackProps[inputName]);
        Assert.Equal(context, callbackProps[inputName]!.messageUserContext);

        var history = MqttClientProxy.SubscribeMethodCalledHistoy;
        Assert.Single(history);
        Assert.Equal(inputName, history[0].Topics![0]);
    }

    [Fact(DisplayName = "MqttModuleClient No035_トピック名の一覧にインプット名が存在する")]
    public async Task MqttModuleClient_Case035()
    {
        SetInputNames([_inputName]);

        var inputNames = GetInputNames()!;
        var callbackProps = GetDictCallbackProps()!;
        Assert.Single(inputNames);
        Assert.Empty(callbackProps);

        object context = new();
        MqttClientProxy.ClearSubscribeCallHistory();
        await _client.SetInputMessageHandlerAsync(_inputName, EmptyHandler, context, TransportTopic.Mqtt);

        Assert.Empty(callbackProps);

        var history = MqttClientProxy.SubscribeMethodCalledHistoy;
        Assert.Empty(history);
    }
}
