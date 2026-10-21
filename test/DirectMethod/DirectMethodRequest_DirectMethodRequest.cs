using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    public class DirectMethodRequest_DirectMethodRequest
    {
        [Fact(DisplayName = "No023_正常系：正常動作の確認_引数なしで初期化する")]
        public void Case023()
        {
            var result = new TICO.GAUDI.Commons.DirectMethodRequest();

            Assert.Equal(string.Empty, result.MethodName);
            Assert.Equal(string.Empty, result.RequestJson);
        }

        [Fact(DisplayName = "No024_正常系：正常動作の確認_引数を指定して初期化する")]
        public void Case024()
        {
            var result = new TICO.GAUDI.Commons.DirectMethodRequest("GetLogLevel", "requestJSON");

            Assert.Equal("GetLogLevel", result.MethodName);
            Assert.Equal("requestJSON", result.RequestJson);
        }
    }
}