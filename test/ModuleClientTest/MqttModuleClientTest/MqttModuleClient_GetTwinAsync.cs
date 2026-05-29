using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest.Stub;
using uPLibrary.Networking.M2Mqtt.Messages;
using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest;

[Collection(nameof(MqttModuleClientTest))]
public class MqttModuleClient_GetTwinAsync : MqttModuleClientTesterBase
{
    private const int TEST_WAITING_TIMEOUT_SECONDS = 30;

    private const string PUBLISHED_TOPIC_PATTERN = @"^\$iothub/twin/GET/\?\$rid=([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})$";

    private const string TWIN_NOTIFY_TOPIC_PREFIX = "$iothub/twin/res";

    private readonly MqttModuleClient _client;

    public MqttModuleClient_GetTwinAsync()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        _client = new MqttModuleClient(_sas, _gatewayHostName);

        MqttClientPatch.PatchPublishMethod(_harmony);
    }

    private void InvokeMassgeReceiveEvent(string topic, string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        var eventArgs = new MqttMsgPublishEventArgs(topic, bytes, false, 0, false);

        var mi = typeof(MqttModuleClient).GetMethod("Client_MqttMsgPublishReceived", BindingFlags.Instance | BindingFlags.NonPublic);
        try
        {
            mi!.Invoke(_client, [ new object(), eventArgs ]);
        }
        catch (TargetInvocationException tie)
        {
            throw tie.InnerException!;
        }
    }

    [Fact(DisplayName = "MqttModuleClient No022_GetTwinAsyncが呼び出される")]
    public async Task MqttModuleClient_Case022()
    {
        var task = _client.GetTwinAsync();
        var begin = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        while (!MqttClientProxy.WasPublished)
        {
            await Task.Delay(100);

            if (begin - DateTimeOffset.UtcNow.ToUnixTimeSeconds() > TEST_WAITING_TIMEOUT_SECONDS)
                throw new TimeoutException();
        }

        var regex = new Regex(PUBLISHED_TOPIC_PATTERN);
        var match = regex.Match(MqttClientProxy.PublishedTopic);
        Assert.True(match.Success);
        Assert.Empty(MqttClientProxy.PublishedMessage);

        // タイムアウトを起こさないように後処理としてコールバック処理を実行する
        var rid = match.Groups[1].Value;
        var topic = @$"{TWIN_NOTIFY_TOPIC_PREFIX}/200/?$rid={rid}";
        InvokeMassgeReceiveEvent(topic, "");

        await task;
    }

    [Fact(DisplayName = "MqttModuleClient No023_受信したメッセージのtopicが正規表現に一致しない")]
    [Trait("Category", "Timeout")]
    public async Task MqttModuleClient_Case023()
    {
        var task = _client.GetTwinAsync();
        var begin = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        while (!MqttClientProxy.WasPublished)
        {
            await Task.Delay(100);

            if (begin - DateTimeOffset.UtcNow.ToUnixTimeSeconds() > TEST_WAITING_TIMEOUT_SECONDS)
                throw new TimeoutException();
        }

        var regex = new Regex(PUBLISHED_TOPIC_PATTERN);
        var match = regex.Match(MqttClientProxy.PublishedTopic);
        Assert.True(match.Success);
        Assert.Empty(MqttClientProxy.PublishedMessage);

        var rid = match.Groups[1].Value;
        var unmatchTopic = @$"{TWIN_NOTIFY_TOPIC_PREFIX}/not-match-pattern/?$rid={rid}";
        InvokeMassgeReceiveEvent(unmatchTopic, "");

        await Assert.ThrowsAnyAsync<Exception>(() => task);
    }

    [Fact(DisplayName = "MqttModuleClient No024_デバイスツインの受信が正常に完了する")]
    public async Task MqttModuleClient_Case024()
    {
        var desired = new JObject();
        for (var i = 0; i < 5; ++i)
        {
            desired.Add($"desired-key{i}", Guid.NewGuid().ToString());
        }
        var reported = new JObject();
        for (var i = 0; i < 5; ++i)
        {
            reported.Add($"reported-key{i}", Guid.NewGuid().ToString());
        }
        var properties = new TwinProperties()
        {
            Desired = new TwinCollection(desired.ToString()),
            Reported = new TwinCollection(reported.ToString())
        };
        var expected = JsonConvert.SerializeObject(properties);

        var task = _client.GetTwinAsync();
        var begin = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        while (!MqttClientProxy.WasPublished)
        {
            await Task.Delay(100);

            if (begin - DateTimeOffset.UtcNow.ToUnixTimeSeconds() > TEST_WAITING_TIMEOUT_SECONDS)
                throw new TimeoutException();
        }

        var regex = new Regex(PUBLISHED_TOPIC_PATTERN);
        var match = regex.Match(MqttClientProxy.PublishedTopic);
        var rid = match.Groups[1].Value;
        var topic = @$"{TWIN_NOTIFY_TOPIC_PREFIX}/200/?$rid={rid}";
        InvokeMassgeReceiveEvent(topic, expected);

        var response = await task;
        Assert.NotNull(response);
        Assert.IsType<Twin>(response);
        Assert.Equal(expected, JsonConvert.SerializeObject(response.Properties));
    }

    [Fact(DisplayName = "MqttModuleClient No025_期待するトピックから異常なステータスを受け取る")]
    [Trait("Category", "Timeout")]
    public async Task MqttModuleClient_Case025()
    {
        var task = _client.GetTwinAsync();
        var begin = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        while (!MqttClientProxy.WasPublished)
        {
            await Task.Delay(100);

            if (begin - DateTimeOffset.UtcNow.ToUnixTimeSeconds() > TEST_WAITING_TIMEOUT_SECONDS)
                throw new TimeoutException();
        }

        var regex = new Regex(PUBLISHED_TOPIC_PATTERN);
        var match = regex.Match(MqttClientProxy.PublishedTopic);
        var rid = match.Groups[1].Value;
        var failedStatusTopic = @$"{TWIN_NOTIFY_TOPIC_PREFIX}/400/?$rid={rid}";
        InvokeMassgeReceiveEvent(failedStatusTopic, "");

        await Assert.ThrowsAnyAsync<Exception>(() => task);
    }

    [Fact(DisplayName = "MqttModuleClient No026_メッセージ受信時、エラーが発生した場合")]
    public async Task MqttModuleClient_Case026()
    {
        var task = _client.GetTwinAsync();
        var begin = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        while (!MqttClientProxy.WasPublished)
        {
            await Task.Delay(100);

            if (begin - DateTimeOffset.UtcNow.ToUnixTimeSeconds() > TEST_WAITING_TIMEOUT_SECONDS)
                throw new TimeoutException();
        }

        var regex = new Regex(PUBLISHED_TOPIC_PATTERN);
        var match = regex.Match(MqttClientProxy.PublishedTopic);
        var rid = match.Groups[1].Value;
        var overflowStatusTopic = @$"{TWIN_NOTIFY_TOPIC_PREFIX}/{long.MaxValue}/?$rid={rid}";
        InvokeMassgeReceiveEvent(overflowStatusTopic, "");

        await Assert.ThrowsAnyAsync<Exception>(() => task);
    }

    [Fact(DisplayName = "MqttModuleClient No027_デバイスツイン取得用メッセージ送信後、60秒間、レスポンスがない場合")]
    [Trait("Category", "Timeout")]
    public async Task MqttModuleClient_Case027()
    {
        var task = _client.GetTwinAsync();
        var begin = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        while (!MqttClientProxy.WasPublished)
        {
            await Task.Delay(100);

            if (begin - DateTimeOffset.UtcNow.ToUnixTimeSeconds() > TEST_WAITING_TIMEOUT_SECONDS)
                throw new TimeoutException();
        }

        await Assert.ThrowsAnyAsync<Exception>(() => task);
    }
}
