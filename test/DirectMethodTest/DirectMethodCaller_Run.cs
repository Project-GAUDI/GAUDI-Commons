using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethodCaller_Run
{
    private static ILogger _logger = LoggerFactory.GetLogger(typeof(DirectMethod_GetLogLevel_Run));

    private readonly ITestOutputHelper _output;

    public DirectMethodCaller_Run(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case029_関数内で呼び出される")]
    public async void DirectMethod_Case029()
    {
        DirectMethodRequest request = new()
        {
            MethodName = "Test Method"
        };

        DirectMethodResponse response = await DirectMethodCaller.Run(request);

        Assert.Equal(-1, response.Status);
        string error = (string)response.Results["Error"];
        Assert.Contains("Direct Method(Test Method) execution failed.", error);
    }

    [Fact(DisplayName = "DirectMethod Case031_関数内で呼び出される", Skip = "runner.Runで予期せぬエラーが起こせないためスキップ")]
    public void DirectMethod_Case031()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "DirectMethod Case032_関数内で呼び出される", Skip = "GetResultで予期せぬエラーが起こせないためスキップ")]
    public void DirectMethod_Case032()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "DirectMethod Case033_メソッドを呼び出す")]
    public async void DirectMethod_Case033()
    {
        DirectMethodRequest request = new()
        {
            MethodName = "SetLogLevel",
            RequestJson = "{\"LogLevel\":\"DEBUG\",\"EnableSec\":\"30\"}"
        };

        DirectMethodResponse response = await DirectMethodCaller.Run(request);

        Assert.Equal(0, response.Status);
        string currentLogLevel = (string)response.Results["CurrentLogLevel"];
        Assert.Equal("debug", currentLogLevel);
    }

    [Fact(DisplayName = "DirectMethod Case044_メソッドを呼び出す")]
    public async void DirectMethod_Case044()
    {
        _logger.SetMandatoryLogLevel("DEBUG", -1);

        DirectMethodRequest request = new()
        {
            MethodName = "GetLogLevel"
        };

        DirectMethodResponse response = await DirectMethodCaller.Run(request);

        Assert.Equal(0, response.Status);
        string currentLogLevel = (string)response.Results["CurrentLogLevel"];
        Assert.Equal("debug", currentLogLevel);
    }
}