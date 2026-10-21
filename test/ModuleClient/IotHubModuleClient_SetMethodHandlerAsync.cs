using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotHubModuleClient_SetMethodHandlerAsync
{
    [Fact(DisplayName = "No432_正常系：正常動作の確認_defaultReceiveTopic=Iothubの状態で、methodName、methodHandler、userContextを指定して、メソッドを呼び出す")]
    public async Task Case432()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Iothub);
        Task<MethodResponse> Handler(MethodRequest request, object context) => Task.FromResult(new MethodResponse(200));
        var userContext = new object();

        await client.SetMethodHandlerAsync("method", Handler, userContext);

        var call = Assert.Single(ModuleClientPatch.Calls);
        Assert.Equal("method", call.MethodName);
        Assert.Equal((MethodCallback)Handler, call.MethodHandler);
        Assert.Same(userContext, call.UserContext);
    }

    [Fact(DisplayName = "No433_正常系：正常動作の確認_methodName、methodHandler、userContext、transportTopic=Iothubを指定して、メソッドを呼び出す")]
    public async Task Case433()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Mqtt);
        Task<MethodResponse> Handler(MethodRequest request, object context) => Task.FromResult(new MethodResponse(200));
        var userContext = new object();

        await InvokeProtectedSetMethodHandlerAsync(client, "method", Handler, userContext, TransportTopic.Iothub);

        var call = Assert.Single(ModuleClientPatch.Calls);
        Assert.Equal("method", call.MethodName);
        Assert.Equal((MethodCallback)Handler, call.MethodHandler);
        Assert.Same(userContext, call.UserContext);
    }

    [Fact(DisplayName = "No434_異常系：異常動作の確認_methodName、methodHandler、userContext、transportTopic=Mqttを指定して、メソッドを呼び出す")]
    public async Task Case434()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Iothub);
        Task<MethodResponse> Handler(MethodRequest request, object context) => Task.FromResult(new MethodResponse(200));
        var userContext = new object();

        await Assert.ThrowsAsync<NotImplementedException>(
            () => InvokeProtectedSetMethodHandlerAsync(client, "method", Handler, userContext, TransportTopic.Mqtt));
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

    private static Task InvokeProtectedSetMethodHandlerAsync(IotHubModuleClient client, string methodName, MethodCallback methodHandler, object userContext, TransportTopic transportTopic)
    {
        var method = typeof(IotHubModuleClient).GetMethod(
            "SetMethodHandlerAsync",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(string), typeof(MethodCallback), typeof(object), typeof(TransportTopic) },
            modifiers: null)!;
        return (Task)method.Invoke(client, new object[] { methodName, methodHandler, userContext, transportTopic })!;
    }

    private sealed class ModuleClientPatch : IDisposable
    {
        private readonly Harmony harmony;

        public ModuleClient ModuleClient { get; }

        public static List<SetMethodHandlerCall> Calls { get; } = new();

        public ModuleClientPatch()
        {
            Calls.Clear();
            ModuleClient = (ModuleClient)RuntimeHelpers.GetUninitializedObject(typeof(ModuleClient));

            harmony = new Harmony($"{nameof(IotHubModuleClient_SetMethodHandlerAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.SetMethodHandlerAsync), new[] { typeof(string), typeof(MethodCallback), typeof(object) }),
                prefix: new HarmonyMethod(typeof(ModuleClientPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(string __0, MethodCallback __1, object __2, ref Task __result)
        {
            Calls.Add(new SetMethodHandlerCall(__0, __1, __2));
            __result = Task.CompletedTask;
            return false;
        }
    }

    private sealed record SetMethodHandlerCall(string MethodName, MethodCallback MethodHandler, object UserContext);
}
