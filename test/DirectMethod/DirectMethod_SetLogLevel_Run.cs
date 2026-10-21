using System.Threading.Tasks;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(SharedLoggerState))]
    public class DirectMethod_SetLogLevel_Run
    {
        [Fact(DisplayName = "No012_正常系：正常動作の確認_メソッドを呼び出す")]
        public async Task Case012()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_SetLogLevel();
            await testInstance.ParseRequest("{\"LogLevel\":\"Info\",\"EnableSec\":0}");

            var result = await testInstance.Run();
            var response = testInstance.GetResult();

            Assert.True(result);
            Assert.Equal(0, response.Status);
        }
    }
}