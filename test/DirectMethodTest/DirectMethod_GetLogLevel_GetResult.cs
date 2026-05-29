using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethod_GetLogLevel_GetResult
{
    private static ILogger _logger = LoggerFactory.GetLogger(typeof(DirectMethod_GetLogLevel_GetResult));

    private readonly ITestOutputHelper _output;

    public DirectMethod_GetLogLevel_GetResult(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case007_メンバー変数requestDataに指定した値が設定されている")]
    public async void DirectMethod_Case007()
    {
        _logger.SetMandatoryLogLevel("DEBUG", -1);
        var getLogLevel = new DirectMethod_GetLogLevel();
        await getLogLevel.Run();
        var ret = getLogLevel.GetResult();
        Assert.Equal(0, ret.Status);
        Assert.Equal("debug", ret.Results["CurrentLogLevel"]);
    }
}