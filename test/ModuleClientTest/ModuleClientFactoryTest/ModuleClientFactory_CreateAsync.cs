using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.ModuleClientFactoryTest;

[Collection(nameof(ModuleClientFactoryTest))]
public class ModuleClientFactory_CreateAsync : IDisposable
{
    private const string ENV_TRANSPORT_PROTOCOL = "TransportProtocol";

    protected readonly Harmony _harmony = new (Guid.NewGuid().ToString());

    private static bool _createMethodCalled = false;

    private static ITransportSettings[]? _settings = null;

    public ModuleClientFactory_CreateAsync()
    {
        var createMethod = AccessTools.Method(
            typeof(IotHubModuleClient),
            nameof(IotHubModuleClient.CreateAsync),
            [ typeof(ITransportSettings[]), typeof(TransportTopic), typeof(TransportTopic), typeof(ClientOptions) ]);
        _harmony.Patch(createMethod,
            prefix: new HarmonyMethod(typeof(ModuleClientFactory_CreateAsync), nameof(CreatePrefix)),
            postfix: new HarmonyMethod(typeof(ModuleClientFactory_CreateAsync), nameof(CreatePostfix)));
    }

    [HarmonyPrefix]
    private static bool CreatePrefix(ITransportSettings[]? settings)
    {
        _createMethodCalled = true;
        _settings = settings;
        return false;
    }

    [HarmonyPostfix]
    private static void CreatePostfix(ref Task<IotHubModuleClient?> __result)
    {
        __result = Task.FromResult<IotHubModuleClient?>(null);
    }

    [Fact(DisplayName = "ModuleClientFactory No001_環境変数: TransportProtocolに列挙型TransportProtocolに変換可能な値が設定されている(\"mqtt\": 小文字)")]
    public async void ModuleClientFactory_Case001()
    {
        Environment.SetEnvironmentVariable(ENV_TRANSPORT_PROTOCOL, "mqtt");
        await ModuleClientFactory.CreateAsync();

        Assert.True(_createMethodCalled);
        Assert.NotNull(_settings);
        Assert.Single(_settings);
        Assert.IsType<MqttTransportSettings>(_settings![0]);
    }

    [Fact(DisplayName = "ModuleClientFactory No002_環境変数: TransportProtocolに列挙型TransportProtocolに変換可能な値が設定されている(\"AMQP\": 大文字)")]
    public async void ModuleClientFactory_Case002()
    {
        Environment.SetEnvironmentVariable(ENV_TRANSPORT_PROTOCOL, "AMQP");
        await ModuleClientFactory.CreateAsync();

        Assert.True(_createMethodCalled);
        Assert.NotNull(_settings);
        Assert.Single(_settings);
        Assert.IsType<AmqpTransportSettings>(_settings![0]);
    }

    [Fact(DisplayName = "ModuleClientFactory No003_環境変数: TransportProtocolに列挙型TransportProtocolに変換可能な値が設定されている(\"0\": 数値)")]
    public async void ModuleClientFactory_Case003()
    {
        Environment.SetEnvironmentVariable(ENV_TRANSPORT_PROTOCOL, "0");
        await ModuleClientFactory.CreateAsync();

        Assert.True(_createMethodCalled);
        Assert.NotNull(_settings);
        Assert.Single(_settings);
        Assert.IsType<AmqpTransportSettings>(_settings![0]);
    }

    [Fact(DisplayName = "ModuleClientFactory No004_環境変数: TransportProtocolが設定されていない")]
    public async void ModuleClientFactory_Case004()
    {
        await ModuleClientFactory.CreateAsync();

        Assert.True(_createMethodCalled);
        Assert.NotNull(_settings);
        Assert.Single(_settings);
        Assert.IsType<AmqpTransportSettings>(_settings![0]);
    }

    [Fact(DisplayName = "ModuleClientFactory No005_環境変数: TransportProtocolに列挙型TransportProtocolに変換できない値が設定されている(\"TestProtocol\")")]
    public async void ModuleClientFactory_Case005()
    {
        Environment.SetEnvironmentVariable(ENV_TRANSPORT_PROTOCOL, "TestProtocol");
        await Assert.ThrowsAsync<ArgumentException>(async () => await ModuleClientFactory.CreateAsync());
    }

    [Fact(DisplayName = "ModuleClientFactory No006_環境変数: TransportProtocolに列挙型TransportProtocolに変換可能な値が設定されているが、サポート外(\"999\": 数値)")]
    public async void ModuleClientFactory_Case006()
    {
        Environment.SetEnvironmentVariable(ENV_TRANSPORT_PROTOCOL, "999");
        await Assert.ThrowsAsync<ArgumentException>(async () => await ModuleClientFactory.CreateAsync());
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(ENV_TRANSPORT_PROTOCOL, null);
        _harmony.UnpatchAll(_harmony.Id);
        _createMethodCalled = false;
        _settings = null;
    }
}
