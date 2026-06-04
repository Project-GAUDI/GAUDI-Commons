using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;
/*
namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(DirectMethod_GetLogLevel_Run))]
    [CollectionDefinition(nameof(DirectMethod_GetLogLevel_Run), DisableParallelization = true)]
    public class DirectMethod_GetLogLevel_Run
    {
        private readonly ITestOutputHelper _output;
        private static ILogger _logger = LoggerFactory.GetLogger(typeof(DirectMethod_GetLogLevel_Run));


        public DirectMethod_GetLogLevel_Run(ITestOutputHelper output)
        {
            Environment.SetEnvironmentVariable("LogLevel", null);
            _output = output;
            _logger.SetOutputLogLevel("INFO");
        }


        [Fact(DisplayName = "正常系：正常構成")]
        public async void NormalInput_TrueReturned()
        {
            Environment.SetEnvironmentVariable("LogLevel", null);
            DirectMethod_GetLogLevel target = new DirectMethod_GetLogLevel();

            Assert.Equal(ILogger.LogLevel.INFO, _logger.OutputLogLevel);
            var result = await target.ParseRequest("");
            result = await target.Run();
            Assert.True(result);
        }
    }
}
*/