using System.Threading.Tasks;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(SharedLoggerState))]
    public class DirectMethod_SetLogLevel_GetResult
    {
        [Fact(DisplayName = "No013_正常系：正常動作の確認_requestJSONにLogLevel=debug、EnableSec=1を指定してParseRequest、Run後にGetResultを呼び出す")]
        public async Task Case013()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_SetLogLevel();
            await testInstance.ParseRequest("{\"LogLevel\":\"debug\",\"EnableSec\":1}");
            await testInstance.Run();

            var result = testInstance.GetResult();

            Assert.Equal(0, result.Status);
            Assert.Equal("debug", result.Results["CurrentLogLevel"]);
        }
    }
}