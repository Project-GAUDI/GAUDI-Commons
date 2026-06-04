using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethod_SetLogLevel_Dispose
{
    private readonly ITestOutputHelper _output;

    public DirectMethod_SetLogLevel_Dispose(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case008_メソッドを呼び出す")]
    public void DirectMethod_Case008()
    {
        DirectMethod_SetLogLevel setLogLevel = new();
        setLogLevel.Dispose();
    }
}