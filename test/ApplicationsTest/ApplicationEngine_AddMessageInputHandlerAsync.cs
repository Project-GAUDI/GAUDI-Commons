using Xunit;
using Xunit.Abstractions;
using Moq;
using System.Collections.Generic;
using System;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_AddMessageInputHandlerAsync))]
public class ApplicationEngine_AddMessageInputHandlerAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_AddMessageInputHandlerAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No019_引数のinputNameが文字列の場合、かつ、繰り返し処理をした場合に異なる文字列を指定した場合")]
    public async void Applications_Case019()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        await applicationEngine.AddMessageInputHandlerAsync("inputName", null, null);
        await applicationEngine.AddMessageInputHandlerAsync("inputName2", null, null);
        moduleClientMock.Verify(x => x.SetInputMessageHandlerAsync(It.IsAny<string>(), It.IsAny<IotMessageHandler>(), It.IsAny<object>()), Times.AtLeastOnce);

        var messageInputEventDataField = applicationEngine.GetType().GetField("messageInputEventData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var messageInputEventData = (Dictionary<string, Commons.MessageEventData>?)messageInputEventDataField!.GetValue(applicationEngine);
        Assert.True(messageInputEventData!.Count > 0);
    }

    [Fact(DisplayName = "Applications No020_引数のinputNameが文字列の場合に同じ文字列を指定して繰り返し追加した場合")]
    public async void Applications_Case020()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        await applicationEngine.AddMessageInputHandlerAsync("inputName", null, null);
        await Assert.ThrowsAsync<ArgumentException>(async () => await applicationEngine.AddMessageInputHandlerAsync("inputName", null, null));
    }

    [Fact(DisplayName = "Applications No021_引数のinputNameがnullの場合")]
    public async void Applications_Case021()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await applicationEngine.AddMessageInputHandlerAsync(null, null, null));
    }

    [Fact(DisplayName = "Applications No022_引数のinputNameが空文字の場合に空文字を指定して繰り返し追加した場合")]
    public async void Applications_Case022()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        await applicationEngine.AddMessageInputHandlerAsync("", null, null);
        await Assert.ThrowsAsync<ArgumentException>(async () => await applicationEngine.AddMessageInputHandlerAsync("", null, null));
    }
}
