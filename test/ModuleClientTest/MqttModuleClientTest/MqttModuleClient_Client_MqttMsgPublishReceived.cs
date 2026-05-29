using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Frameworks;
using System.Reflection;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;
using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest;

[Collection(nameof(MqttModuleClientTest))]
public class MqttModuleClient_Client_MqttMsgPublishReceived : MqttModuleClientTesterBase
{
    private const string TWIN_PATCH_PREFIX = "$iothub/twin/PATCH/properties/desired";

    private readonly MqttModuleClient _client;

    public MqttModuleClient_Client_MqttMsgPublishReceived()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, _moduleId);

        _client = new MqttModuleClient(_sas, hostName: _gatewayHostName);
    }

    private string IotHubTopicPrefix
    {
        get => $"devices/{_deviceId}/modules/{_moduleId}";
    }

    private void SetPatchCallback(DesiredPropertyUpdateCallback callback)
    {
        SetFieldValue(_client, FI_PATCH_CALLBACK, callback);
    }

    private void SetInputNames(List<string> inputs)
    {
        SetFieldValue(_client, FI_INPUT_NAMES, inputs);
    }

    private void SetDictCallbackProps(Dictionary<string, CallbackProperties> map)
    {
        SetFieldValue(_client, FI_DICT_CALLBACK_PROPS, map);
    }

    private void InvokeEvent(string topic, string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        var eventArgs = new MqttMsgPublishEventArgs(topic, bytes, false, 0, false);

        var mi = typeof(MqttModuleClient).GetMethod("Client_MqttMsgPublishReceived", BindingFlags.Instance | BindingFlags.NonPublic);
        try
        {
            mi!.Invoke(_client, [new object(), eventArgs]);
        }
        catch (TargetInvocationException tie)
        {
            throw tie.InnerException!;
        }
    }

    [Fact(DisplayName = "MqttModuleClient No007_プロパティ更新コールバックが存在する")]
    public void MqttModuleClient_Case007()
    {
        var desired = new JObject
        {
            { "prop1", Guid.NewGuid().ToString() }
        };
        var message = new TwinProperties
        {
            Desired = new TwinCollection(desired.ToString())
        };

        var executedCallback = false;
        DesiredPropertyUpdateCallback callback = (desiredProperties, userContext) =>
        {
            executedCallback = true;

            var expected = JsonConvert.SerializeObject(message.Desired);
            var actual = JsonConvert.SerializeObject(desiredProperties);
            Assert.Equal(expected, actual);
            return Task.CompletedTask;
        };
        SetPatchCallback(callback);

        var topic = $"{TWIN_PATCH_PREFIX}/prop1";
        InvokeEvent(topic, JsonConvert.SerializeObject(message));

        Assert.True(executedCallback);
    }

    [Fact(DisplayName = "MqttModuleClient No008_プロパティ更新コールバックが存在しない")]
    public void MqttModuleClient_Case008()
    {
        var topic = $"{TWIN_PATCH_PREFIX}/prop1";
        InvokeEvent(topic, "");
    }

    [Fact(DisplayName = "MqttModuleClient No010_トピック名に対応するコールバックが存在しない")]
    public void MqttModuleClient_Case010()
    {
        const string inputName = "input1";

        SetInputNames([inputName]);

        var topic = $"{IotHubTopicPrefix}/inputs/{inputName}/param1=";
        InvokeEvent(topic, "");
    }

    [Fact(DisplayName = "MqttModuleClient No011_システムプロパティを含むIoTHubトピックを受信する")]
    public void MqttModuleClient_Case011()
    {
        const string inputName = "input1";

        Dictionary<string, string> messageMap = [];
        for (var i = 0; i < 5; ++i)
        {
            messageMap.Add($"key-{i}", Guid.NewGuid().ToString());
        }
        var messageBody = JsonConvert.SerializeObject(messageMap, Formatting.None);

        Dictionary<string, string> parameters = [];
        for (var i = 0; i < 5; ++i)
        {
            parameters.Add($"prop-{i}", Guid.NewGuid().ToString());
        }
        for (var i = 0; i < 5; ++i)
        {
            parameters.Add($"$sysProp-{i}", Guid.NewGuid().ToString());
        }

        var map = new Dictionary<string, CallbackProperties>();
        var executedCallback = false;
        var context = new object();
        map.Add(inputName, new CallbackProperties()
        {
            messageHandler = (message, userContext) =>
            {
                executedCallback = true;

                var actualBody = Encoding.UTF8.GetString(message.GetBytes());
                Assert.Equal(messageBody, actualBody);

                var expectedProperties = parameters.Where(kv => !kv.Key.StartsWith('$')).ToDictionary(kv => kv.Key, kv => kv.Value);
                Assert.Equal(expectedProperties.Count, message.Properties.Count);
                foreach (var expectedProp in expectedProperties)
                {
                    Assert.Equal(expectedProp.Value, message.Properties[expectedProp.Key]);
                }

                Assert.Equal(context.GetHashCode(), userContext.GetHashCode());
                return Task.FromResult(MessageResponse.Completed);
            },
            messageUserContext = context
        });

        SetInputNames([inputName]);
        SetDictCallbackProps(map);

        var paramPart = string.Join('&', parameters.Select(kv => $"{kv.Key}={kv.Value}"));
        var topic = $"{IotHubTopicPrefix}/inputs/{inputName}/{paramPart}";
        InvokeEvent(topic, messageBody);

        Assert.True(executedCallback);
    }

    [Fact(DisplayName = "MqttModuleClient No012_非KetValue形式のプロパティを含むIoTHubトピックを受信する")]
    public void MqttModuleClient_Case012()
    {
        const string inputName = "input1";

        Dictionary<string, string> messageMap = [];
        for (var i = 0; i < 5; ++i)
        {
            messageMap.Add($"key-{i}", Guid.NewGuid().ToString());
        }
        var messageBody = JsonConvert.SerializeObject(messageMap, Formatting.None);

        Dictionary<string, string> validParameters = [];
        for (var i = 0; i < 5; ++i)
        {
            validParameters.Add($"prop-{i}", Guid.NewGuid().ToString());
        }

        var map = new Dictionary<string, CallbackProperties>();
        var executedCallback = false;
        var context = new object();
        map.Add(inputName, new CallbackProperties()
        {
            messageHandler = (message, userContext) =>
            {
                executedCallback = true;

                var actualBody = Encoding.UTF8.GetString(message.GetBytes());
                Assert.Equal(messageBody, actualBody);

                Assert.Equal(validParameters.Count, message.Properties.Count);
                foreach (var expectedProp in validParameters)
                {
                    Assert.Equal(expectedProp.Value, message.Properties[expectedProp.Key]);
                }

                Assert.Equal(context.GetHashCode(), userContext.GetHashCode());
                return Task.FromResult(MessageResponse.Completed);
            },
            messageUserContext = context
        });

        SetInputNames([inputName]);
        SetDictCallbackProps(map);

        var chaosParameters = validParameters.Select(kv => $"{kv.Key}={kv.Value}").Union(["single", "multi=1=2"]);
        var paramPart = string.Join('&', chaosParameters);
        var topic = $"{IotHubTopicPrefix}/inputs/{inputName}/{paramPart}";
        InvokeEvent(topic, messageBody);

        Assert.True(executedCallback);
    }

    [Fact(DisplayName = "MqttModuleClient No013_プロパティが指定されていないIoTHubトピックを受信する")]
    public void MqttModuleClient_Case013()
    {
        const string inputName = "input1";

        Dictionary<string, string> messageMap = [];
        for (var i = 0; i < 5; ++i)
        {
            messageMap.Add($"key-{i}", Guid.NewGuid().ToString());
        }
        var messageBody = JsonConvert.SerializeObject(messageMap, Formatting.None);

        var map = new Dictionary<string, CallbackProperties>();
        var executedCallback = false;
        var context = new object();
        map.Add(inputName, new CallbackProperties()
        {
            messageHandler = (message, userContext) =>
            {
                executedCallback = true;

                var actualBody = Encoding.UTF8.GetString(message.GetBytes());
                Assert.Equal(messageBody, actualBody);

                Assert.Empty(message.Properties);
                Assert.Equal(context.GetHashCode(), userContext.GetHashCode());
                return Task.FromResult(MessageResponse.Completed);
            },
            messageUserContext = context
        });

        SetInputNames([inputName]);
        SetDictCallbackProps(map);

        var topic = $"{IotHubTopicPrefix}/inputs/{inputName}/";
        InvokeEvent(topic, messageBody);

        Assert.True(executedCallback);
    }

    [Fact(DisplayName = "MqttModuleClient No014_トピック名に対応するコールバックが存在しない")]
    public void MqttModuleClient_Case014()
    {
        const string mqttTopicName = "topic1";

        SetInputNames([mqttTopicName]);

        var topic = $"{mqttTopicName}/param1=";
        InvokeEvent(topic, "");
    }

    [Fact(DisplayName = "MqttModuleClient No015_トピック名に合致するコールバックが存在する")]
    public void MqttModuleClient_Case015()
    {
        const string mqttTopicName = "topic1";

        Dictionary<string, string> messageMap = [];
        for (var i = 0; i < 5; ++i)
        {
            messageMap.Add($"key-{i}", Guid.NewGuid().ToString());
        }
        var messageBody = JsonConvert.SerializeObject(messageMap, Formatting.None);

        Dictionary<string, string> parameters = [];
        for (var i = 0; i < 5; ++i)
        {
            parameters.Add($"prop-{i}", Guid.NewGuid().ToString());
        }

        var map = new Dictionary<string, CallbackProperties>();
        var executedCallback = false;
        var context = new object();
        map.Add(mqttTopicName, new CallbackProperties()
        {
            messageHandler = (message, userContext) =>
            {
                executedCallback = true;

                var actualBody = Encoding.UTF8.GetString(message.GetBytes());
                Assert.Equal(messageBody, actualBody);

                Assert.Equal(parameters.Count, message.Properties.Count);
                foreach (var expectedProp in parameters)
                {
                    Assert.Equal(expectedProp.Value, message.Properties[expectedProp.Key]);
                }

                Assert.Equal(context.GetHashCode(), userContext.GetHashCode());
                return Task.FromResult(MessageResponse.Completed);
            },
            messageUserContext = context
        });

        SetInputNames([mqttTopicName]);
        SetDictCallbackProps(map);

        var paramPart = string.Join('&', parameters.Select(kv => $"{kv.Key}={kv.Value}"));
        var topic = $"{mqttTopicName}/{paramPart}";
        InvokeEvent(topic, messageBody);

        Assert.True(executedCallback);
    }

    [Fact(DisplayName = "MqttModuleClient No016_MQTTトピックに対する汎用コールバックが存在する")]
    public void MqttModuleClient_Case016()
    {
        const string genericKeyword = "#";
        const string mqttTopicName = "topic1";

        Dictionary<string, string> messageMap = [];
        for (var i = 0; i < 5; ++i)
        {
            messageMap.Add($"key-{i}", Guid.NewGuid().ToString());
        }
        var messageBody = JsonConvert.SerializeObject(messageMap, Formatting.None);

        Dictionary<string, string> parameters = [];
        for (var i = 0; i < 5; ++i)
        {
            parameters.Add($"prop-{i}", Guid.NewGuid().ToString());
        }

        var map = new Dictionary<string, CallbackProperties>();
        var executedCallback = false;
        var context = new object();
        map.Add(genericKeyword, new CallbackProperties()
        {
            messageHandler = (message, userContext) =>
            {
                executedCallback = true;

                var actualBody = Encoding.UTF8.GetString(message.GetBytes());
                Assert.Equal(messageBody, actualBody);

                Assert.Equal(parameters.Count, message.Properties.Count);
                foreach (var expectedProp in parameters)
                {
                    Assert.Equal(expectedProp.Value, message.Properties[expectedProp.Key]);
                }

                Assert.Equal(context.GetHashCode(), userContext.GetHashCode());
                return Task.FromResult(MessageResponse.Completed);
            },
            messageUserContext = context
        });

        SetInputNames([genericKeyword]);
        SetDictCallbackProps(map);

        var paramPart = string.Join('&', parameters.Select(kv => $"{kv.Key}={kv.Value}"));
        var topic = $"{mqttTopicName}/{paramPart}";
        InvokeEvent(topic, messageBody);

        Assert.True(executedCallback);
    }

    [Fact(DisplayName = "MqttModuleClient No017_処理対象外のIoTHub予約トピックを受信")]
    public void MqttModuleClient_Case017()
    {
        const string genericKeyword = "#";
        const string reservedTopic = "$iothub/reserved";

        var map = new Dictionary<string, CallbackProperties>();
        var executedCallback = false;
        var callbackProps = new CallbackProperties()
        {
            messageHandler = (message, userContext) => {
                executedCallback = true;
                return Task.FromResult(MessageResponse.None);
            },
            messageUserContext = new object()
        };
        map.Add(genericKeyword, callbackProps);
        map.Add(reservedTopic, callbackProps);

        SetInputNames([genericKeyword, reservedTopic]);
        SetDictCallbackProps(map);

        InvokeEvent(reservedTopic, "");

        // 汎用コールバック、送信したトピック名に対するコールバックが実行されないことで、コールバックが実行されないこととする
        Assert.False(executedCallback);
    }
}
