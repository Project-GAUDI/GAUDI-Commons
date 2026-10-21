using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(MqttModuleClientHarmonyCollection))]
public sealed class MqttModuleClient_SetInputMessageHandlerAsync
{
    [Fact(DisplayName = "No484_正常系：正常動作の確認_defaultReceiveTopic=Iothubの状態で、inputName、iotHandler、userContextを指定して、メソッドを呼び出す")]
    public async Task Case484()
    {
        var client = NewMqttModuleClientWithModule(TransportTopic.Iothub);
        using var patch = new SetInputMessageHandlerAsyncPatch();
        IotMessageHandler handler = (msg, obj) => Task.FromResult(MessageResponse.Completed);
        var userContext = new object();

        await client.SetInputMessageHandlerAsync("input1", handler, userContext);

        var call = Assert.Single(patch.Calls);
        Assert.Equal("input1", call.InputName);
        Assert.Same(handler, call.IotHandler);
        Assert.Same(userContext, call.UserContext);
        Assert.Equal(TransportTopic.Iothub, call.TransportTopic);
    }

    [Fact(DisplayName = "No485_正常系：正常動作の確認_defaultReceiveTopic=Mqttの状態で、outputName、messageを指定して、メソッドを呼び出す")]
    public async Task Case485()
    {
        var client = NewMqttModuleClientWithModule(TransportTopic.Mqtt);
        using var patch = new SetInputMessageHandlerAsyncPatch();
        IotMessageHandler handler = (msg, obj) => Task.FromResult(MessageResponse.Completed);
        var userContext = new object();

        await client.SetInputMessageHandlerAsync("input1", handler, userContext);

        var call = Assert.Single(patch.Calls);
        Assert.Equal("input1", call.InputName);
        Assert.Same(handler, call.IotHandler);
        Assert.Same(userContext, call.UserContext);
        Assert.Equal(TransportTopic.Mqtt, call.TransportTopic);
    }

    [Fact(DisplayName = "No486_正常系：正常動作の確認_inputName、iotHandler、userContext、TransportTopic.Iothubを指定して、メソッドを呼び出す")]
    public async Task Case486()
    {
        var client = NewMqttModuleClientWithModule();
        using var patch = new SubscribePatch();
        IotMessageHandler handler = (msg, obj) => Task.FromResult(MessageResponse.Completed);
        var userContext = new object();

        await client.SetInputMessageHandlerAsync("input1", handler, userContext, TransportTopic.Iothub);

        Assert.Equal(new[] { "input1" }, GetInputNames(client));
        Assert.True(GetDictCallbackProps(client).ContainsKey("input1"));
        Assert.Empty(patch.Calls);
    }

    [Fact(DisplayName = "No487_正常系：正常動作の確認_TransportTopic.Mqttを指定し、inputNameに#を含めずに、メソッドを呼び出す")]
    public async Task Case487()
    {
        var client = NewMqttModuleClientWithModule();
        using var patch = new SubscribePatch();
        IotMessageHandler handler = (msg, obj) => Task.FromResult(MessageResponse.Completed);
        var userContext = new object();

        await client.SetInputMessageHandlerAsync("input1", handler, userContext, TransportTopic.Mqtt);

        var call = Assert.Single(patch.Calls);
        Assert.Equal(new[] { "input1/#" }, call.Topics);
        Assert.Equal(new byte[] { 1 }, call.QosLevels);
        Assert.Equal(new[] { "input1" }, GetInputNames(client));
        Assert.True(GetDictCallbackProps(client).ContainsKey("input1"));
    }

    [Fact(DisplayName = "No488_正常系：正常動作の確認_TransportTopic.Mqttを指定し、inputNameに#を含めて、メソッドを呼び出す")]
    public async Task Case488()
    {
        var client = NewMqttModuleClientWithModule();
        using var patch = new SubscribePatch();
        IotMessageHandler handler = (msg, obj) => Task.FromResult(MessageResponse.Completed);
        var userContext = new object();

        await client.SetInputMessageHandlerAsync("input1/#", handler, userContext, TransportTopic.Mqtt);

        var call = Assert.Single(patch.Calls);
        Assert.Equal(new[] { "input1/#" }, call.Topics);
        Assert.Equal(new byte[] { 1 }, call.QosLevels);
    }

    [Fact(DisplayName = "No489_正常系：正常動作の確認_同じinputNameを2回登録する_2回目はTransportTopic.Mqttを指定する")]
    public async Task Case489()
    {
        var client = NewMqttModuleClientWithModule();
        using var patch = new SubscribePatch();
        IotMessageHandler handler1 = (msg, obj) => Task.FromResult(MessageResponse.Completed);
        var userContext1 = new object();
        await client.SetInputMessageHandlerAsync("input1", handler1, userContext1, TransportTopic.Iothub);

        IotMessageHandler handler2 = (msg, obj) => Task.FromResult(MessageResponse.Completed);
        var userContext2 = new object();
        await client.SetInputMessageHandlerAsync("input1", handler2, userContext2, TransportTopic.Mqtt);

        Assert.Equal(new[] { "input1" }, GetInputNames(client));
        Assert.Single(GetDictCallbackProps(client));
        Assert.Empty(patch.Calls);
    }

    [Fact(DisplayName = "No490_正常系：正常動作の確認_登録した入力名に対応するMQTTメッセージを受信し、ハンドラーを実行する")]
    public async Task Case490()
    {
        var client = NewMqttModuleClientWithModule();
        var received = new List<(IotMessage Message, object Context)>();
        Task<MessageResponse> Handler(IotMessage message, object context)
        {
            received.Add((message, context));
            return Task.FromResult(MessageResponse.Completed);
        }
        var userContext = new object();
        await client.SetInputMessageHandlerAsync("myinput", Handler, userContext, TransportTopic.Iothub);

        var payload = Encoding.UTF8.GetBytes("hello");
        InvokePublishReceived(client, "devices/dev1/modules/mod1/myinput/", payload);

        var result = Assert.Single(received);
        Assert.Same(userContext, result.Context);
        Assert.Equal(payload, result.Message.GetBytes());
    }

    [Fact(DisplayName = "No491_正常系：正常動作の確認_MQTT入力名をinput/topicとして登録し、input/topic/childのメッセージを受信する")]
    public async Task Case491()
    {
        var client = NewMqttModuleClientWithModule();
        var received = new List<(IotMessage Message, object Context)>();
        Task<MessageResponse> Handler(IotMessage message, object context)
        {
            received.Add((message, context));
            return Task.FromResult(MessageResponse.Completed);
        }
        var userContext = new object();
        await client.SetInputMessageHandlerAsync("input/topic", Handler, userContext, TransportTopic.Mqtt);

        var payload = Encoding.UTF8.GetBytes("hello");
        InvokePublishReceived(client, "input/topic/child", payload);

        var result = Assert.Single(received);
        Assert.Same(userContext, result.Context);
    }

    [Fact(DisplayName = "No492_正常系：正常動作の確認_MQTTのワイルドカード入力名#を登録し、未登録のMQTTトピックのメッセージを受信する")]
    public async Task Case492()
    {
        var client = NewMqttModuleClientWithModule();
        var received = new List<(IotMessage Message, object Context)>();
        Task<MessageResponse> Handler(IotMessage message, object context)
        {
            received.Add((message, context));
            return Task.FromResult(MessageResponse.Completed);
        }
        var userContext = new object();
        await client.SetInputMessageHandlerAsync("#", Handler, userContext, TransportTopic.Mqtt);

        var payload = Encoding.UTF8.GetBytes("hello");
        InvokePublishReceived(client, "some/random/topic", payload);

        var result = Assert.Single(received);
        Assert.Same(userContext, result.Context);
    }

    [Fact(DisplayName = "No493_異常系：異常動作の確認_inputNameにnull、TransportTopic.Iothubを指定して登録する")]
    public async Task Case493()
    {
        var client = NewMqttModuleClientWithModule();
        IotMessageHandler handler = (msg, obj) => Task.FromResult(MessageResponse.Completed);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => client.SetInputMessageHandlerAsync(null!, handler, new object(), TransportTopic.Iothub));
    }

    [Fact(DisplayName = "No494_異常系：異常動作の確認_inputNameにnull、TransportTopic.Mqttを指定して登録する")]
    public async Task Case494()
    {
        var client = NewMqttModuleClientWithModule();
        IotMessageHandler handler = (msg, obj) => Task.FromResult(MessageResponse.Completed);

        await Assert.ThrowsAsync<NullReferenceException>(
            () => client.SetInputMessageHandlerAsync(null!, handler, new object(), TransportTopic.Mqtt));
    }

    [Fact(DisplayName = "No495_異常系：異常動作の確認_iotHandlerにnullを指定して登録した後、登録入力に一致するメッセージを受信する")]
    public async Task Case495()
    {
        var client = NewMqttModuleClientWithModule();
        await client.SetInputMessageHandlerAsync("myinput", null!, new object(), TransportTopic.Iothub);

        var payload = Encoding.UTF8.GetBytes("hello");
        Assert.Throws<NullReferenceException>(
            () => InvokePublishReceived(client, "devices/dev1/modules/mod1/myinput/", payload));
    }

    [Fact(DisplayName = "No496_正常系：正常動作の確認_userContextにnullを指定して登録し、登録入力に一致するメッセージを受信する")]
    public async Task Case496()
    {
        var client = NewMqttModuleClientWithModule();
        var received = new List<(IotMessage Message, object? Context)>();
        Task<MessageResponse> Handler(IotMessage message, object context)
        {
            received.Add((message, context));
            return Task.FromResult(MessageResponse.Completed);
        }
        await client.SetInputMessageHandlerAsync("myinput", Handler, null!, TransportTopic.Iothub);

        var payload = Encoding.UTF8.GetBytes("hello");
        InvokePublishReceived(client, "devices/dev1/modules/mod1/myinput/", payload);

        var result = Assert.Single(received);
        Assert.Null(result.Context);
    }

    private static MqttModuleClient NewMqttModuleClientWithModule(TransportTopic defaultReceiveTopic = TransportTopic.Iothub)
    {
        using var env = new EnvironmentVariablesScope(
            ("IOTEDGE_DEVICEID", "dev1"),
            ("IOTEDGE_MODULEID", "mod1"));

        return new MqttModuleClient("sas-token", "localhost", null, TransportTopic.Iothub, defaultReceiveTopic);
    }

    private static List<string> GetInputNames(MqttModuleClient client)
    {
        return (List<string>)typeof(MqttModuleClient).GetField("inputNames", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(client)!;
    }

    private static Dictionary<string, CallbackProperties> GetDictCallbackProps(MqttModuleClient client)
    {
        return (Dictionary<string, CallbackProperties>)typeof(MqttModuleClient).GetField("dictCallbackProps", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(client)!;
    }

    private static void InvokePublishReceived(MqttModuleClient client, string topic, byte[] payload)
    {
        var method = typeof(MqttModuleClient).GetMethod("Client_MqttMsgPublishReceived", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var args = Activator.CreateInstance(typeof(MqttMsgPublishEventArgs), topic, payload, false, (byte)1, false);

        try
        {
            method.Invoke(client, new object?[] { null, args });
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
        }
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

    private sealed class SetInputMessageHandlerAsyncPatch : IDisposable
    {
        private readonly Harmony harmony;

        public List<SetInputMessageHandlerCall> Calls { get; } = new();

        private static SetInputMessageHandlerAsyncPatch? current;

        public SetInputMessageHandlerAsyncPatch()
        {
            current = this;

            harmony = new Harmony($"{nameof(MqttModuleClient_SetInputMessageHandlerAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(MqttModuleClient), nameof(MqttModuleClient.SetInputMessageHandlerAsync), new[] { typeof(string), typeof(IotMessageHandler), typeof(object), typeof(TransportTopic) }),
                prefix: new HarmonyMethod(typeof(SetInputMessageHandlerAsyncPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
            current = null;
        }

        private static bool Prefix(string __0, IotMessageHandler __1, object __2, TransportTopic __3, ref Task __result)
        {
            current?.Calls.Add(new SetInputMessageHandlerCall(__0, __1, __2, __3));
            __result = Task.CompletedTask;
            return false;
        }
    }

    private sealed record SetInputMessageHandlerCall(string InputName, IotMessageHandler IotHandler, object UserContext, TransportTopic TransportTopic);

    private sealed class SubscribePatch : IDisposable
    {
        private readonly Harmony harmony;

        public List<SubscribeCall> Calls { get; } = new();

        private static SubscribePatch? current;

        public SubscribePatch()
        {
            current = this;

            harmony = new Harmony($"{nameof(MqttModuleClient_SetInputMessageHandlerAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(MqttClient), nameof(MqttClient.Subscribe), new[] { typeof(string[]), typeof(byte[]) }),
                prefix: new HarmonyMethod(typeof(SubscribePatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
            current = null;
        }

        private static bool Prefix(string[] __0, byte[] __1, ref ushort __result)
        {
            current?.Calls.Add(new SubscribeCall(__0, __1));
            __result = 0;
            return false;
        }
    }

    private sealed record SubscribeCall(string[] Topics, byte[] QosLevels);
}
