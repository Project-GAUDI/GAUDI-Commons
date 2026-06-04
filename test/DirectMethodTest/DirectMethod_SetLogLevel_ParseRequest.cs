using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethod_SetLogLevel_ParseRequest
{
    private readonly ITestOutputHelper _output;

    public DirectMethod_SetLogLevel_ParseRequest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case012_関数内で呼び出される")]
    public async void DirectMethod_Case012()
    {
        DirectMethod_SetLogLevel setLogLevel = new();
        System.Environment.SetEnvironmentVariable("OTEDGE_COMMON_DEFAULT_JSONSERIALIZER", "NEWTONSOFTJSON");
        //引数にnullを指定することでserializer.Deserialize<RequestData>(requestJSON)はnullを返す
        var ret = await setLogLevel.ParseRequest(null);
        Assert.False(ret);

        var requestDataField = setLogLevel.GetType().GetField("<requestData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var requestData = requestDataField!.GetValue(setLogLevel);

        var responseDataField = setLogLevel.GetType().GetField("<responseData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var responseData = responseDataField!.GetValue(setLogLevel) as DirectMethodResponse;

        Assert.Null(requestData);
        Assert.Equal(-1, responseData!.Status);
        Assert.Equal("Bad request.()", responseData.Results["Error"]);
    }

    [Fact(DisplayName = "DirectMethod Case014_関数内で呼び出される")]
    public async void DirectMethod_Case014()
    {
        DirectMethod_SetLogLevel setLogLevel = new();
        var ret = await setLogLevel.ParseRequest("SAMPLE");
        Assert.False(ret);

        var requestDataField = setLogLevel.GetType().GetField("<requestData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var requestData = requestDataField!.GetValue(setLogLevel);

        var responseDataField = setLogLevel.GetType().GetField("<responseData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var responseData = responseDataField!.GetValue(setLogLevel) as DirectMethodResponse;

        Assert.Null(requestData);
        Assert.Equal(-1, responseData!.Status);
        Assert.Equal("Bad request.(SAMPLE)", responseData.Results["Error"]);
    }

    [Fact(DisplayName = "DirectMethod Case016_関数内で呼び出される")]
    public async void DirectMethod_Case016()
    {
        DirectMethod_SetLogLevel setLogLevel = new();
        var ret = await setLogLevel.ParseRequest("{\"LogLevel\":\"DEBUG\",\"EnableSec\":\"1\"}");
        Assert.True(ret);

        var requestDataField = setLogLevel.GetType().GetField("<requestData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var requestData = requestDataField!.GetValue(setLogLevel);

        var logLevelField = requestData!.GetType().GetField("LogLevel", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var logLevelValue = logLevelField!.GetValue(requestData) as string;

        var enableSecProperty = requestData!.GetType().GetProperty("EnableSec", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var enableSecValue = enableSecProperty!.GetValue(requestData) as int?;

        Assert.Equal("DEBUG", logLevelValue);
        Assert.Equal(1, enableSecValue);
    }
}