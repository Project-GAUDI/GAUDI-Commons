using Xunit;
using Xunit.Abstractions;
using System.Threading;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(DirectMethod_SetLogLevel_GetResult))]
    [CollectionDefinition(nameof(DirectMethod_SetLogLevel_GetResult), DisableParallelization = true)]
    public class DirectMethod_SetLogLevel_GetResult
    {
        private readonly ITestOutputHelper _output;
        private static ILogger _logger = LoggerFactory.GetLogger(typeof(DirectMethod_SetLogLevel_Run));


        public DirectMethod_SetLogLevel_GetResult(ITestOutputHelper output)
        {
            _output = output;
            _logger.SetOutputLogLevel("INFO");
            _logger.SetMandatoryLogLevel("INFO", 0);
        }

        [Fact(DisplayName = "異常系：null指定")]
        public async void NullInput_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest(null);
            Assert.False(result);

            var resp = target.GetResult();
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("Bad request.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：空文字列指定")]
        public async void EmptyStringInput_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("");
            Assert.False(result);

            var resp = target.GetResult();
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("Bad request.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：非JSON書式指定")]
        public async void NonJsonInput_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("This is Test.");
            Assert.False(result);

            var resp = target.GetResult();
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("Bad request.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：対象パラメータなし")]
        public async void NoParametersMatched_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"level\":\"debug\",\"seconds\":\"1\"}");
            Assert.False(result);

            var resp = target.GetResult();
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("Bad request.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：ログレベル指定なし")]
        public async void NoLogLevel_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"level\":\"debug\",\"EnableSec\":\"1\"}");
            Assert.False(result);

            var resp = target.GetResult();
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("Bad request.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：ログレベル指定不正")]
        public async void BadLogLevel_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"LogLevel\":\"デバッグ\",\"EnableSec\":\"1\"}");
            result = await target.Run();
            Assert.False(result);

            var resp = target.GetResult();
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("SetLogLevel failed.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "正常系：正常構成")]
        public async void NormalInput_TrueReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            Assert.Equal(ILogger.LogLevel.INFO, _logger.OutputLogLevel);
            var result = await target.ParseRequest("{\"LogLevel\":\"debug\",\"EnableSec\":\"1\"}");
            result = await target.Run();
            Assert.True(result);
            Assert.Equal(ILogger.LogLevel.DEBUG, _logger.OutputLogLevel);
            var resp = target.GetResult();
            Assert.Equal(0, resp.Status);
            Assert.StartsWith("debug", resp.Results["CurrentLogLevel"].ToString());
            Thread.Sleep(1000);
        }

        [Fact(DisplayName = "正常系：有効時間指定0(キャンセル)＋ログレベル指定なし")]
        public async void EnableSec0andNoLogLevel_TrueReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            // 準備（debugに一時変更）
            Assert.Equal(ILogger.LogLevel.INFO, _logger.OutputLogLevel);
            var result = await target.ParseRequest("{\"LogLevel\":\"debug\",\"EnableSec\":\"100\"}");
            result = await target.Run();
            Assert.True(result);
            Assert.Equal(ILogger.LogLevel.DEBUG, _logger.OutputLogLevel);
            var resp = target.GetResult();
            Assert.Equal(0, resp.Status);
            Assert.StartsWith("debug", resp.Results["CurrentLogLevel"].ToString());

            // テスト（キャンセル設定）
            target = new DirectMethod_SetLogLevel();
            result = await target.ParseRequest("{\"EnableSec\":\"0\"}");
            result = await target.Run();
            Assert.True(result);
            Assert.Equal(ILogger.LogLevel.INFO, _logger.OutputLogLevel);
            resp = target.GetResult();
            Assert.Equal(0, resp.Status);
            Assert.StartsWith("info", resp.Results["CurrentLogLevel"].ToString());
            
        }

    }
}
