using System.Reflection;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngine_SendMessageAsync
{
    [Fact(DisplayName = "No669_正常系：正常動作の確認_outputNameとsendingMsgを指定して、メソッドを呼び出す")]
    public async Task Case669()
    {
        var client = NewClient(IotConnectionStatus.Connected);
        var engine = NewEngine(client.Object);
        var message = new IotMessage();

        // CancellationTokenを渡さないオーバーロードの委譲先を検証するため、あえてトークンを渡さない
#pragma warning disable xUnit1051
        await engine.SendMessageAsync("output1", message);
#pragma warning restore xUnit1051

        client.Verify(c => c.SendEventAsync("output1", message, CancellationToken.None), Times.Once);
    }

    [Fact(DisplayName = "No670_異常系：異常動作の確認_SendEventAsyncが例外を送出するMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case670()
    {
        var client = NewClient(IotConnectionStatus.Connected);
        client.Setup(c => c.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("send error"));
        var engine = NewEngine(client.Object);

#pragma warning disable xUnit1051
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => engine.SendMessageAsync("output1", new IotMessage()));
#pragma warning restore xUnit1051

        Assert.Equal("send error", exception.Message);
    }

    [Fact(DisplayName = "No671_正常系：正常動作の確認_ConnectedのMyModuleClientと未キャンセルのcancellationTokenを指定して、メソッドを呼び出す")]
    public async Task Case671()
    {
        var client = NewClient(IotConnectionStatus.Connected);
        var engine = NewEngine(client.Object);
        var message = new IotMessage();
        using var cts = new CancellationTokenSource();

        await engine.SendMessageAsync("output1", message, cts.Token);

        client.Verify(c => c.SendEventAsync("output1", message, cts.Token), Times.Once);
    }

    [Fact(DisplayName = "No672_正常系：正常動作の確認_初回はDisconnected_Retryingを返し、待機後にConnectedを返すMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case672()
    {
        var client = new Mock<IModuleClient>();
        client.SetupSequence(c => c.ConnectionStatus)
            .Returns(IotConnectionStatus.Disconnected_Retrying)
            .Returns(IotConnectionStatus.Disconnected_Retrying)
            .Returns(IotConnectionStatus.Connected)
            .Returns(IotConnectionStatus.Connected);
        var engine = NewEngine(client.Object);
        var message = new IotMessage();

        await engine.SendMessageAsync("output1", message, TestContext.Current.CancellationToken);

        client.VerifyGet(c => c.ConnectionStatus, Times.Exactly(4));
        client.Verify(c => c.SendEventAsync("output1", message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "No673_異常系：異常動作の確認_DisabledのMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case673()
    {
        var client = NewClient(IotConnectionStatus.Disabled);
        var engine = NewEngine(client.Object);

        await Assert.ThrowsAsync<Exception>(() => engine.SendMessageAsync("output1", new IotMessage(), TestContext.Current.CancellationToken));

        VerifyNotSent(client);
    }

    [Fact(DisplayName = "No674_異常系：異常動作の確認_DisconnectedのMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case674()
    {
        var client = NewClient(IotConnectionStatus.Disconnected);
        var engine = NewEngine(client.Object);

        await Assert.ThrowsAsync<Exception>(() => engine.SendMessageAsync("output1", new IotMessage(), TestContext.Current.CancellationToken));

        VerifyNotSent(client);
    }

    [Fact(DisplayName = "No675_異常系：異常動作の確認_初回はDisconnected_Retryingを返し、待機後にDisconnectedを返すMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case675()
    {
        var client = new Mock<IModuleClient>();
        client.SetupSequence(c => c.ConnectionStatus)
            .Returns(IotConnectionStatus.Disconnected_Retrying)
            .Returns(IotConnectionStatus.Disconnected_Retrying)
            .Returns(IotConnectionStatus.Disconnected)
            .Returns(IotConnectionStatus.Disconnected);
        var engine = NewEngine(client.Object);

        await Assert.ThrowsAsync<Exception>(() => engine.SendMessageAsync("output1", new IotMessage(), TestContext.Current.CancellationToken));

        VerifyNotSent(client);
    }

    [Fact(DisplayName = "No676_異常系：異常動作の確認_Disconnected_RetryingのMyModuleClientとキャンセル済みのcancellationTokenを指定して、メソッドを呼び出す")]
    public async Task Case676()
    {
        var client = NewClient(IotConnectionStatus.Disconnected_Retrying);
        var engine = NewEngine(client.Object);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => engine.SendMessageAsync("output1", new IotMessage(), cts.Token));

        VerifyNotSent(client);
    }

    [Fact(DisplayName = "No677_異常系：異常動作の確認_ConnectionStatusが未定義値のMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case677()
    {
        var client = NewClient((IotConnectionStatus)999);
        var engine = NewEngine(client.Object);

        await Assert.ThrowsAsync<Exception>(() => engine.SendMessageAsync("output1", new IotMessage(), TestContext.Current.CancellationToken));

        VerifyNotSent(client);
    }

    [Fact(DisplayName = "No678_異常系：異常動作の確認_MyModuleClientをnullにして、メソッドを呼び出す")]
    public async Task Case678()
    {
        var engine = NewEngine(null);

        await Assert.ThrowsAsync<NullReferenceException>(() => engine.SendMessageAsync("output1", new IotMessage(), TestContext.Current.CancellationToken));
    }

    [Fact(DisplayName = "No679_異常系：異常動作の確認_SendEventAsyncが例外を送出するMyModuleClientを設定して、メソッドを呼び出す")]
    public async Task Case679()
    {
        var client = NewClient(IotConnectionStatus.Connected);
        client.Setup(c => c.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("send error"));
        var engine = NewEngine(client.Object);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => engine.SendMessageAsync("output1", new IotMessage(), TestContext.Current.CancellationToken));

        Assert.Equal("send error", exception.Message);
    }

    private static Mock<IModuleClient> NewClient(IotConnectionStatus status)
    {
        var client = new Mock<IModuleClient>();
        client.SetupGet(c => c.ConnectionStatus).Returns(status);
        return client;
    }

    private static ApplicationEngine NewEngine(IModuleClient? client)
    {
        var engine = new ApplicationEngine();
        typeof(ApplicationEngine).GetField("MyModuleClient", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(engine, client);
        return engine;
    }

    private static void VerifyNotSent(Mock<IModuleClient> client)
    {
        client.Verify(c => c.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
