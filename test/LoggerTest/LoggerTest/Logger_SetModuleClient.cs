using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.LoggerTest;

[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class Logger_SetModuleClient : LoggerBase
{
    public Logger_SetModuleClient(ITestOutputHelper output) : base(output) { }

    private readonly Logger _log = new();

    [Fact(DisplayName = "Logger No016_引数に有効なIModuleClientインスタンスを渡す")]
    public void Logger_Case016()
    {
        var myClient = CreateModuleClient();
        _log.SetModuleClient(myClient);

        Assert.Equal(myClient, _log.MyClient);
    }

    [Fact(DisplayName = "Logger No017_引数にnullを渡す")]
    public void Logger_Case017()
    {
        var myClient = CreateModuleClient();
        _log.SetModuleClient(myClient);

        _log.SetModuleClient(null);
        Assert.Null(_log.MyClient);
    }
}
