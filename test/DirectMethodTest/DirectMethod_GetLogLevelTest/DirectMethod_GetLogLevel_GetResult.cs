using Xunit;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(DirectMethod_GetLogLevel_GetResult))]
    [CollectionDefinition(nameof(DirectMethod_GetLogLevel_GetResult), DisableParallelization = true)]

    public class DirectMethod_GetLogLevel_GetResult
    {
        private static ILogger _logger = LoggerFactory.GetLogger(typeof(DirectMethod_SetLogLevel_Run));

        [Fact(DisplayName = "正常系：正常構成")]
        public async void NormalInput_TrueReturned()
        {
            DirectMethod_GetLogLevel target1 = new DirectMethod_GetLogLevel();
            _logger.SetOutputLogLevel("INFO");
            Assert.Equal(ILogger.LogLevel.INFO, _logger.OutputLogLevel);
            _logger.SetMandatoryLogLevel("DEBUG", -1);
            var result1 = await target1.ParseRequest("");
            result1 = await target1.Run();
            Assert.True(result1);
            var resp1 = target1.GetResult();
            Assert.Equal(0, resp1.Status);
            Assert.StartsWith("debug", resp1.Results["CurrentLogLevel"].ToString());
            _logger.SetMandatoryLogLevel("INFO", 0);
            DirectMethod_GetLogLevel target2 = new DirectMethod_GetLogLevel();
            var result2 = await target2.ParseRequest(null);
            result2 = await target2.Run();
            Assert.True(result2);
            var resp2 = target2.GetResult();
            Assert.Equal(0, resp2.Status);
            Assert.StartsWith("info", resp2.Results["CurrentLogLevel"].ToString());
        }
    }
}