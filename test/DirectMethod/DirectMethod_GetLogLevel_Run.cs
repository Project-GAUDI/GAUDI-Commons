using System.Threading.Tasks;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(SharedLoggerState))]
    public class DirectMethod_GetLogLevel_Run
    {
        [Fact(DisplayName = "No002_正常系：正常動作の確認_メソッドを呼び出す")]
        public async Task Case002()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_GetLogLevel();

            var result = await testInstance.Run();
            var response = testInstance.GetResult();

            Assert.True(result);
            Assert.Equal(0, response.Status);
        }
    }
}