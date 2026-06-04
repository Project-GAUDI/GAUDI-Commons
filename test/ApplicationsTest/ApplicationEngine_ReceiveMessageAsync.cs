using Xunit;
using Xunit.Abstractions;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Moq;
using System.Collections.Generic;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_ReceiveMessageAsync))]
public class ApplicationEngine_ReceiveMessageAsync
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_ReceiveMessageAsync(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No082_ステータスを実行中状態に設定できない場合")]
    public async void Applications_Case082()
    {
        ApplicationEngine applicationEngine = new();
        MessageResponse messageResponse = await applicationEngine.ReceiveMessageAsync(null, null);
        Assert.Equal(MessageResponse.None, messageResponse);
    }

    [Fact(DisplayName = "Applications No084_eventDataがnullではない場合")]
    public async void Applications_Case084()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        var messageInputEventDataField = applicationEngine.GetType().GetField("messageInputEventData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var eventDataDict = (Dictionary<string, Commons.MessageEventData>?)messageInputEventDataField!.GetValue(applicationEngine);
        var inputName = "testInput";

        MessageEventHandler mockMessageHandler = (inputName, receivedMessage, userContext) =>
        {
            return Task.FromResult(true);
        };

        var eventData = new Commons.MessageEventData(inputName, "testUserContext", mockMessageHandler);
        eventDataDict![inputName] = eventData;

        var messageMock = new Mock<IotMessage>();
        var result = await applicationEngine.ReceiveMessageAsync(messageMock.Object, inputName);

        Assert.Equal(MessageResponse.Completed, result);
    }

    [Fact(DisplayName = "Applications No085_eventDataがnullの場合")]
    public async void Applications_Case085()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        var messageInputEventDataField = applicationEngine.GetType().GetField("messageInputEventData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var eventDataDict = (Dictionary<string, Commons.MessageEventData?>?)messageInputEventDataField!.GetValue(applicationEngine);
        var inputName = "testInput";

        eventDataDict![inputName] = null;

        var messageMock = new Mock<IotMessage>();
        var result = await applicationEngine.ReceiveMessageAsync(messageMock.Object, inputName);

        Assert.Equal(MessageResponse.None, result);
    }

    [Fact(DisplayName = "Applications No086_呼び出し", Skip = "Readyに変更するために前処理でRunningにする必要があるが、エラーとなるため状況を発生させることができないためスキップ")]
    public void Applications_Case086()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "Applications No087_すべての処理が正常に終了した場合")]
    public async void Applications_Case087()
    {
        ApplicationEngine applicationEngine = new();

        var applicationMainMock = new Mock<IApplicationMain>();
        var moduleClientMock = new Mock<IModuleClient>();

        var moduleClientInfo = applicationEngine.GetType().GetField("MyModuleClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        moduleClientInfo!.SetValue(applicationEngine, moduleClientMock.Object);
        applicationEngine.SetApplication(applicationMainMock.Object);

        var stateController = (StateController?)applicationEngine.GetType().GetField("stateController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(applicationEngine);
        var stateControllerType = stateController!.GetType();
        stateControllerType.GetField("currentState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(stateController, ApplicationState.Ready);

        var messageInputEventDataField = applicationEngine.GetType().GetField("messageInputEventData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var eventDataDict = (Dictionary<string, Commons.MessageEventData?>?)messageInputEventDataField!.GetValue(applicationEngine);
        var inputName = "testInput";

        MessageEventHandler mockMessageHandler = (inputName, receivedMessage, userContext) =>
        {
            return Task.FromResult(true);
        };

        var eventData = new Commons.MessageEventData(inputName, "testUserContext", mockMessageHandler);
        eventDataDict![inputName] = eventData;

        var messageMock = new Mock<IotMessage>();
        var result = await applicationEngine.ReceiveMessageAsync(messageMock.Object, inputName);

        Assert.Equal(MessageResponse.Completed, result);
    }
}