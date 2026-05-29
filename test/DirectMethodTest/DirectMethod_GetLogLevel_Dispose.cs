using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethod_GetLogLevel_Dispose
{
    private readonly ITestOutputHelper _output;

    public DirectMethod_GetLogLevel_Dispose(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case001_メソッドを呼び出す")]
    public void DirectMethod_Case001()
    {
        DirectMethod_GetLogLevel getLogLevel = new();
        getLogLevel.Dispose();
    }
}