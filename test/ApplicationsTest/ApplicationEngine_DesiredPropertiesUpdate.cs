using Xunit;
using Xunit.Abstractions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Microsoft.Azure.Devices.Shared;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_DesiredPropertiesUpdate))]
public class ApplicationEngine_DesiredPropertiesUpdate
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_DesiredPropertiesUpdate(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No106_DesiredPropertiesUpdateを呼び出し")]
    public void Applications_Case106()
    {
        var twinCollection = new TwinCollection(
             @"{
                ""info1"": {
                    ""second"": ""0""
                }
            }"
         );
        object?[] parameters = [twinCollection, null];

        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);

        var methodInfo = typeof(ApplicationEngine).GetMethod("DesiredPropertiesUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task<bool>?)methodInfo!.Invoke(applicationEngine, parameters);

        applicationMainMock.Verify(x => x.OnDesiredPropertiesReceivedAsync(It.IsAny<JObject>()), Times.Once);
    }

    [Fact(DisplayName = "Applications No107_applicationMain.OnDesiredPropertiesReceivedAsyncの結果が失敗で返ってきた場合")]
    public async void Applications_Case107()
    {
        var twinCollection = new TwinCollection(
            @"{
                ""info1"": {
                    ""second"": ""0""
                }
            }"
        );
        object?[] parameters = [twinCollection, null];

        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.OnDesiredPropertiesReceivedAsync(It.IsAny<JObject>()))
            .ReturnsAsync(false)
            .Verifiable();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        var methodInfo = typeof(ApplicationEngine).GetMethod("DesiredPropertiesUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task<bool>?)methodInfo!.Invoke(applicationEngine, parameters);
        bool ret = await task!;
        Assert.False(ret);
    }

    [Fact(DisplayName = "Applications No108_予期せぬエラーが発生した場合")]
    public async void Applications_Case108()
    {
        var twinCollection = new TwinCollection(
            @"{
                ""info1"": {
                    ""second"": ""0""
                }
            }"
        );
        object?[] parameters = [twinCollection, null];

        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.OnDesiredPropertiesReceivedAsync(It.IsAny<JObject>()))
            .Throws(new System.Exception());
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        var methodInfo = typeof(ApplicationEngine).GetMethod("DesiredPropertiesUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task<bool>?)methodInfo!.Invoke(applicationEngine, parameters);
        bool ret = await task!;
        Assert.True(ret);
    }

    [Fact(DisplayName = "Applications No109_applicationMain.OnDesiredPropertiesReceivedAsyncの結果が成功で返ってきた場合")]
    public async void Applications_Case109()
    {
        var twinCollection = new TwinCollection(
            @"{
                ""info1"": {
                    ""second"": ""0""
                }
            }"
        );
        object?[] parameters = [twinCollection, null];

        var applicationMainMock = new Mock<IApplicationMain>();
        applicationMainMock
            .Setup(x => x.OnDesiredPropertiesReceivedAsync(It.IsAny<JObject>()))
            .ReturnsAsync(true)
            .Verifiable();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        var methodInfo = typeof(ApplicationEngine).GetMethod("DesiredPropertiesUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task<bool>?)methodInfo!.Invoke(applicationEngine, parameters);
        bool ret = await task!;
        Assert.True(ret);
    }
}