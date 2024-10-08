using System.Threading;
using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(DirectMethodCaller_Run))]
    [CollectionDefinition(nameof(DirectMethodCaller_Run), DisableParallelization = true)]
    public class DirectMethodCaller_Run
    {
        private readonly ITestOutputHelper _output;
        private static ILogger _logger = LoggerFactory.GetLogger(typeof(DirectMethod_SetLogLevel_Run));


        public DirectMethodCaller_Run(ITestOutputHelper output)
        {
            _output = output;
            _logger.SetOutputLogLevel("INFO");
            _logger.SetMandatoryLogLevel("INFO", -1);
        }

        [Fact(DisplayName = "異常系：メソッド指定null")]
        public async void MethodNameNullInput_ExecutionFailedReturned()
        {
            DirectMethodRequest methodRequest = new DirectMethodRequest() { MethodName = null };
            methodRequest.RequestJson = null;

            var resp = await DirectMethodCaller.Run(methodRequest);
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith($"Direct Method({methodRequest.MethodName}) execution failed.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：メソッド指定空文字列")]
        public async void MethodNameEmptyInput_ExecutionFailedReturned()
        {
            DirectMethodRequest methodRequest = new DirectMethodRequest() { MethodName = "" };
            methodRequest.RequestJson = null;

            var resp = await DirectMethodCaller.Run(methodRequest);
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith($"Direct Method({methodRequest.MethodName}) execution failed.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：存在しないメソッド指定")]
        public async void NotExistMethodNameInput_ExecutionFailedReturned()
        {
            DirectMethodRequest methodRequest = new DirectMethodRequest() { MethodName = "ResetLogLevel" };
            methodRequest.RequestJson = null;

            var resp = await DirectMethodCaller.Run(methodRequest);
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith($"Direct Method({methodRequest.MethodName}) execution failed.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：リクエストnull指定")]
        public async void RequestNullInput_BadRequestReturned()
        {
            DirectMethodRequest methodRequest = new DirectMethodRequest() { MethodName = "SetLogLevel" };
            methodRequest.RequestJson = null;

            var resp = await DirectMethodCaller.Run(methodRequest);
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("Bad request.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：リクエスト空文字列指定")]
        public async void RequestEmptyStringInput_BadRequestReturned()
        {
            DirectMethodRequest methodRequest = new DirectMethodRequest() { MethodName = "SetLogLevel" };
            methodRequest.RequestJson = "";

            var resp = await DirectMethodCaller.Run(methodRequest);
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("Bad request.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：リクエスト非JSON書式指定")]
        public async void RequestNonJsonInput_BadRequestReturned()
        {
            DirectMethodRequest methodRequest = new DirectMethodRequest() { MethodName = "SetLogLevel" };
            methodRequest.RequestJson = "This is Test.";

            var resp = await DirectMethodCaller.Run(methodRequest);
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("Bad request.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：リクエスト対象パラメータなし")]
        public async void RequestNoParametersMatched_BadRequestReturned()
        {
            DirectMethodRequest methodRequest = new DirectMethodRequest() { MethodName = "SetLogLevel" };
            methodRequest.RequestJson = "{\"level\":\"debug\",\"seconds\":\"1\"}";

            var resp = await DirectMethodCaller.Run(methodRequest);
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("Bad request.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：リクエストログレベル指定なし")]
        public async void RequestNoLogLevel_BadRequestReturned()
        {
            DirectMethodRequest methodRequest = new DirectMethodRequest() { MethodName = "SetLogLevel" };
            methodRequest.RequestJson = "{\"level\":\"debug\",\"EnableSec\":\"1\"}";

            var resp = await DirectMethodCaller.Run(methodRequest);
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("Bad request.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "異常系：リクエストログレベル指定不正")]
        public async void RequestBadLogLevel_SetLogLevelFailedReturned()
        {
            DirectMethodRequest methodRequest = new DirectMethodRequest() { MethodName = "SetLogLevel" };
            methodRequest.RequestJson = "{\"LogLevel\":\"デバッグ\",\"EnableSec\":\"1\"}";

            var resp = await DirectMethodCaller.Run(methodRequest);
            Assert.Equal(-1, resp.Status);
            Assert.StartsWith("SetLogLevel failed.", resp.Results["Error"].ToString());
        }

        [Fact(DisplayName = "正常系：正常構成")]
        public async void NormalInput_TrueReturned()
        {
            DirectMethodRequest methodRequest = new DirectMethodRequest() { MethodName = "SetLogLevel" };
            methodRequest.RequestJson = "{\"LogLevel\":\"debug\",\"EnableSec\":\"1\"}";

            var resp = await DirectMethodCaller.Run(methodRequest);
            Assert.Equal(0, resp.Status);
            Assert.StartsWith("debug", resp.Results["CurrentLogLevel"].ToString());
            Thread.Sleep(1000);
        }

    }
}
