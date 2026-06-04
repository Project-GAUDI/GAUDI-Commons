using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethod_SetLogLevel_GetResult
{
    private readonly ITestOutputHelper _output;

    public DirectMethod_SetLogLevel_GetResult(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case024_requestDataに値をセットしてメソッドを呼び出す")]
    public void DirectMethod_Case024()
    {
        DirectMethod_SetLogLevel setLogLevel = new();

        var responseDataField = setLogLevel.GetType().GetField("<responseData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var responseData = System.Activator.CreateInstance(
            responseDataField!.FieldType,
            [0, null, null]
        );
        var statusField = responseData!.GetType().GetProperty("Status", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        statusField!.SetValue(responseData, 0);
        var resultsField = responseData!.GetType().GetProperty("Results", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        resultsField!.SetValue(responseData, new Dictionary<string, object>
        {
            { "CurrentLogLevel", "debug" }
        });
        responseDataField.SetValue(setLogLevel, responseData);

        DirectMethodResponse ret = setLogLevel.GetResult();
        Assert.Equal(0, ret.Status);
        Assert.Equal("debug", ret.Results["CurrentLogLevel"]);
    }

    [Fact(DisplayName = "DirectMethod Case047_requestDataに値をセットせずメソッドを呼び出す")]
    public void DirectMethod_Case047()
    {
        DirectMethod_SetLogLevel setLogLevel = new();

        DirectMethodResponse ret = setLogLevel.GetResult();
        Assert.Equal(-1, ret.Status);
        Assert.Empty(ret.Results);
    }
}