using System.Reflection;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngine_DisposeAsync
{
    [Fact(DisplayName = "No642_正常系：正常動作の確認_applicationMainとMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case642()
    {
        var engine = new ApplicationEngine();
        var app = NewApplicationMain();
        var client = new Mock<IModuleClient>();
        client.Setup(c => c.CloseAsync()).Returns(Task.CompletedTask);
        SetField(engine, "applicationMain", app.Object);
        SetField(engine, "MyModuleClient", client.Object);
        AddEventData(engine);

        await engine.DisposeAsync();

        app.Verify(a => a.TerminateAsync(), Times.Once);
        client.Verify(c => c.CloseAsync(), Times.Once);
        client.Verify(c => c.Dispose(), Times.Once);
        Assert.Null(GetField(engine, "applicationMain"));
        Assert.Null(GetField(engine, "MyModuleClient"));
        Assert.Empty(GetMessageInputEventData(engine));
        Assert.Empty(GetMethodRequestEventData(engine));
    }

    [Fact(DisplayName = "No643_正常系：正常動作の確認_applicationMainとMyModuleClientをnullにして、メソッドを呼び出す")]
    public async Task Case643()
    {
        var engine = new ApplicationEngine();
        SetField(engine, "applicationMain", null);
        SetField(engine, "MyModuleClient", null);
        AddEventData(engine);

        var exception = await Record.ExceptionAsync(async () => await engine.DisposeAsync());

        Assert.Null(exception);
        Assert.Empty(GetMessageInputEventData(engine));
        Assert.Empty(GetMethodRequestEventData(engine));
    }

    [Fact(DisplayName = "No644_正常系：正常動作の確認_メソッドを2回呼び出す")]
    public async Task Case644()
    {
        var engine = new ApplicationEngine();
        var app = NewApplicationMain();
        var client = new Mock<IModuleClient>();
        client.Setup(c => c.CloseAsync()).Returns(Task.CompletedTask);
        SetField(engine, "applicationMain", app.Object);
        SetField(engine, "MyModuleClient", client.Object);
        await engine.DisposeAsync();

        SetField(engine, "applicationMain", app.Object);
        SetField(engine, "MyModuleClient", client.Object);
        AddEventData(engine);

        var exception = await Record.ExceptionAsync(async () => await engine.DisposeAsync());

        Assert.Null(exception);
        app.Verify(a => a.TerminateAsync(), Times.Once);
        client.Verify(c => c.CloseAsync(), Times.Once);
        client.Verify(c => c.Dispose(), Times.Once);
        Assert.Single(GetMessageInputEventData(engine));
        Assert.Single(GetMethodRequestEventData(engine));
    }

    [Fact(DisplayName = "No645_異常系：異常動作の確認_TerminateAsyncが例外を送出するapplicationMainを設定して、メソッドを呼び出す")]
    public async Task Case645()
    {
        var engine = new ApplicationEngine();
        var app = new Mock<IApplicationMain>();
        app.Setup(a => a.TerminateAsync()).ThrowsAsync(new InvalidOperationException("terminate error"));
        var client = new Mock<IModuleClient>();
        client.Setup(c => c.CloseAsync()).Returns(Task.CompletedTask);
        SetField(engine, "applicationMain", app.Object);
        SetField(engine, "MyModuleClient", client.Object);
        AddEventData(engine);

        var exception = await Record.ExceptionAsync(async () => await engine.DisposeAsync());

        Assert.Null(exception);
        Assert.Null(GetField(engine, "applicationMain"));
        client.Verify(c => c.CloseAsync(), Times.Once);
        client.Verify(c => c.Dispose(), Times.Once);
        Assert.Null(GetField(engine, "MyModuleClient"));
        Assert.Empty(GetMessageInputEventData(engine));
        Assert.Empty(GetMethodRequestEventData(engine));
    }

    [Fact(DisplayName = "No646_異常系：異常動作の確認_CloseAsyncが例外を送出するMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case646()
    {
        var engine = new ApplicationEngine();
        var client = new Mock<IModuleClient>();
        client.Setup(c => c.CloseAsync()).ThrowsAsync(new InvalidOperationException("close error"));
        SetField(engine, "MyModuleClient", client.Object);

        var exception = await Record.ExceptionAsync(async () => await engine.DisposeAsync());

        Assert.Null(exception);
        client.Verify(c => c.Dispose(), Times.Once);
        Assert.Null(GetField(engine, "MyModuleClient"));
    }

    [Fact(DisplayName = "No647_異常系：異常動作の確認_Disposeが例外を送出するMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case647()
    {
        var engine = new ApplicationEngine();
        var client = new Mock<IModuleClient>();
        client.Setup(c => c.CloseAsync()).Returns(Task.CompletedTask);
        client.Setup(c => c.Dispose()).Throws(new InvalidOperationException("dispose error"));
        SetField(engine, "MyModuleClient", client.Object);

        var exception = await Record.ExceptionAsync(async () => await engine.DisposeAsync());

        Assert.Null(exception);
        client.Verify(c => c.Dispose(), Times.Once);
        Assert.Null(GetField(engine, "MyModuleClient"));
    }

    [Fact(DisplayName = "No648_異常系：異常動作の確認_リフレクションでmessageInputEventDataをnullにして、メソッドを呼び出す")]
    public async Task Case648()
    {
        var engine = new ApplicationEngine();
        SetField(engine, "messageInputEventData", null);

        var exception = await Record.ExceptionAsync(async () => await engine.DisposeAsync());

        Assert.Null(exception);
    }

    private static Mock<IApplicationMain> NewApplicationMain()
    {
        var app = new Mock<IApplicationMain>();
        app.Setup(a => a.TerminateAsync()).ReturnsAsync(true);
        return app;
    }

    private static void AddEventData(ApplicationEngine engine)
    {
        GetMessageInputEventData(engine)["input1"] = new MessageEventData("input1", null!, null!);
        GetMethodRequestEventData(engine)["method1"] = new MethodEventData("method1", null!, null!);
    }

    private static Dictionary<string, MessageEventData> GetMessageInputEventData(ApplicationEngine engine)
    {
        return (Dictionary<string, MessageEventData>)GetField(engine, "messageInputEventData")!;
    }

    private static Dictionary<string, MethodEventData> GetMethodRequestEventData(ApplicationEngine engine)
    {
        return (Dictionary<string, MethodEventData>)GetField(engine, "methodRequestEventData")!;
    }

    private static object? GetField(ApplicationEngine engine, string name)
    {
        return typeof(ApplicationEngine).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(engine);
    }

    private static void SetField(ApplicationEngine engine, string name, object? value)
    {
        typeof(ApplicationEngine).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(engine, value);
    }
}
