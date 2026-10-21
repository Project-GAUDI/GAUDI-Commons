using System.Reflection;
using System.Text;
using HarmonyLib;
using TICO.GAUDI.Commons;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(MqttModuleClientHarmonyCollection))]
public sealed class MqttModuleClient_GetTwinAsync
{
    [Fact(DisplayName = "No459_正常系：正常動作の確認_発行時と同じrid、status300未満が返る状態で、メソッドを呼び出す")]
    public async Task Case459()
    {
        var client = NewMqttModuleClient();
        using var patch = new PublishPatch();
        patch.OnPublish = (topic, _) =>
        {
            var rid = ExtractRid(topic);
            RaiseTwinResponse(client, 200, rid, "{\"desired\":{\"key1\":\"value1\"},\"reported\":{\"key2\":\"value2\"}}");
        };

        var twin = await client.GetTwinAsync();

        Assert.NotNull(twin.Properties);
        Assert.Equal("value1", twin.Properties.Desired["key1"].ToString());
        Assert.Equal("value2", twin.Properties.Reported["key2"].ToString());
        var call = Assert.Single(patch.Calls);
        Assert.Empty(call.Payload);
        Assert.StartsWith("$iothub/twin/GET/?$rid=", call.Topic);
    }

    [Fact(DisplayName = "No460_異常系：異常動作の確認_発行時と異なるrid、status300未満が返る状態で、メソッドを呼び出す")]
    public async Task Case460()
    {
        var client = NewMqttModuleClient();
        using var patch = new PublishPatch();
        patch.OnPublish = (topic, _) =>
        {
            RaiseTwinResponse(client, 200, Guid.NewGuid().ToString(), "{\"desired\":{},\"reported\":{}}");
        };

        await Assert.ThrowsAsync<TimeoutException>(() => client.GetTwinAsync());
    }

    [Fact(DisplayName = "No461_異常系：異常動作の確認_発行時と同じrid、status300以上が返る状態で、メソッドを呼び出す")]
    public async Task Case461()
    {
        var client = NewMqttModuleClient();
        using var patch = new PublishPatch();
        patch.OnPublish = (topic, _) =>
        {
            var rid = ExtractRid(topic);
            RaiseTwinResponse(client, 300, rid, "{\"desired\":{},\"reported\":{}}");
        };

        await Assert.ThrowsAsync<TimeoutException>(() => client.GetTwinAsync());
    }

    [Fact(DisplayName = "No462_正常系：正常動作の確認_同じrid、status=200で、正しいTwinJSONを受信する_DesiredとReportedの両方を設定する")]
    public async Task Case462()
    {
        var client = NewMqttModuleClient();
        using var patch = new PublishPatch();
        patch.OnPublish = (topic, _) =>
        {
            var rid = ExtractRid(topic);
            RaiseTwinResponse(client, 200, rid, "{\"desired\":{\"key1\":\"value1\"},\"reported\":{\"key2\":\"value2\"}}");
        };

        var twin = await client.GetTwinAsync();

        Assert.Equal("value1", twin.Properties.Desired["key1"].ToString());
        Assert.Equal("value2", twin.Properties.Reported["key2"].ToString());
    }

    [Fact(DisplayName = "No463_異常系：異常動作の確認_同じrid、status=200で、不正なTwinJSONを受信する")]
    public async Task Case463()
    {
        var client = NewMqttModuleClient();
        using var patch = new PublishPatch();
        patch.OnPublish = (topic, _) =>
        {
            var rid = ExtractRid(topic);
            RaiseTwinResponse(client, 200, rid, "{ invalid json");
        };

        await Assert.ThrowsAnyAsync<Exception>(() => client.GetTwinAsync());
    }

    [Fact(DisplayName = "No464_正常系：正常動作の確認_同じrid、status=200で、空文字列のレスポンスを受信する")]
    public async Task Case464()
    {
        var client = NewMqttModuleClient();
        using var patch = new PublishPatch();
        patch.OnPublish = (topic, _) =>
        {
            var rid = ExtractRid(topic);
            RaiseTwinResponse(client, 200, rid, "");
        };

        var twin = await client.GetTwinAsync();

        Assert.NotNull(twin);
        Assert.Null(twin.Properties);
    }

    [Fact(DisplayName = "No465_異常系：異常動作の確認_TwinGETメッセージ発行後、応答を受信しない")]
    public async Task Case465()
    {
        var client = NewMqttModuleClient();
        using var patch = new PublishPatch();

        await Assert.ThrowsAsync<TimeoutException>(() => client.GetTwinAsync());
    }

    private static MqttModuleClient NewMqttModuleClient()
    {
        var client = new MqttModuleClient("sas-token", "localhost", "device1");
        // タイムアウト待ちのテストを高速化するため、既定の60秒から短縮する
        typeof(MqttModuleClient).GetField("twinTimeout", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(client, TimeSpan.FromMilliseconds(300));
        return client;
    }

    private static string ExtractRid(string topic)
    {
        return topic.Substring(topic.IndexOf("$rid=") + "$rid=".Length);
    }

    private static void RaiseTwinResponse(MqttModuleClient client, int status, string rid, string message)
    {
        var topic = $"$iothub/twin/res/{status}/?$rid={rid}";
        var method = typeof(MqttModuleClient).GetMethod("Client_MqttMsgPublishReceived", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var args = Activator.CreateInstance(typeof(MqttMsgPublishEventArgs), topic, Encoding.UTF8.GetBytes(message), false, (byte)1, false);
        method.Invoke(client, new object?[] { null, args });
    }

    private sealed class PublishPatch : IDisposable
    {
        private readonly Harmony harmony;

        public Action<string, byte[]>? OnPublish { get; set; }

        public List<PublishCall> Calls { get; } = new();

        private static PublishPatch? current;

        public PublishPatch()
        {
            current = this;

            harmony = new Harmony($"{nameof(MqttModuleClient_GetTwinAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(MqttClient), nameof(MqttClient.Publish), new[] { typeof(string), typeof(byte[]) }),
                prefix: new HarmonyMethod(typeof(PublishPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
            current = null;
        }

        private static bool Prefix(string __0, byte[] __1, ref ushort __result)
        {
            current?.Calls.Add(new PublishCall(__0, __1));
            current?.OnPublish?.Invoke(__0, __1);
            __result = 0;
            return false;
        }
    }

    private sealed record PublishCall(string Topic, byte[] Payload);
}
