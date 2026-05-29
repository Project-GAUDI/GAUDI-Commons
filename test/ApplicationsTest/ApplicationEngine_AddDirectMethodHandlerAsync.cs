using Xunit;
using Xunit.Abstractions;
using Moq;
using System.Collections.Generic;
using System;
using Microsoft.Azure.Devices.Client;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_AddDirectMethodHandlerAsync))]
public class ApplicationEngine_AddDirectMethodHandlerAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_AddDirectMethodHandlerAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No032_引数のmethodNameが文字列の場合、かつ、繰り返し処理をした場合に異なる文字列を指定した場合")]
    public async void Applications_Case032()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        await applicationEngine.AddDirectMethodHandlerAsync("outputName", null, null);
        await applicationEngine.AddDirectMethodHandlerAsync("outputName2", null, null);
        moduleClientMock.Verify(x => x.SetMethodHandlerAsync(It.IsAny<string>(), It.IsAny<MethodCallback>(), It.IsAny<object>()), Times.AtLeastOnce);

        var methodRequestEventDataField = applicationEngine.GetType().GetField("methodRequestEventData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var methodRequestEventData = (Dictionary<string, Commons.MethodEventData>?)methodRequestEventDataField!.GetValue(applicationEngine);
        Assert.True(methodRequestEventData!.Count > 0);
    }

    [Fact(DisplayName = "Applications No033_引数のmethodNameが文字列の場合に同じ文字列を指定して繰り返し追加した場合")]
    public async void Applications_Case033()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        await applicationEngine.AddDirectMethodHandlerAsync("outputName", null, null);
        await Assert.ThrowsAsync<ArgumentException>(async () => await applicationEngine.AddDirectMethodHandlerAsync("outputName", null, null));
    }

    [Fact(DisplayName = "Applications No034_引数のmethodNameがnullの場合")]
    public async void Applications_Case034()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await applicationEngine.AddDirectMethodHandlerAsync(null, null, null));
    }

    [Fact(DisplayName = "Applications No035_引数のmethodNameが空文字の場合に空文字を指定して繰り返し追加した場合")]
    public async void Applications_Case035()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var moduleClient = (IModuleClient?)moduleClientInfo!.GetValue(applicationEngine);
        moduleClientInfo.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        await applicationEngine.AddDirectMethodHandlerAsync("", null, null);
        await Assert.ThrowsAsync<ArgumentException>(async () => await applicationEngine.AddDirectMethodHandlerAsync("", null, null));
    }
}
