using System.Reflection;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class MqttModuleClient_SetDesiredPropertyUpdateCallbackAsync
{
    [Fact(DisplayName = "No478_正常系：正常動作の確認_callback、userContextを指定して、メソッドを呼び出す")]
    public async Task Case478()
    {
        var client = NewMqttModuleClient();
        Task Callback(TwinCollection desiredProperties, object userContext) => Task.CompletedTask;
        var userContext = new object();

        await client.SetDesiredPropertyUpdateCallbackAsync(Callback, userContext);

        Assert.Equal((DesiredPropertyUpdateCallback)Callback, GetPatchCallback(client));
        Assert.Same(userContext, GetPatchUserContext(client));
    }

    [Fact(DisplayName = "No479_正常系：正常動作の確認_callback、userContext、TransportTopic.Iothubを指定して、メソッドを呼び出す")]
    public async Task Case479()
    {
        var client = NewMqttModuleClient();
        Task Callback(TwinCollection desiredProperties, object userContext) => Task.CompletedTask;
        var userContext = new object();

        await client.SetDesiredPropertyUpdateCallbackAsync(Callback, userContext, TransportTopic.Iothub);

        Assert.Equal((DesiredPropertyUpdateCallback)Callback, GetPatchCallback(client));
        Assert.Same(userContext, GetPatchUserContext(client));
    }

    [Fact(DisplayName = "No480_正常系：正常動作の確認_callback、userContext、TransportTopic.Mqttを指定して、メソッドを呼び出す")]
    public async Task Case480()
    {
        var client = NewMqttModuleClient();
        Task Callback(TwinCollection desiredProperties, object userContext) => Task.CompletedTask;
        var userContext = new object();

        await client.SetDesiredPropertyUpdateCallbackAsync(Callback, userContext, TransportTopic.Mqtt);

        Assert.Equal((DesiredPropertyUpdateCallback)Callback, GetPatchCallback(client));
        Assert.Same(userContext, GetPatchUserContext(client));
    }

    [Fact(DisplayName = "No481_正常系：正常動作の確認_callbackにnullを指定して、メソッドを呼び出す")]
    public async Task Case481()
    {
        var client = NewMqttModuleClient();
        var userContext = new object();

        await client.SetDesiredPropertyUpdateCallbackAsync(null!, userContext);

        Assert.Null(GetPatchCallback(client));
        Assert.Same(userContext, GetPatchUserContext(client));
    }

    [Fact(DisplayName = "No482_正常系：正常動作の確認_userContextにnullを指定して、メソッドを呼び出す")]
    public async Task Case482()
    {
        var client = NewMqttModuleClient();
        Task Callback(TwinCollection desiredProperties, object userContext) => Task.CompletedTask;

        await client.SetDesiredPropertyUpdateCallbackAsync(Callback, null!);

        Assert.Equal((DesiredPropertyUpdateCallback)Callback, GetPatchCallback(client));
        Assert.Null(GetPatchUserContext(client));
    }

    [Fact(DisplayName = "No483_正常系：正常動作の確認_既にコールバックとコンテキストが設定された状態で、別のコールバックとコンテキストを指定して、メソッドを呼び出す")]
    public async Task Case483()
    {
        var client = NewMqttModuleClient();
        Task FirstCallback(TwinCollection desiredProperties, object userContext) => Task.CompletedTask;
        var firstUserContext = new object();
        await client.SetDesiredPropertyUpdateCallbackAsync(FirstCallback, firstUserContext);

        Task SecondCallback(TwinCollection desiredProperties, object userContext) => Task.CompletedTask;
        var secondUserContext = new object();

        await client.SetDesiredPropertyUpdateCallbackAsync(SecondCallback, secondUserContext);

        Assert.Equal((DesiredPropertyUpdateCallback)SecondCallback, GetPatchCallback(client));
        Assert.Same(secondUserContext, GetPatchUserContext(client));
    }

    private static MqttModuleClient NewMqttModuleClient()
    {
        return new MqttModuleClient("sas-token", "localhost", "device1");
    }

    private static DesiredPropertyUpdateCallback? GetPatchCallback(MqttModuleClient client)
    {
        var field = typeof(MqttModuleClient).GetField("patchCallback", BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (DesiredPropertyUpdateCallback?)field.GetValue(client);
    }

    private static object? GetPatchUserContext(MqttModuleClient client)
    {
        var field = typeof(MqttModuleClient).GetField("patchUserContext", BindingFlags.Instance | BindingFlags.NonPublic)!;
        return field.GetValue(client);
    }
}
