using Xunit;
using Xunit.Abstractions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_DisposeAsync))]
public class ApplicationEngine_DisposeAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_DisposeAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No001_SetApplicationを呼び出し")]
    public void Applications_Case001()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        var value = applicationEngine.GetType().GetField("applicationMain", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(applicationEngine);
        Assert.Same(applicationMainMock.Object, value);
    }

    [Fact(DisplayName = "Applications No002_applicationMainがnullの状態で呼び出し")]
    public async Task Applications_Case002()
    {
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(null);
        //エラーが出ないかどうかで判定する
        try
        {
            await applicationEngine.DisposeAsync();
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }
    }

    [Fact(DisplayName = "Applications No003_MyModuleClientがnullの状態で呼び出し")]
    public async Task Applications_Case003()
    {
        ApplicationEngine applicationEngine = new();
        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, null);
        //エラーが出ないかどうかで判定する
        try
        {
            await applicationEngine.DisposeAsync();
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }
    }

    [Fact(DisplayName = "Applications No004_applicationMainがnullではない状態で呼び出し")]
    public async Task Applications_Case004()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.TerminateAsync())
            .ReturnsAsync(true)
            .Verifiable();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        await applicationEngine.DisposeAsync();
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Once);
    }

    [Fact(DisplayName = "Applications No005_MyModuleClientがnullではない状態で呼び出し")]
    public async Task Applications_Case005()
    {
        ApplicationEngine applicationEngine = new();

        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        await applicationEngine.DisposeAsync();

        moduleClientMock.Verify(x => x.CloseAsync(), Times.Once);
        moduleClientMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact(DisplayName = "Applications No006_applicationMainがnullではない状態かつ、MyModuleClientがnullではない状態で呼び出し")]
    public async Task Applications_Case006()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.TerminateAsync())
            .ReturnsAsync(true);

        var moduleClientMock = new Mock<IModuleClient>();
        moduleClientMock
            .Setup(x => x.CloseAsync())
            .Returns(Task.CompletedTask);

        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        
        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);

        // 事前状態確認: 全てのリソースが設定されている
        var applicationMainField = applicationEngine.GetType().GetField("applicationMain", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var messageInputEventDataField = applicationEngine.GetType().GetField("messageInputEventData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var methodRequestEventDataField = applicationEngine.GetType().GetField("methodRequestEventData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // 辞書にテストデータを追加
        var messageInputEventData = (Dictionary<string, Commons.MessageEventData?>?)messageInputEventDataField!.GetValue(applicationEngine);
        messageInputEventData!.Add("TEST", null);
        var methodRequestEventData = (Dictionary<string, Commons.MethodEventData?>?)methodRequestEventDataField!.GetValue(applicationEngine);
        methodRequestEventData!.Add("TEST", null);

        Assert.NotNull(applicationMainField!.GetValue(applicationEngine));
        Assert.NotNull(moduleClientInfo.GetValue(applicationEngine));
        Assert.NotEmpty(messageInputEventData); // テストデータが追加されていることを確認
        Assert.NotEmpty(methodRequestEventData); // テストデータが追加されていることを確認

        await applicationEngine.DisposeAsync();

        // 事後状態確認: 全てのリソースが解放されている
        Assert.Null(applicationMainField.GetValue(applicationEngine));
        Assert.Null(moduleClientInfo.GetValue(applicationEngine));
        
        messageInputEventData = (Dictionary<string, Commons.MessageEventData?>?)messageInputEventDataField!.GetValue(applicationEngine);
        methodRequestEventData = (Dictionary<string, Commons.MethodEventData?>?)methodRequestEventDataField!.GetValue(applicationEngine);
        
        Assert.Empty(messageInputEventData!);
        Assert.Empty(methodRequestEventData!);
    }

    [Fact(DisplayName = "Applications No007_DisposeAsyncを順次実行で複数回呼び出し")]
    public async Task Applications_Case007()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.TerminateAsync())
            .ReturnsAsync(true);

        var moduleClientMock = new Mock<IModuleClient>();
        moduleClientMock
            .Setup(x => x.CloseAsync())
            .Returns(Task.CompletedTask);

        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        
        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);

        // 1回目のDisposeAsync
        await applicationEngine.DisposeAsync();
        
        // 2回目のDisposeAsync
        await applicationEngine.DisposeAsync();
        
        // 3回目のDisposeAsync
        await applicationEngine.DisposeAsync();

        // 各メソッドが1回のみ呼び出されることを確認
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Once);
        moduleClientMock.Verify(x => x.CloseAsync(), Times.Once);
        moduleClientMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact(DisplayName = "Applications No008_DisposeAsyncを同時実行で複数回呼び出し")]
    public async Task Applications_Case008()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        var terminateCallCount = 0;
        applicationMainMock
            .Setup(x => x.TerminateAsync())
            .Returns(async () =>
            {
                Interlocked.Increment(ref terminateCallCount);
                await Task.Delay(100);
                return true;
            });

        var moduleClientMock = new Mock<IModuleClient>();
        moduleClientMock
            .Setup(x => x.CloseAsync())
            .Returns(Task.CompletedTask);

        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);

        // 複数のタスクで同時にDisposeAsyncを実行
        var task1 = applicationEngine.DisposeAsync();
        var task2 = applicationEngine.DisposeAsync();
        var task3 = applicationEngine.DisposeAsync();

        await Task.WhenAll(task1.AsTask(), task2.AsTask(), task3.AsTask());

        // 各メソッドが1回のみ呼び出されることを確認
        Assert.Equal(1, terminateCallCount);
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Once);
        moduleClientMock.Verify(x => x.CloseAsync(), Times.Once);
        moduleClientMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact(DisplayName = "Applications No009_disposed状態確認")]
    public async Task Applications_Case009()
    {
        ApplicationEngine applicationEngine = new();
        
        // 初期状態では_disposedが0であることを確認
        var disposedField = applicationEngine.GetType().GetField("_disposed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.Equal(0, disposedField!.GetValue(applicationEngine));

        // 1回目のDisposeAsync
        await applicationEngine.DisposeAsync();
        
        // _disposedに1が設定されることを確認
        Assert.Equal(1, disposedField.GetValue(applicationEngine));

        // 2回目のDisposeAsync
        await applicationEngine.DisposeAsync();
        
        // _disposedが1のままであることを確認
        Assert.Equal(1, disposedField.GetValue(applicationEngine));
    }

    [Fact(DisplayName = "Applications No010_applicationMain.TerminateAsyncで例外が発生")]
    public async Task Applications_Case010()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.TerminateAsync())
            .ThrowsAsync(new InvalidOperationException("TerminateAsync failed"));

        var moduleClientMock = new Mock<IModuleClient>();
        moduleClientMock
            .Setup(x => x.CloseAsync())
            .Returns(Task.CompletedTask);
        moduleClientMock
            .Setup(x => x.Dispose())
            .Verifiable();

        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);

        // TerminateAsyncで例外が発生してもDisposeAsyncが完了することを確認
        await applicationEngine.DisposeAsync();

        // 各メソッドが呼び出されることを確認
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Once);
        moduleClientMock.Verify(x => x.CloseAsync(), Times.Once);
        moduleClientMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact(DisplayName = "Applications No011_ModuleClient.CloseAsyncで例外が発生")]
    public async Task Applications_Case011()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.TerminateAsync())
            .ReturnsAsync(true);

        var moduleClientMock = new Mock<IModuleClient>();
        moduleClientMock
            .Setup(x => x.CloseAsync())
            .ThrowsAsync(new InvalidOperationException("CloseAsync failed"));
        moduleClientMock
            .Setup(x => x.Dispose())
            .Verifiable();

        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);

        // ModuleClient.CloseAsyncで例外が発生してもDisposeAsyncが完了することを確認
        await applicationEngine.DisposeAsync();

        // 各メソッドが呼び出されることを確認
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Once);
        moduleClientMock.Verify(x => x.CloseAsync(), Times.Once);
        moduleClientMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact(DisplayName = "Applications No012_ModuleClient.Disposeで例外が発生")]
    public async Task Applications_Case012()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.TerminateAsync())
            .ReturnsAsync(true);

        var moduleClientMock = new Mock<IModuleClient>();
        moduleClientMock
            .Setup(x => x.CloseAsync())
            .Returns(Task.CompletedTask)
            .Verifiable();
        moduleClientMock
            .Setup(x => x.Dispose())
            .Throws(new InvalidOperationException("Dispose failed"));

        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);

        // ModuleClient.Disposeで例外が発生してもDisposeAsyncが完了することを確認
        await applicationEngine.DisposeAsync();

        // 各メソッドが呼び出されることを確認
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Once);
        moduleClientMock.Verify(x => x.CloseAsync(), Times.Once);
        moduleClientMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact(DisplayName = "Applications No013_applicationMainとModuleClient両方で例外が発生")]
    public async Task Applications_Case013()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.TerminateAsync())
            .ThrowsAsync(new InvalidOperationException("TerminateAsync failed"));

        var moduleClientMock = new Mock<IModuleClient>();
        moduleClientMock
            .Setup(x => x.CloseAsync())
            .ThrowsAsync(new InvalidOperationException("CloseAsync failed"));
        moduleClientMock
            .Setup(x => x.Dispose())
            .Throws(new InvalidOperationException("Dispose failed"));

        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);

        // 複数の例外が発生してもDisposeAsyncが完了することを確認
        await applicationEngine.DisposeAsync();

        // 各メソッドが呼び出されることを確認
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Once);
        moduleClientMock.Verify(x => x.CloseAsync(), Times.Once);
        moduleClientMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact(DisplayName = "Applications No014_非同期処理の完了待機")]
    public async Task Applications_Case014()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        var terminateStarted = false;
        var terminateCompleted = false;
        
        applicationMainMock
            .Setup(x => x.TerminateAsync())
            .Returns(async () =>
            {
                terminateStarted = true;
                await Task.Delay(100); // 長時間処理を想定した待機設定
                terminateCompleted = true;
                return true;
            });

        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        // DisposeAsyncを非同期で開始
        var disposeTask = applicationEngine.DisposeAsync();
        
        // TerminateAsyncが開始されるまで待機
        while (!terminateStarted)
        {
            await Task.Delay(10);
        }
        
        Assert.True(terminateStarted);
        Assert.False(terminateCompleted);

        // DisposeAsyncの完了を待機
        await disposeTask;
        
        // 完了まで待機されたことを確認
        Assert.True(terminateCompleted);
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Once);
    }

    [Fact(DisplayName = "Applications No015_ModuleClientがDispose済みの場合")]
    public async Task Applications_Case015()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.TerminateAsync())
            .ReturnsAsync(true);

        var moduleClientMock = new Mock<IModuleClient>();
        moduleClientMock
            .Setup(x => x.CloseAsync())
            .Returns(Task.CompletedTask);
        moduleClientMock
            .Setup(x => x.Dispose());

        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);

        // 事前にModuleClientのDispose実行
        moduleClientMock.Object.Dispose();

        // DisposeAsync実行
        //エラーにならずに処理が終了すること
        try
        {
            await applicationEngine.DisposeAsync();
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }

        // 各メソッドが呼び出されることを確認
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Once);
        moduleClientMock.Verify(x => x.CloseAsync(), Times.Once);
        moduleClientMock.Verify(x => x.Dispose(), Times.Exactly(2)); // 事前 + DisposeAsyncからの2回呼び出し
    }

    [Fact(DisplayName = "Applications No016_ModuleClientがCloseAsync済みの場合")]
    public async Task Applications_Case016()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.TerminateAsync())
            .ReturnsAsync(true);

        var moduleClientMock = new Mock<IModuleClient>();
        moduleClientMock
            .Setup(x => x.CloseAsync())
            .Returns(Task.CompletedTask);
        moduleClientMock
            .Setup(x => x.Dispose());

        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);

        // 事前にModuleClientのCloseAsync実行
        await moduleClientMock.Object.CloseAsync();

        // DisposeAsync実行
        //エラーにならずに処理が終了すること
        try
        {
            await applicationEngine.DisposeAsync();
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }

        // ApplicationMainのメソッドが呼び出されることを確認
        applicationMainMock.Verify(x => x.TerminateAsync(), Times.Once);
        moduleClientMock.Verify(x => x.CloseAsync(), Times.Exactly(2)); // 事前 + DisposeAsyncからの2回呼び出し
        moduleClientMock.Verify(x => x.Dispose(), Times.Once);
    }
}
