using System.Reflection;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngine_AddMessageInputHandlerAsync
{
    [Fact(DisplayName = "No665_正常系：正常動作の確認_inputName、msgHandler、userContextを指定して、メソッドを呼び出す")]
    public async Task Case665()
    {
        var engine = new ApplicationEngine();
        var client = new Mock<IModuleClient>();
        IotMessageHandler? registered = null;
        client.Setup(c => c.SetInputMessageHandlerAsync(It.IsAny<string>(), It.IsAny<IotMessageHandler>(), It.IsAny<object>()))
            .Callback<string, IotMessageHandler, object>((_, handler, _) => registered = handler)
            .Returns(Task.CompletedTask);
        SetField(engine, "MyModuleClient", client.Object);
        MessageEventHandler msgHandler = (name, msg, ctx) => Task.FromResult(true);
        var userContext = new object();

        await engine.AddMessageInputHandlerAsync("input1", msgHandler, userContext);

        var eventData = Assert.Single(GetMessageInputEventData(engine));
        Assert.Equal("input1", eventData.Key);
        Assert.Equal("input1", eventData.Value.InputName);
        Assert.Same(userContext, eventData.Value.UserContext);
        Assert.Same(msgHandler, eventData.Value.MsgHandler);
        client.Verify(c => c.SetInputMessageHandlerAsync("input1", It.IsAny<IotMessageHandler>(), "input1"), Times.Once);
        Assert.NotNull(registered);
        Assert.Equal(nameof(ApplicationEngine.ReceiveMessageAsync), registered.Method.Name);
        Assert.Same(engine, registered.Target);
    }

    [Fact(DisplayName = "No666_正常系：正常動作の確認_userContextを省略して、メソッドを呼び出す")]
    public async Task Case666()
    {
        var engine = new ApplicationEngine();
        var client = new Mock<IModuleClient>();
        SetField(engine, "MyModuleClient", client.Object);
        MessageEventHandler msgHandler = (name, msg, ctx) => Task.FromResult(true);

        await engine.AddMessageInputHandlerAsync("input1", msgHandler);

        var eventData = Assert.Single(GetMessageInputEventData(engine));
        Assert.Null(eventData.Value.UserContext);
        Assert.Same(msgHandler, eventData.Value.MsgHandler);
        client.Verify(c => c.SetInputMessageHandlerAsync("input1", It.IsAny<IotMessageHandler>(), "input1"), Times.Once);
    }

    [Fact(DisplayName = "No667_異常系：異常動作の確認_同じinputNameを指定してメソッドを2回呼び出す")]
    public async Task Case667()
    {
        var engine = new ApplicationEngine();
        SetField(engine, "MyModuleClient", new Mock<IModuleClient>().Object);
        MessageEventHandler msgHandler = (name, msg, ctx) => Task.FromResult(true);
        await engine.AddMessageInputHandlerAsync("input1", msgHandler);

        await Assert.ThrowsAsync<ArgumentException>(() => engine.AddMessageInputHandlerAsync("input1", msgHandler));
    }

    [Fact(DisplayName = "No668_異常系：異常動作の確認_SetInputMessageHandlerAsyncが例外を送出するMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case668()
    {
        var engine = new ApplicationEngine();
        var client = new Mock<IModuleClient>();
        client.Setup(c => c.SetInputMessageHandlerAsync(It.IsAny<string>(), It.IsAny<IotMessageHandler>(), It.IsAny<object>()))
            .ThrowsAsync(new InvalidOperationException("set handler error"));
        SetField(engine, "MyModuleClient", client.Object);
        MessageEventHandler msgHandler = (name, msg, ctx) => Task.FromResult(true);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => engine.AddMessageInputHandlerAsync("input1", msgHandler));

        Assert.Equal("set handler error", exception.Message);
    }

    private static Dictionary<string, MessageEventData> GetMessageInputEventData(ApplicationEngine engine)
    {
        return (Dictionary<string, MessageEventData>)typeof(ApplicationEngine)
            .GetField("messageInputEventData", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
    }

    private static void SetField(ApplicationEngine engine, string name, object? value)
    {
        typeof(ApplicationEngine).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(engine, value);
    }
}
