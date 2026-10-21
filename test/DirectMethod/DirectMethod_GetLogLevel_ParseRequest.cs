using System.Threading.Tasks;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    public class DirectMethod_GetLogLevel_ParseRequest
    {
        [Fact(DisplayName = "No001_正常系：正常動作の確認_requestJSONに任意の文字列を入れて、メソッドを呼び出す")]
        public async Task Case001()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_GetLogLevel();
            var requestJson = "requestJSON";

            var result = await testInstance.ParseRequest(requestJson);

            Assert.True(result);
        }
    }
}