using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethod_SetLogLevel_Run
{
    private readonly ITestOutputHelper _output;

    public DirectMethod_SetLogLevel_Run(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case017_関数内で呼び出される LoggerFactory.GetLogger が例外をスローする", Skip = "予期せぬエラーが起こせない(EnableSec=-2としたがValidateとRunでエラーが起こり、別の予期せぬエラーが発生する)ためスキップ")]
    public void DirectMethod_Case017()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "DirectMethod Case018_関数内で呼び出される LoggerFactory.GetLogger が例外をスローする", Skip = "予期せぬエラーが起こせない(EnableSec=-2としたがValidateとRunでエラーが起こり、別の予期せぬエラーが発生する)ためスキップ")]
    public void DirectMethod_Case018()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "DirectMethod Case019_関数内で呼び出される LoggerFactory.GetLogger が返却したインスタンスから呼び出す SetMandatoryLogLevel 関数が例外をスローする", Skip = "予期せぬエラーが起こせない(EnableSec=-2としたがValidateとRunでエラーが起こり、別の予期せぬエラーが発生する)ためスキップ")]
    public void DirectMethod_Case019()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "DirectMethod Case020_関数内で呼び出される LoggerFactory.GetLogger が返却したインスタンスから呼び出す SetMandatoryLogLevel 関数が例外をスローする", Skip = "予期せぬエラーが起こせない(EnableSec=-2としたがValidateとRunでエラーが起こり、別の予期せぬエラーが発生する)ためスキップ")]
    public void DirectMethod_Case020()
    {
        Assert.True(true);
    }

    [Fact(DisplayName = "DirectMethod Case023_メソッドを呼び出す")]
    public async void DirectMethod_Case023()
    {
        DirectMethod_SetLogLevel setLogLevel = new();

        var requestDataField = setLogLevel.GetType().GetField("<requestData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var requestData = System.Activator.CreateInstance(requestDataField!.FieldType);
        var logLevelField = requestData!.GetType().GetField("LogLevel", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        logLevelField!.SetValue(requestData, "DEBUG");
        var enableSecProperty = requestData.GetType().GetProperty("EnableSec", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        enableSecProperty!.SetValue(requestData, 30);
        requestDataField.SetValue(setLogLevel, requestData);

        bool ret = await setLogLevel.Run();
        Assert.True(ret);

        var responseDataField = setLogLevel.GetType().GetField("<responseData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var responseData = responseDataField!.GetValue(setLogLevel) as DirectMethodResponse;

        Assert.Equal(0, responseData!.Status);
        Assert.Equal("debug", responseData.Results["CurrentLogLevel"]);
    }
}