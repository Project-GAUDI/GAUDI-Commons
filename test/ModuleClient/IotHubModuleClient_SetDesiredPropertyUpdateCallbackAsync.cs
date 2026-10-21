using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotHubModuleClient_SetDesiredPropertyUpdateCallbackAsync
{
    [Fact(DisplayName = "No426_正常系：正常動作の確認_defaultReceiveTopic=Iothubの状態で、callback、userContextを指定して、メソッドを呼び出す")]
    public async Task Case426()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Iothub);
        Task Callback(TwinCollection desiredProperties, object userContext) => Task.CompletedTask;
        var userContext = new object();

        await client.SetDesiredPropertyUpdateCallbackAsync(Callback, userContext);

        var call = Assert.Single(ModuleClientPatch.Calls);
        Assert.Equal((DesiredPropertyUpdateCallback)Callback, call.Callback);
        Assert.Same(userContext, call.UserContext);
    }

    [Fact(DisplayName = "No427_正常系：正常動作の確認_callback、userContext、transportTopic=Iothubを指定して、メソッドを呼び出す")]
    public async Task Case427()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Mqtt);
        Task Callback(TwinCollection desiredProperties, object userContext) => Task.CompletedTask;
        var userContext = new object();

        await InvokeProtectedSetDesiredPropertyUpdateCallbackAsync(client, Callback, userContext, TransportTopic.Iothub);

        var call = Assert.Single(ModuleClientPatch.Calls);
        Assert.Equal((DesiredPropertyUpdateCallback)Callback, call.Callback);
        Assert.Same(userContext, call.UserContext);
    }

    [Fact(DisplayName = "No428_異常系：異常動作の確認_callback、userContext、transportTopic=Mqttを指定して、メソッドを呼び出す")]
    public async Task Case428()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Iothub);
        Task Callback(TwinCollection desiredProperties, object userContext) => Task.CompletedTask;
        var userContext = new object();

        await Assert.ThrowsAsync<NotImplementedException>(
            () => InvokeProtectedSetDesiredPropertyUpdateCallbackAsync(client, Callback, userContext, TransportTopic.Mqtt));
    }

    private static IotHubModuleClient NewIotHubModuleClient(ModuleClient moduleClient, TransportTopic defaultReceiveTopic)
    {
        var client = (IotHubModuleClient)Activator.CreateInstance(
            typeof(IotHubModuleClient),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: Array.Empty<object>(),
            culture: null)!;

        typeof(IotHubModuleClient).GetProperty("MyModuleClient", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(client, moduleClient);
        typeof(IotHubModuleClient).GetField("defaultReceiveTopic", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(client, defaultReceiveTopic);
        return client;
    }

    private static Task InvokeProtectedSetDesiredPropertyUpdateCallbackAsync(IotHubModuleClient client, DesiredPropertyUpdateCallback callback, object userContext, TransportTopic transportTopic)
    {
        var method = typeof(IotHubModuleClient).GetMethod(
            "SetDesiredPropertyUpdateCallbackAsync",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(DesiredPropertyUpdateCallback), typeof(object), typeof(TransportTopic) },
            modifiers: null)!;
        return (Task)method.Invoke(client, new object[] { callback, userContext, transportTopic })!;
    }

    private sealed class ModuleClientPatch : IDisposable
    {
        private readonly Harmony harmony;

        public ModuleClient ModuleClient { get; }

        public static List<SetDesiredPropertyUpdateCallbackCall> Calls { get; } = new();

        public ModuleClientPatch()
        {
            Calls.Clear();
            ModuleClient = (ModuleClient)RuntimeHelpers.GetUninitializedObject(typeof(ModuleClient));

            harmony = new Harmony($"{nameof(IotHubModuleClient_SetDesiredPropertyUpdateCallbackAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.SetDesiredPropertyUpdateCallbackAsync), new[] { typeof(DesiredPropertyUpdateCallback), typeof(object) }),
                prefix: new HarmonyMethod(typeof(ModuleClientPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(DesiredPropertyUpdateCallback __0, object __1, ref Task __result)
        {
            Calls.Add(new SetDesiredPropertyUpdateCallbackCall(__0, __1));
            __result = Task.CompletedTask;
            return false;
        }
    }

    private sealed record SetDesiredPropertyUpdateCallbackCall(DesiredPropertyUpdateCallback Callback, object UserContext);
}
