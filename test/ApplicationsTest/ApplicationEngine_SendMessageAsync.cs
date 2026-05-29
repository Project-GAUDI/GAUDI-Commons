using Xunit;
using Xunit.Abstractions;
using System.Threading.Tasks;
using Moq;
using System;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_SendMessageAsync))]
public class ApplicationEngine_SendMessageAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_SendMessageAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No023_IotConnectionStatus.Connectedの状態で呼び出し")]
    public async void Applications_Case023()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Connected);

        await applicationEngine.SendMessageAsync("outputName", null);

        moduleClientMock.Verify(x => x.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Once);
    }

    [Fact(DisplayName = "Applications No024_引数のoutputNameが文字列の場合")]
    public async void Applications_Case024()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Connected);

        await applicationEngine.SendMessageAsync("outputName", null);

        moduleClientMock.Verify(x => x.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Once);
    }

    [Fact(DisplayName = "Applications No025_引数のoutputNameがnullの場合")]
    public async void Applications_Case025()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Connected);

        await applicationEngine.SendMessageAsync(null, null);

        moduleClientMock.Verify(x => x.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Once);
    }

    [Fact(DisplayName = "Applications No026_引数のoutputNameが空文字の場合")]
    public async void Applications_Case026()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Connected);

        await applicationEngine.SendMessageAsync("", null);

        moduleClientMock.Verify(x => x.SendEventAsync(It.IsAny<string>(), It.IsAny<IotMessage>()), Times.Once);
    }

    [Fact(DisplayName = "Applications No027_IotConnectionStatus.Disabledの状態で呼び出し")]
    public async void Applications_Case027()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Disabled);

        await Assert.ThrowsAsync<Exception>(async () => await applicationEngine.SendMessageAsync("outputName", null));
    }

    [Fact(DisplayName = "Applications No028_IotConnectionStatus.Disconnectedの状態で呼び出し")]
    public async void Applications_Case028()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(IotConnectionStatus.Disconnected);

        await Assert.ThrowsAsync<Exception>(async () => await applicationEngine.SendMessageAsync("outputName", null));
    }

    [Fact(DisplayName = "Applications No029_IotConnectionStatus.Disconnected_Retryingの状態で呼び出し")]
    public async void Applications_Case029()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        var connectionStatus = IotConnectionStatus.Disconnected_Retrying;

        moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(() => connectionStatus);

        var changeStatusTask = Task.Run(async () =>
        {
            await Task.Delay(3000);
            connectionStatus = IotConnectionStatus.Connected;
        });

        await applicationEngine.SendMessageAsync("outputName", null);
        moduleClientMock.Verify(x => x.ConnectionStatus, Times.AtLeastOnce);
        await changeStatusTask;
        Assert.Equal(IotConnectionStatus.Connected, connectionStatus);
    }

    [Fact(DisplayName = "Applications No031_上記以外の状態で呼び出し")]
    public async void Applications_Case031()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        var connectionStatus = (IotConnectionStatus)99;

        moduleClientMock.SetupGet(x => x.ConnectionStatus).Returns(() => connectionStatus);

        await Assert.ThrowsAsync<Exception>(async () => await applicationEngine.SendMessageAsync("outputName", null));
    }
}
