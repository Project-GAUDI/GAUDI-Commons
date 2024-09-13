using System.Threading;
using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(DirectMethod_SetLogLevel_Run))]
    [CollectionDefinition(nameof(DirectMethod_SetLogLevel_Run), DisableParallelization = true)]
    public class DirectMethod_SetLogLevel_Run
    {
        private readonly ITestOutputHelper _output;
        private static ILogger _logger = LoggerFactory.GetLogger(typeof(DirectMethod_SetLogLevel_Run));


        public DirectMethod_SetLogLevel_Run(ITestOutputHelper output)
        {
            _output = output;
            _logger.SetOutputLogLevel("INFO");
            _logger.SetMandatoryLogLevel("INFO", -1);
        }

        [Fact(DisplayName = "異常系：ログレベル指定不正")]
        public async void BadLogLevel_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"LogLevel\":\"デバッグ\",\"EnableSec\":\"1\"}");
            result = await target.Run();

            Assert.False(result);
        }

        [Fact(DisplayName = "正常系：有効時間指定0(キャンセル)＋ログレベル指定なし")]
        public async void EnableSec0andNoLogLevel_TrueReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"EnableSec\":\"0\"}");
            result = await target.Run();

            Assert.True(result);
        }

        [Fact(DisplayName = "正常系：正常構成")]
        public async void NormalInput_TrueReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            Assert.Equal(ILogger.LogLevel.INFO, _logger.OutputLogLevel);
            var result = await target.ParseRequest("{\"LogLevel\":\"debug\",\"EnableSec\":\"2\"}");
            result = await target.Run();
            Assert.True(result);
            Assert.Equal(ILogger.LogLevel.DEBUG, _logger.OutputLogLevel);
            Thread.Sleep(3000);
            Assert.Equal(ILogger.LogLevel.INFO, _logger.OutputLogLevel);

        }
    }
}
