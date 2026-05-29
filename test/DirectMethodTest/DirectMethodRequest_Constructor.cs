using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethodRequest_Constructor
{
    private readonly ITestOutputHelper _output;

    public DirectMethodRequest_Constructor(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case038_引数なしで初期化する")]
    public void DirectMethod_Case038()
    {
        DirectMethodRequest request = new();

        Assert.Equal("", request.MethodName);
        Assert.Equal("", request.RequestJson);
    }

    [Fact(DisplayName = "DirectMethod Case039_引数なしで初期化する")]
    public void DirectMethod_Case039()
    {
        DirectMethodRequest request = new("test method name", "test request json");

        Assert.Equal("test method name", request.MethodName);
        Assert.Equal("test request json", request.RequestJson);
    }
}