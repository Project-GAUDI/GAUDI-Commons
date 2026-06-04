using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethodResponse_Constructor
{
    private readonly ITestOutputHelper _output;

    public DirectMethodResponse_Constructor(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case040_引数なしで初期化する")]
    public void DirectMethod_Case040()
    {
        DirectMethodResponse response = new();

        Assert.Equal(0, response.Status);
        Assert.Empty(response.Results);
    }

    [Fact(DisplayName = "DirectMethod Case041_status: 1で初期化する")]
    public void DirectMethod_Case041()
    {
        DirectMethodResponse response = new(1);

        Assert.Equal(1, response.Status);
        Assert.Empty(response.Results);
    }

    [Fact(DisplayName = "DirectMethod Case042_status: 1、resultKey: result-keyで初期化する")]
    public void DirectMethod_Case042()
    {
        DirectMethodResponse response = new(1, "result-key");

        Assert.Equal(1, response.Status);
        Assert.Empty(response.Results);
    }

    [Fact(DisplayName = "DirectMethod Case043_status: 1、resultKey: result-key、resultValue: test result valueで初期化する")]
    public void DirectMethod_Case043()
    {
        DirectMethodResponse response = new(1, "result-key", "test result value");

        Assert.Equal(1, response.Status);
        Assert.Equal("test result value", response.Results["result-key"]);
    }

    [Fact(DisplayName = "DirectMethod Case046_status: 1、resultValue: test result valueで初期化する")]
    public void DirectMethod_Case046()
    {
        DirectMethodResponse response = new(1, null, "test result value");

        Assert.Equal(1, response.Status);
        Assert.Empty(response.Results);
    }
}