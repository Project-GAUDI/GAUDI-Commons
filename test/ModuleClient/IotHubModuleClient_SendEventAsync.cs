using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotHubModuleClient_SendEventAsync
{
    [Fact(DisplayName = "No420_正常系：正常動作の確認_outputName、messageを指定して、メソッドを呼び出す")]
    public async Task Case420()
    {
        using var patch = new SendEventAsyncPatch();
        var client = NewIotHubModuleClient(TransportTopic.Mqtt);
        var message = new IotMessage();

        // CancellationTokenを渡さないオーバーロードの挙動を検証するテストのため、あえてTestContext.Current.CancellationTokenは使用しない
#pragma warning disable xUnit1051
        await client.SendEventAsync("output", message);
#pragma warning restore xUnit1051

        var call = Assert.Single(SendEventAsyncPatch.Calls);
        Assert.Equal("output", call.OutputName);
        Assert.Same(message, call.Message);
        Assert.Equal(TransportTopic.Mqtt, call.TransportTopic);
        Assert.Equal(CancellationToken.None, call.CancellationToken);
    }

    [Fact(DisplayName = "No421_正常系：正常動作の確認_outputName、message、cancellationTokenを指定して、メソッドを呼び出す")]
    public async Task Case421()
    {
        using var patch = new SendEventAsyncPatch();
        var client = NewIotHubModuleClient(TransportTopic.Mqtt);
        var message = new IotMessage();
        using var cts = new CancellationTokenSource();

        await client.SendEventAsync("output", message, cts.Token);

        var call = Assert.Single(SendEventAsyncPatch.Calls);
        Assert.Equal("output", call.OutputName);
        Assert.Same(message, call.Message);
        Assert.Equal(TransportTopic.Mqtt, call.TransportTopic);
        Assert.Equal(cts.Token, call.CancellationToken);
    }

    [Fact(DisplayName = "No422_正常系：正常動作の確認_outputName、message、transportTopicを指定して、メソッドを呼び出す")]
    public async Task Case422()
    {
        using var patch = new SendEventAsyncPatch();
        var client = NewIotHubModuleClient(TransportTopic.Iothub);
        var message = new IotMessage();

        // CancellationTokenを渡さないオーバーロードの挙動を検証するテストのため、あえてTestContext.Current.CancellationTokenは使用しない
#pragma warning disable xUnit1051
        await InvokeProtectedSendEventAsync(client, "output", message, TransportTopic.Mqtt);
#pragma warning restore xUnit1051

        var call = Assert.Single(SendEventAsyncPatch.Calls);
        Assert.Equal("output", call.OutputName);
        Assert.Same(message, call.Message);
        Assert.Equal(TransportTopic.Mqtt, call.TransportTopic);
        Assert.Equal(CancellationToken.None, call.CancellationToken);
    }

    [Fact(DisplayName = "No423_正常系：正常動作の確認_message(MessageId=null)、transportTopic=Iothub、cancellationTokenを指定して、メソッドを呼び出す")]
    public async Task Case423()
    {
        using var utilPatch = new GetMessageIdPatch();
        using var modulePatch = new ModuleClientSendEventAsyncPatch();
        var client = NewIotHubModuleClient(TransportTopic.Iothub, modulePatch.ModuleClient);
        var message = new IotMessage();
        using var cts = new CancellationTokenSource();

        await InvokeProtectedSendEventAsync(client, "output", message, TransportTopic.Iothub, cts.Token);

        Assert.Equal(1, GetMessageIdPatch.CallCount);
        Assert.Equal(GetMessageIdPatch.GeneratedId, message.GetMessageId());
        var call = Assert.Single(ModuleClientSendEventAsyncPatch.Calls);
        Assert.Equal("output", call.OutputName);
        Assert.Same(message.GetMessage(), call.Message);
        Assert.Equal(cts.Token, call.CancellationToken);
    }

    [Fact(DisplayName = "No424_正常系：正常動作の確認_message(MessageId!=null)、transportTopic=Iothub、cancellationTokenを指定して、メソッドを呼び出す")]
    public async Task Case424()
    {
        using var utilPatch = new GetMessageIdPatch();
        using var modulePatch = new ModuleClientSendEventAsyncPatch();
        var client = NewIotHubModuleClient(TransportTopic.Iothub, modulePatch.ModuleClient);
        var message = new IotMessage();
        message.SetMessageId("existing-message-id");
        using var cts = new CancellationTokenSource();

        await InvokeProtectedSendEventAsync(client, "output", message, TransportTopic.Iothub, cts.Token);

        Assert.Equal(0, GetMessageIdPatch.CallCount);
        Assert.Equal("existing-message-id", message.GetMessageId());
        var call = Assert.Single(ModuleClientSendEventAsyncPatch.Calls);
        Assert.Equal("output", call.OutputName);
        Assert.Same(message.GetMessage(), call.Message);
        Assert.Equal(cts.Token, call.CancellationToken);
    }

    [Fact(DisplayName = "No425_異常系：異常動作の確認_transportTopic=Mqtt、cancellationTokenを指定して、メソッドを呼び出す")]
    public async Task Case425()
    {
        using var modulePatch = new ModuleClientSendEventAsyncPatch();
        var client = NewIotHubModuleClient(TransportTopic.Iothub, modulePatch.ModuleClient);
        var message = new IotMessage();
        using var cts = new CancellationTokenSource();

        await Assert.ThrowsAsync<NotImplementedException>(
            () => InvokeProtectedSendEventAsync(client, "output", message, TransportTopic.Mqtt, cts.Token));
    }

    private static IotHubModuleClient NewIotHubModuleClient(TransportTopic defaultSendTopic, ModuleClient? moduleClient = null)
    {
        var client = (IotHubModuleClient)Activator.CreateInstance(
            typeof(IotHubModuleClient),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: Array.Empty<object>(),
            culture: null)!;

        typeof(IotHubModuleClient).GetProperty("MyModuleClient", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(client, moduleClient ?? (ModuleClient)RuntimeHelpers.GetUninitializedObject(typeof(ModuleClient)));
        typeof(IotHubModuleClient).GetField("defaultSendTopic", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(client, defaultSendTopic);
        return client;
    }

    private static Task InvokeProtectedSendEventAsync(IotHubModuleClient client, string outputName, IotMessage message, TransportTopic transportTopic)
    {
        var method = typeof(IotHubModuleClient).GetMethod(
            "SendEventAsync",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(string), typeof(IotMessage), typeof(TransportTopic) },
            modifiers: null)!;
        return (Task)method.Invoke(client, new object[] { outputName, message, transportTopic })!;
    }

    private static Task InvokeProtectedSendEventAsync(IotHubModuleClient client, string outputName, IotMessage message, TransportTopic transportTopic, CancellationToken cancellationToken)
    {
        var method = typeof(IotHubModuleClient).GetMethod(
            "SendEventAsync",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(string), typeof(IotMessage), typeof(TransportTopic), typeof(CancellationToken) },
            modifiers: null)!;
        return (Task)method.Invoke(client, new object[] { outputName, message, transportTopic, cancellationToken })!;
    }

    private sealed class SendEventAsyncPatch : IDisposable
    {
        private readonly Harmony harmony;

        public static List<SendEventAsyncCall> Calls { get; } = new();

        public SendEventAsyncPatch()
        {
            Calls.Clear();

            harmony = new Harmony($"{nameof(IotHubModuleClient_SendEventAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(IotHubModuleClient), "SendEventAsync", new[] { typeof(string), typeof(IotMessage), typeof(TransportTopic), typeof(CancellationToken) }),
                prefix: new HarmonyMethod(typeof(SendEventAsyncPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(string __0, IotMessage __1, TransportTopic __2, CancellationToken __3, ref Task __result)
        {
            Calls.Add(new SendEventAsyncCall(__0, __1, __2, __3));
            __result = Task.CompletedTask;
            return false;
        }
    }

    private sealed record SendEventAsyncCall(string OutputName, IotMessage Message, TransportTopic TransportTopic, CancellationToken CancellationToken);

    private sealed class GetMessageIdPatch : IDisposable
    {
        private readonly Harmony harmony;

        public static int CallCount { get; private set; }

        public static string GeneratedId { get; } = "generated-message-id";

        public GetMessageIdPatch()
        {
            CallCount = 0;

            harmony = new Harmony($"{nameof(IotHubModuleClient_SendEventAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(Util), nameof(Util.GetMessageId)),
                prefix: new HarmonyMethod(typeof(GetMessageIdPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(ref string __result)
        {
            CallCount++;
            __result = GeneratedId;
            return false;
        }
    }

    private sealed class ModuleClientSendEventAsyncPatch : IDisposable
    {
        private readonly Harmony harmony;

        public ModuleClient ModuleClient { get; }

        public static List<ModuleClientSendEventAsyncCall> Calls { get; } = new();

        public ModuleClientSendEventAsyncPatch()
        {
            Calls.Clear();
            ModuleClient = (ModuleClient)RuntimeHelpers.GetUninitializedObject(typeof(ModuleClient));

            harmony = new Harmony($"{nameof(IotHubModuleClient_SendEventAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.SendEventAsync), new[] { typeof(string), typeof(Message), typeof(CancellationToken) }),
                prefix: new HarmonyMethod(typeof(ModuleClientSendEventAsyncPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(string __0, Message __1, CancellationToken __2, ref Task __result)
        {
            Calls.Add(new ModuleClientSendEventAsyncCall(__0, __1, __2));
            __result = Task.CompletedTask;
            return false;
        }
    }

    private sealed record ModuleClientSendEventAsyncCall(string OutputName, Message Message, CancellationToken CancellationToken);
}
