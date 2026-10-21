using System.Reflection;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ModuleClientFactory_CreateAsync
{
    [Fact(DisplayName = "No440_正常系：デフォルト値の確認_ENV:TransportProtocol=未設定の状態で、メソッドを呼び出す")]
    public async Task Case440()
    {
        using var env = new TransportProtocolEnvironment(null);
        using var patch = new IotHubModuleClientCreateAsyncPatch();

        var result = await ModuleClientFactory.CreateAsync();

        Assert.Same(IotHubModuleClientCreateAsyncPatch.FakeClient, result);
        var settings = Assert.Single(IotHubModuleClientCreateAsyncPatch.Calls);
        var setting = Assert.Single(settings);
        Assert.IsType<AmqpTransportSettings>(setting);
    }

    [Fact(DisplayName = "No441_正常系：デフォルト値の確認_ENV:TransportProtocol=空文字の状態で、メソッドを呼び出す")]
    public async Task Case441()
    {
        using var env = new TransportProtocolEnvironment("");
        using var patch = new IotHubModuleClientCreateAsyncPatch();

        var result = await ModuleClientFactory.CreateAsync();

        Assert.Same(IotHubModuleClientCreateAsyncPatch.FakeClient, result);
        var settings = Assert.Single(IotHubModuleClientCreateAsyncPatch.Calls);
        var setting = Assert.Single(settings);
        Assert.IsType<AmqpTransportSettings>(setting);
    }

    [Fact(DisplayName = "No442_正常系：正常値の確認（string型）_ENV:TransportProtocol=AMQPの状態で、メソッドを呼び出す")]
    public async Task Case442()
    {
        using var env = new TransportProtocolEnvironment("AMQP");
        using var patch = new IotHubModuleClientCreateAsyncPatch();

        var result = await ModuleClientFactory.CreateAsync();

        Assert.Same(IotHubModuleClientCreateAsyncPatch.FakeClient, result);
        var settings = Assert.Single(IotHubModuleClientCreateAsyncPatch.Calls);
        var setting = Assert.Single(settings);
        Assert.IsType<AmqpTransportSettings>(setting);
    }

    [Fact(DisplayName = "No443_正常系：正常値の確認（string型）_ENV:TransportProtocol=amqpの状態で、メソッドを呼び出す")]
    public async Task Case443()
    {
        using var env = new TransportProtocolEnvironment("amqp");
        using var patch = new IotHubModuleClientCreateAsyncPatch();

        var result = await ModuleClientFactory.CreateAsync();

        Assert.Same(IotHubModuleClientCreateAsyncPatch.FakeClient, result);
        var settings = Assert.Single(IotHubModuleClientCreateAsyncPatch.Calls);
        var setting = Assert.Single(settings);
        Assert.IsType<AmqpTransportSettings>(setting);
    }

    [Fact(DisplayName = "No444_正常系：正常値の確認（string型）_ENV:TransportProtocol=MQTTの状態で、メソッドを呼び出す")]
    public async Task Case444()
    {
        using var env = new TransportProtocolEnvironment("MQTT");
        using var patch = new IotHubModuleClientCreateAsyncPatch();

        var result = await ModuleClientFactory.CreateAsync();

        Assert.Same(IotHubModuleClientCreateAsyncPatch.FakeClient, result);
        var settings = Assert.Single(IotHubModuleClientCreateAsyncPatch.Calls);
        var setting = Assert.Single(settings);
        Assert.IsType<MqttTransportSettings>(setting);
    }

    [Fact(DisplayName = "No445_正常系：正常値の確認（string型）_ENV:TransportProtocol=mqttの状態で、メソッドを呼び出す")]
    public async Task Case445()
    {
        using var env = new TransportProtocolEnvironment("mqtt");
        using var patch = new IotHubModuleClientCreateAsyncPatch();

        var result = await ModuleClientFactory.CreateAsync();

        Assert.Same(IotHubModuleClientCreateAsyncPatch.FakeClient, result);
        var settings = Assert.Single(IotHubModuleClientCreateAsyncPatch.Calls);
        var setting = Assert.Single(settings);
        Assert.IsType<MqttTransportSettings>(setting);
    }

    [Fact(DisplayName = "No446_異常系：不正値の確認（string型）_ENV:TransportProtocol=不正値(InvalidValue)の状態で、メソッドを呼び出す")]
    public async Task Case446()
    {
        using var env = new TransportProtocolEnvironment("InvalidValue");
        using var patch = new IotHubModuleClientCreateAsyncPatch();

        await Assert.ThrowsAsync<ArgumentException>(() => ModuleClientFactory.CreateAsync());

        Assert.Empty(IotHubModuleClientCreateAsyncPatch.Calls);
    }

    private sealed class TransportProtocolEnvironment : IDisposable
    {
        private const string Name = "TransportProtocol";
        private readonly string? original;

        public TransportProtocolEnvironment(string? value)
        {
            original = Environment.GetEnvironmentVariable(Name);
            Environment.SetEnvironmentVariable(Name, value);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(Name, original);
        }
    }

    private sealed class IotHubModuleClientCreateAsyncPatch : IDisposable
    {
        private readonly Harmony harmony;

        public static IotHubModuleClient FakeClient { get; } = (IotHubModuleClient)Activator.CreateInstance(
            typeof(IotHubModuleClient),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: Array.Empty<object>(),
            culture: null)!;

        public static List<ITransportSettings[]> Calls { get; } = new();

        public IotHubModuleClientCreateAsyncPatch()
        {
            Calls.Clear();

            harmony = new Harmony($"{nameof(ModuleClientFactory_CreateAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(IotHubModuleClient), nameof(IotHubModuleClient.CreateAsync), new[] { typeof(ITransportSettings[]), typeof(TransportTopic), typeof(TransportTopic), typeof(ClientOptions) }),
                prefix: new HarmonyMethod(typeof(IotHubModuleClientCreateAsyncPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(ITransportSettings[] __0, ref Task<IotHubModuleClient> __result)
        {
            Calls.Add(__0);
            __result = Task.FromResult(FakeClient);
            return false;
        }
    }
}
