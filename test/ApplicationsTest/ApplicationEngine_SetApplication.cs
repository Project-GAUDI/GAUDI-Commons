using Xunit;
using Xunit.Abstractions;
using Moq;

namespace TICO.GAUDI.Commons.Test.ApplicationsTest;

[Collection(nameof(ApplicationEngine_SetApplication))]
public class ApplicationEngine_SetApplication
{
    private readonly ITestOutputHelper _output;

    public ApplicationEngine_SetApplication(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Applications No006_SetApplicationを呼び出し")]
    public void Applications_Case006()
    {
        var applicationMainMock = new Mock<IApplicationMain>();
        ApplicationEngine applicationEngine = new();
        applicationEngine.SetApplication(applicationMainMock.Object);
        var value = applicationEngine.GetType().GetField("applicationMain", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(applicationEngine);
        Assert.Same(applicationMainMock.Object, value);
    }
}
