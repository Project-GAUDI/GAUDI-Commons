using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethod_GetLogLevel_ParseRequest
{
    private readonly ITestOutputHelper _output;

    public DirectMethod_GetLogLevel_ParseRequest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case002_メソッドを呼び出す")]
    public async void DirectMethod_Case002()
    {
        try
        {
            DirectMethod_GetLogLevel getLogLevel = new();
            var ret = await getLogLevel.ParseRequest("Request JSON");
            Assert.True(ret);
        }
        catch
        {
            Assert.True(false);
        }
    }
}