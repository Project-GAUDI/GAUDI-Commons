using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(DirectMethod_SetLogLevel_ParseRequest))]
    [CollectionDefinition(nameof(DirectMethod_SetLogLevel_ParseRequest), DisableParallelization = true)]
    public class DirectMethod_SetLogLevel_ParseRequest
    {
        private readonly ITestOutputHelper _output;

        public DirectMethod_SetLogLevel_ParseRequest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "異常系：null指定")]
        public async void NullInput_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest(null);

            Assert.False(result);
        }

        [Fact(DisplayName = "異常系：空文字列指定")]
        public async void EmptyStringInput_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("");

            Assert.False(result);
        }

        [Fact(DisplayName = "異常系：非JSON書式指定")]
        public async void NonJsonInput_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("This is Test.");

            Assert.False(result);
        }

        [Fact(DisplayName = "異常系：対象パラメータなし")]
        public async void NoParametersMatched_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"level\":\"debug\",\"seconds\":\"1\"}");

            Assert.False(result);
        }

        [Fact(DisplayName = "異常系：ログレベル指定なし")]
        public async void NoLogLevel_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"level\":\"debug\",\"EnableSec\":\"1\"}");

            Assert.False(result);
        }

        [Fact(DisplayName = "異常系：有効時間指定なし")]
        public async void NoEnableSec_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"LogLevel\":\"debug\",\"seconds\":\"1\"}");

            Assert.False(result);
        }

        [Fact(DisplayName = "異常系：有効時間指定範囲外")]
        public async void BadEnableSec_FalseReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"LogLevel\":\"debug\",\"EnableSec\":\"-2\"}");

            Assert.False(result);
        }

        [Fact(DisplayName = "正常系：ログレベル指定不正")]
        public async void BadLogLevel_TrueReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"LogLevel\":\"デバッグ\",\"EnableSec\":\"1\"}");

            Assert.True(result);
        }

        [Fact(DisplayName = "正常系：有効時間指定0(キャンセル)＋ログレベル指定なし")]
        public async void EnableSec0andNoLogLevel_TrueReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"EnableSec\":\"0\"}");

            Assert.True(result);
        }

        [Fact(DisplayName = "正常系：正常構成")]
        public async void NormalInput_TrueReturned()
        {
            DirectMethod_SetLogLevel target = new DirectMethod_SetLogLevel();

            var result = await target.ParseRequest("{\"LogLevel\":\"debug\",\"EnableSec\":\"1\"}");

            Assert.True(result);
        }
    }
}
