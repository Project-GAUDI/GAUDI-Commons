using System.Reflection;
using Microsoft.Azure.Devices.Client;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngine_AddDirectMethodHandlerAsync
{
    [Fact(DisplayName = "No680_正常系：正常動作の確認_methodName、methodHandler、userContextを指定して、メソッドを呼び出す")]
    public async Task Case680()
    {
        var engine = new ApplicationEngine();
        var client = new Mock<IModuleClient>();
        MethodCallback? registered = null;
        client.Setup(c => c.SetMethodHandlerAsync(It.IsAny<string>(), It.IsAny<MethodCallback>(), It.IsAny<object>()))
            .Callback<string, MethodCallback, object>((_, handler, _) => registered = handler)
            .Returns(Task.CompletedTask);
        SetField(engine, "MyModuleClient", client.Object);
        DirectMethodHandler methodHandler = (name, req, ctx) => Task.FromResult(new DirectMethodResponse());
        var userContext = new object();

        await engine.AddDirectMethodHandlerAsync("method1", methodHandler, userContext);

        var eventData = Assert.Single(GetMethodRequestEventData(engine));
        Assert.Equal("method1", eventData.Key);
        Assert.Equal("method1", eventData.Value.MethodName);
        Assert.Same(userContext, eventData.Value.UserContext);
        Assert.Same(methodHandler, eventData.Value.MethodHandler);
        client.Verify(c => c.SetMethodHandlerAsync("method1", It.IsAny<MethodCallback>(), "method1"), Times.Once);
        Assert.NotNull(registered);
        Assert.Equal("ReceiveMethodAsync", registered.Method.Name);
        Assert.Same(engine, registered.Target);
    }

    [Fact(DisplayName = "No681_正常系：正常動作の確認_userContextを省略して、メソッドを呼び出す")]
    public async Task Case681()
    {
        var engine = new ApplicationEngine();
        var client = new Mock<IModuleClient>();
        SetField(engine, "MyModuleClient", client.Object);
        DirectMethodHandler methodHandler = (name, req, ctx) => Task.FromResult(new DirectMethodResponse());

        await engine.AddDirectMethodHandlerAsync("method1", methodHandler);

        var eventData = Assert.Single(GetMethodRequestEventData(engine));
        Assert.Null(eventData.Value.UserContext);
        Assert.Same(methodHandler, eventData.Value.MethodHandler);
        client.Verify(c => c.SetMethodHandlerAsync("method1", It.IsAny<MethodCallback>(), "method1"), Times.Once);
    }

    [Fact(DisplayName = "No682_異常系：異常動作の確認_同じmethodNameを指定してメソッドを2回呼び出す")]
    public async Task Case682()
    {
        var engine = new ApplicationEngine();
        SetField(engine, "MyModuleClient", new Mock<IModuleClient>().Object);
        DirectMethodHandler methodHandler = (name, req, ctx) => Task.FromResult(new DirectMethodResponse());
        await engine.AddDirectMethodHandlerAsync("method1", methodHandler);

        await Assert.ThrowsAsync<ArgumentException>(() => engine.AddDirectMethodHandlerAsync("method1", methodHandler));
    }

    [Fact(DisplayName = "No683_異常系：異常動作の確認_SetMethodHandlerAsyncが例外を送出するMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case683()
    {
        var engine = new ApplicationEngine();
        var client = new Mock<IModuleClient>();
        client.Setup(c => c.SetMethodHandlerAsync(It.IsAny<string>(), It.IsAny<MethodCallback>(), It.IsAny<object>()))
            .ThrowsAsync(new InvalidOperationException("set handler error"));
        SetField(engine, "MyModuleClient", client.Object);
        DirectMethodHandler methodHandler = (name, req, ctx) => Task.FromResult(new DirectMethodResponse());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => engine.AddDirectMethodHandlerAsync("method1", methodHandler));

        Assert.Equal("set handler error", exception.Message);
    }

    private static Dictionary<string, MethodEventData> GetMethodRequestEventData(ApplicationEngine engine)
    {
        return (Dictionary<string, MethodEventData>)typeof(ApplicationEngine)
            .GetField("methodRequestEventData", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
    }

    private static void SetField(ApplicationEngine engine, string name, object? value)
    {
        typeof(ApplicationEngine).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(engine, value);
    }
}
