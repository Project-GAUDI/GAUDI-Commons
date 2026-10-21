using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotHubModuleClient_SetInputMessageHandlerAsync
{
    [Fact(DisplayName = "No429_正常系：正常動作の確認_defaultReceiveTopic=Iothubの状態で、inputName、iotHandler、userContextを指定して、メソッドを呼び出す")]
    public async Task Case429()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Iothub);
        var received = new List<(IotMessage Message, object Context)>();
        Task<MessageResponse> IotHandler(IotMessage message, object context)
        {
            received.Add((message, context));
            return Task.FromResult(MessageResponse.Completed);
        }
        var userContext = new object();

        await client.SetInputMessageHandlerAsync("input", IotHandler, userContext);

        var call = Assert.Single(ModuleClientPatch.Calls);
        Assert.Equal("input", call.InputName);
        Assert.Same(userContext, call.UserContext);

        var rawMessage = new Message();
        var response = await call.Handler(rawMessage, userContext);

        Assert.Equal(MessageResponse.Completed, response);
        var received1 = Assert.Single(received);
        Assert.Same(userContext, received1.Context);
        Assert.Same(rawMessage, received1.Message.GetMessage());
    }

    [Fact(DisplayName = "No430_正常系：正常動作の確認_inputName、iotHandler、userContext、transportTopic=Iothubを指定して、メソッドを呼び出す")]
    public async Task Case430()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Mqtt);
        Task<MessageResponse> IotHandler(IotMessage message, object context) => Task.FromResult(MessageResponse.Completed);
        var userContext = new object();

        await InvokeProtectedSetInputMessageHandlerAsync(client, "input", IotHandler, userContext, TransportTopic.Iothub);

        var call = Assert.Single(ModuleClientPatch.Calls);
        Assert.Equal("input", call.InputName);
        Assert.Same(userContext, call.UserContext);
    }

    [Fact(DisplayName = "No431_異常系：異常動作の確認_inputName、iotHandler、userContext、transportTopic=Mqttを指定して、メソッドを呼び出す")]
    public async Task Case431()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Iothub);
        Task<MessageResponse> IotHandler(IotMessage message, object context) => Task.FromResult(MessageResponse.Completed);
        var userContext = new object();

        await Assert.ThrowsAsync<NotImplementedException>(
            () => InvokeProtectedSetInputMessageHandlerAsync(client, "input", IotHandler, userContext, TransportTopic.Mqtt));
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

    private static Task InvokeProtectedSetInputMessageHandlerAsync(IotHubModuleClient client, string inputName, IotMessageHandler iotHandler, object userContext, TransportTopic transportTopic)
    {
        var method = typeof(IotHubModuleClient).GetMethod(
            "SetInputMessageHandlerAsync",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(string), typeof(IotMessageHandler), typeof(object), typeof(TransportTopic) },
            modifiers: null)!;
        return (Task)method.Invoke(client, new object[] { inputName, iotHandler, userContext, transportTopic })!;
    }

    private sealed class ModuleClientPatch : IDisposable
    {
        private readonly Harmony harmony;

        public ModuleClient ModuleClient { get; }

        public static List<SetInputMessageHandlerCall> Calls { get; } = new();

        public ModuleClientPatch()
        {
            Calls.Clear();
            ModuleClient = (ModuleClient)RuntimeHelpers.GetUninitializedObject(typeof(ModuleClient));

            harmony = new Harmony($"{nameof(IotHubModuleClient_SetInputMessageHandlerAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.SetInputMessageHandlerAsync), new[] { typeof(string), typeof(MessageHandler), typeof(object) }),
                prefix: new HarmonyMethod(typeof(ModuleClientPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(string __0, MessageHandler __1, object __2, ref Task __result)
        {
            Calls.Add(new SetInputMessageHandlerCall(__0, __1, __2));
            __result = Task.CompletedTask;
            return false;
        }
    }

    private sealed record SetInputMessageHandlerCall(string InputName, MessageHandler Handler, object UserContext);
}
