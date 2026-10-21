using HarmonyLib;
using TICO.GAUDI.Commons;
using uPLibrary.Networking.M2Mqtt;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(MqttModuleClientHarmonyCollection))]
public sealed class MqttModuleClient_SendEventAsync
{
    [Fact(DisplayName = "No466_正常系：正常動作の確認_defaultSendTopic=Iothubの状態で、outputName、messageを指定して、メソッドを呼び出す")]
    public async Task Case466()
    {
        var client = NewMqttModuleClientNoModule(TransportTopic.Iothub);
        using var patch = new SendEventAsyncPatch();
        var message = new IotMessage();

        await client.SendEventAsync("output", message);

        var call = Assert.Single(SendEventAsyncPatch.Calls);
        Assert.Equal("output", call.OutputName);
        Assert.Same(message, call.Message);
        Assert.Equal(TransportTopic.Iothub, call.TransportTopic);
    }

    [Fact(DisplayName = "No467_正常系：正常動作の確認_defaultSendTopic=Mqttの状態で、outputName、messageを指定して、メソッドを呼び出す")]
    public async Task Case467()
    {
        var client = NewMqttModuleClientNoModule(TransportTopic.Mqtt);
        using var patch = new SendEventAsyncPatch();
        var message = new IotMessage();

        await client.SendEventAsync("output", message);

        var call = Assert.Single(SendEventAsyncPatch.Calls);
        Assert.Equal("output", call.OutputName);
        Assert.Same(message, call.Message);
        Assert.Equal(TransportTopic.Mqtt, call.TransportTopic);
    }

    [Fact(DisplayName = "No468_正常系：正常動作の確認_メッセージプロパティあり、モジュールIDありの状態で、outputName、message、TransportTopic.Iothubを指定して、メソッドを呼び出す")]
    public async Task Case468()
    {
        var client = NewMqttModuleClientWithModule();
        using var patch = new PublishPatch();
        var message = new IotMessage();
        message.SetProperty("key1", "value1");

        await client.SendEventAsync("output", message, TransportTopic.Iothub);

        var call = Assert.Single(patch.Calls);
        Assert.Equal("devices/dev1/modules/mod1/messages/events/key1=value1&%24.on=output", call.Topic);
        Assert.Same(message.GetBytes(), call.Payload);
    }

    [Fact(DisplayName = "No469_正常系：正常動作の確認_メッセージプロパティあり、モジュールIDなしの状態で、outputName、message、TransportTopic.Iothubを指定して、メソッドを呼び出す")]
    public async Task Case469()
    {
        var client = NewMqttModuleClientNoModule();
        using var patch = new PublishPatch();
        var message = new IotMessage();
        message.SetProperty("key1", "value1");

        await client.SendEventAsync("output", message, TransportTopic.Iothub);

        var call = Assert.Single(patch.Calls);
        Assert.Equal("devices/dev1/messages/events/key1=value1&%24.on=output", call.Topic);
    }

    [Fact(DisplayName = "No470_正常系：正常動作の確認_メッセージプロパティなしの状態で、outputName、message、TransportTopic.Iothubを指定して、メソッドを呼び出す")]
    public async Task Case470()
    {
        var client = NewMqttModuleClientNoModule();
        using var patch = new PublishPatch();
        var message = new IotMessage();

        await client.SendEventAsync("output", message, TransportTopic.Iothub);

        var call = Assert.Single(patch.Calls);
        Assert.Equal("devices/dev1/messages/events/%24.on=output", call.Topic);
    }

    [Fact(DisplayName = "No471_正常系：正常動作の確認_メッセージプロパティありの状態で、outputName、message、TransportTopic.Mqttを指定して、メソッドを呼び出す")]
    public async Task Case471()
    {
        var client = NewMqttModuleClientNoModule();
        using var patch = new PublishPatch();
        var message = new IotMessage();
        message.SetProperty("key1", "value1");

        await client.SendEventAsync("output", message, TransportTopic.Mqtt);

        var call = Assert.Single(patch.Calls);
        Assert.Equal("output/key1=value1", call.Topic);
    }

    [Fact(DisplayName = "No472_正常系：正常動作の確認_メッセージプロパティなしの状態で、outputName、message、TransportTopic.Mqttを指定して、メソッドを呼び出す")]
    public async Task Case472()
    {
        var client = NewMqttModuleClientNoModule();
        using var patch = new PublishPatch();
        var message = new IotMessage();

        await client.SendEventAsync("output", message, TransportTopic.Mqtt);

        var call = Assert.Single(patch.Calls);
        Assert.Equal("output/", call.Topic);
    }

    [Fact(DisplayName = "No473_正常系：正常動作の確認_outputNameにスペース、&、=、日本語などURLエンコードが必要な文字を指定して、メソッドを呼び出す")]
    public async Task Case473()
    {
        var client = NewMqttModuleClientNoModule();
        using var patch = new PublishPatch();
        var message = new IotMessage();
        message.SetProperty("key one", "value&two");
        var outputName = "out put&name=日本語";

        await client.SendEventAsync(outputName, message, TransportTopic.Iothub);

        var expectedPropertyString = $"{Uri.EscapeDataString("key one")}={Uri.EscapeDataString("value&two")}"
            + "&" + $"{Uri.EscapeDataString("$.on")}={Uri.EscapeDataString(outputName)}";
        var call = Assert.Single(patch.Calls);
        Assert.Equal($"devices/dev1/messages/events/{expectedPropertyString}", call.Topic);
        Assert.Contains("key%20one", call.Topic);
        Assert.Contains("value%26two", call.Topic);
        Assert.Contains("%24.on=", call.Topic);
        Assert.DoesNotContain(" ", call.Topic);
    }

    [Fact(DisplayName = "No474_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public async Task Case474()
    {
        var client = NewMqttModuleClientNoModule();
        using var patch = new PublishPatch();

        await Assert.ThrowsAsync<NullReferenceException>(
            () => client.SendEventAsync("output", null!, TransportTopic.Iothub));
    }

    [Fact(DisplayName = "No475_異常系：異常動作の確認_IotHub指定時にoutputNameにnullを指定して、メソッドを呼び出す")]
    public async Task Case475()
    {
        var client = NewMqttModuleClientNoModule();
        using var patch = new PublishPatch();
        var message = new IotMessage();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => client.SendEventAsync(null!, message, TransportTopic.Iothub));
    }

    [Fact(DisplayName = "No476_正常系：正常動作の確認_Mqtt指定時にoutputNameにnullを指定して、メソッドを呼び出す")]
    public async Task Case476()
    {
        var client = NewMqttModuleClientNoModule();
        using var patch = new PublishPatch();
        var message = new IotMessage();
        message.SetProperty("key1", "value1");

        await client.SendEventAsync(null!, message, TransportTopic.Mqtt);

        var call = Assert.Single(patch.Calls);
        Assert.Equal("/key1=value1", call.Topic);
    }

    [Fact(DisplayName = "No477_正常系：正常動作の確認_TransportTopic.Iothub、TransportTopic.Mqtt以外の列挙値を指定して、メソッドを呼び出す")]
    public async Task Case477()
    {
        var client = NewMqttModuleClientNoModule();
        using var patch = new PublishPatch();
        var message = new IotMessage();

        await client.SendEventAsync("output", message, (TransportTopic)99);

        var call = Assert.Single(patch.Calls);
        Assert.Equal("output/", call.Topic);
    }

    private static MqttModuleClient NewMqttModuleClientNoModule(TransportTopic defaultSendTopic = TransportTopic.Iothub)
    {
        return new MqttModuleClient("sas-token", "localhost", "dev1", defaultSendTopic);
    }

    private static MqttModuleClient NewMqttModuleClientWithModule()
    {
        using var env = new EnvironmentVariablesScope(
            ("IOTEDGE_DEVICEID", "dev1"),
            ("IOTEDGE_MODULEID", "mod1"));

        return new MqttModuleClient("sas-token", "localhost");
    }

    private sealed class EnvironmentVariablesScope : IDisposable
    {
        private readonly List<(string Name, string? OriginalValue)> originalValues = new();

        public EnvironmentVariablesScope(params (string Name, string? Value)[] variables)
        {
            foreach (var (name, value) in variables)
            {
                originalValues.Add((name, Environment.GetEnvironmentVariable(name)));
                Environment.SetEnvironmentVariable(name, value);
            }
        }

        public void Dispose()
        {
            foreach (var (name, originalValue) in originalValues)
            {
                Environment.SetEnvironmentVariable(name, originalValue);
            }
        }
    }

    private sealed class SendEventAsyncPatch : IDisposable
    {
        private readonly Harmony harmony;

        public static List<SendEventAsyncCall> Calls { get; } = new();

        public SendEventAsyncPatch()
        {
            Calls.Clear();

            harmony = new Harmony($"{nameof(MqttModuleClient_SendEventAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(MqttModuleClient), nameof(MqttModuleClient.SendEventAsync), new[] { typeof(string), typeof(IotMessage), typeof(TransportTopic) }),
                prefix: new HarmonyMethod(typeof(SendEventAsyncPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(string __0, IotMessage __1, TransportTopic __2, ref Task __result)
        {
            Calls.Add(new SendEventAsyncCall(__0, __1, __2));
            __result = Task.CompletedTask;
            return false;
        }
    }

    private sealed record SendEventAsyncCall(string OutputName, IotMessage Message, TransportTopic TransportTopic);

    private sealed class PublishPatch : IDisposable
    {
        private readonly Harmony harmony;

        public List<PublishCall> Calls { get; } = new();

        private static PublishPatch? current;

        public PublishPatch()
        {
            current = this;

            harmony = new Harmony($"{nameof(MqttModuleClient_SendEventAsync)}.{Guid.NewGuid()}");
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
            __result = 0;
            return false;
        }
    }

    private sealed record PublishCall(string Topic, byte[] Payload);
}
