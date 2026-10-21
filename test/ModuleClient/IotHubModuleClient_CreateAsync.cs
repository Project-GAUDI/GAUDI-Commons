using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(IotHubModuleClientHarmonyCollection))]
public sealed class IotHubModuleClient_CreateAsync
{
    [Fact(DisplayName = "No411_正常系：正常動作の確認_引数を指定せず、メソッドを呼び出す")]
    public async Task Case411()
    {
        using var patch = new ModuleClientPatch();

        var result = await IotHubModuleClient.CreateAsync();

        Assert.NotNull(result);
        Assert.IsType<IotHubModuleClient>(result);
        var call = Assert.Single(ModuleClientPatch.CreateCalls);
        Assert.Null(call.Settings);
        Assert.Null(call.Options);
    }

    [Fact(DisplayName = "No412_正常系：正常動作の確認_defaultSendTopic、defaultReceiveTopic、optionsを指定して、メソッドを呼び出す")]
    public async Task Case412()
    {
        using var patch = new ModuleClientPatch();
        var options = new ClientOptions();

        var result = await IotHubModuleClient.CreateAsync(
            defaultSendTopic: TransportTopic.Mqtt,
            defaultReceiveTopic: TransportTopic.Mqtt,
            options: options);

        Assert.NotNull(result);
        Assert.IsType<IotHubModuleClient>(result);
        var call = Assert.Single(ModuleClientPatch.CreateCalls);
        Assert.Null(call.Settings);
        Assert.Same(options, call.Options);
    }

    [Fact(DisplayName = "No413_正常系：正常動作の確認_settings、defaultSendTopic、defaultReceiveTopicを指定して、メソッドを呼び出す")]
    public async Task Case413()
    {
        using var patch = new ModuleClientPatch();
        var settings = new ITransportSettings[] { new MqttTransportSettings(TransportType.Mqtt_Tcp_Only) };

        var result = await IotHubModuleClient.CreateAsync(
            settings,
            TransportTopic.Mqtt,
            TransportTopic.Mqtt);

        Assert.NotNull(result);
        Assert.IsType<IotHubModuleClient>(result);
        var call = Assert.Single(ModuleClientPatch.CreateCalls);
        Assert.Same(settings, call.Settings);
        Assert.Null(call.Options);
    }

    [Fact(DisplayName = "No414_正常系：正常動作の確認_settings、defaultSendTopic、defaultReceiveTopic、optionsを指定して、メソッドを呼び出す")]
    public async Task Case414()
    {
        using var patch = new ModuleClientPatch();
        var settings = new ITransportSettings[] { new MqttTransportSettings(TransportType.Mqtt_Tcp_Only) };
        var options = new ClientOptions();

        var result = await IotHubModuleClient.CreateAsync(
            settings,
            TransportTopic.Mqtt,
            TransportTopic.Mqtt,
            options);

        Assert.NotNull(result);
        Assert.IsType<IotHubModuleClient>(result);
        var call = Assert.Single(ModuleClientPatch.CreateCalls);
        Assert.Same(settings, call.Settings);
        Assert.Same(options, call.Options);
    }

    [Fact(DisplayName = "No415_正常系：正常動作の確認_メソッドを呼び出す")]
    public async Task Case415()
    {
        using var patch = new ModuleClientPatch();

        var result = await IotHubModuleClient.CreateAsync();

        Assert.NotNull(result);
        Assert.IsType<IotHubModuleClient>(result);
        Assert.Equal(1, ModuleClientPatch.SetConnectionStatusChangesHandlerCallCount);
        Assert.NotNull(ModuleClientPatch.ConnectionStatusChangesHandler);
    }

    private sealed class ModuleClientPatch : IDisposable
    {
        private readonly Harmony harmony;

        public static List<CreateCall> CreateCalls { get; } = new();

        public static int SetConnectionStatusChangesHandlerCallCount { get; private set; }

        public static ConnectionStatusChangesHandler? ConnectionStatusChangesHandler { get; private set; }

        private static ModuleClient PatchedModuleClient { get; set; } = null!;

        public ModuleClientPatch()
        {
            CreateCalls.Clear();
            SetConnectionStatusChangesHandlerCallCount = 0;
            ConnectionStatusChangesHandler = null;
            PatchedModuleClient = (ModuleClient)RuntimeHelpers.GetUninitializedObject(typeof(ModuleClient));

            harmony = new Harmony($"{nameof(IotHubModuleClient_CreateAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.CreateFromEnvironmentAsync), new[] { typeof(ClientOptions) }),
                prefix: new HarmonyMethod(typeof(ModuleClientPatch), nameof(CreateFromEnvironmentWithOptionsPrefix)));
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.CreateFromEnvironmentAsync), new[] { typeof(ITransportSettings[]), typeof(ClientOptions) }),
                prefix: new HarmonyMethod(typeof(ModuleClientPatch), nameof(CreateFromEnvironmentWithSettingsAndOptionsPrefix)));
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.SetConnectionStatusChangesHandler), new[] { typeof(ConnectionStatusChangesHandler) }),
                prefix: new HarmonyMethod(typeof(ModuleClientPatch), nameof(SetConnectionStatusChangesHandlerPrefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool CreateFromEnvironmentWithOptionsPrefix(ClientOptions __0, ref Task<ModuleClient> __result)
        {
            CreateCalls.Add(new CreateCall(null, __0));
            __result = Task.FromResult(PatchedModuleClient);
            return false;
        }

        private static bool CreateFromEnvironmentWithSettingsAndOptionsPrefix(ITransportSettings[] __0, ClientOptions __1, ref Task<ModuleClient> __result)
        {
            CreateCalls.Add(new CreateCall(__0, __1));
            __result = Task.FromResult(PatchedModuleClient);
            return false;
        }

        private static bool SetConnectionStatusChangesHandlerPrefix(ConnectionStatusChangesHandler __0)
        {
            SetConnectionStatusChangesHandlerCallCount++;
            ConnectionStatusChangesHandler = __0;
            return false;
        }
    }

    private sealed record CreateCall(ITransportSettings[]? Settings, ClientOptions? Options);
}