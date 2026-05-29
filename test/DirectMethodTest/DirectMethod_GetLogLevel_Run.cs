using System;
using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethod_GetLogLevel_Run
{
    private static ILogger _logger = LoggerFactory.GetLogger(typeof(DirectMethod_GetLogLevel_Run));

    private readonly ITestOutputHelper _output;

    public DirectMethod_GetLogLevel_Run(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case004_関数内で呼び出される")]
    public async void DirectMethod_Case004()
    {
        _logger.SetMandatoryLogLevel("DEBUG", -1);
        var getLogLevel = new DirectMethod_GetLogLevel();
        bool ret = await getLogLevel.Run();
        Assert.True(ret);

        var responseDataField = getLogLevel.GetType().GetField("<responseData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var responseData = responseDataField!.GetValue(getLogLevel) as DirectMethodResponse;

        Assert.Equal(0, responseData!.Status);
        Assert.Equal("debug", responseData.Results["CurrentLogLevel"]);
    }

    [Fact(DisplayName = "DirectMethod Case005_関数内で呼び出される", Skip = "例外が発生させられないためスキップ")]
    public void DirectMethod_Case005()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "DirectMethod Case006_関数内で呼び出される", Skip = "例外が発生させられないためスキップ")]
    public void DirectMethod_Case006()
    {
        Assert.True(true);
    }
}