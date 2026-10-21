using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    public class DirectMethodResponse_DirectMethodResponse
    {
        [Fact(DisplayName = "No025_正常系：正常動作の確認_引数なしで初期化する")]
        public void Case025()
        {
            var result = new TICO.GAUDI.Commons.DirectMethodResponse();

            Assert.Equal(0, result.Status);
            Assert.Empty(result.Results);
        }

        [Fact(DisplayName = "No026_正常系：正常動作の確認_status=1を指定して初期化する")]
        public void Case026()
        {
            var result = new TICO.GAUDI.Commons.DirectMethodResponse(1);

            Assert.Equal(1, result.Status);
            Assert.Empty(result.Results);
        }

        [Fact(DisplayName = "No027_正常系：正常動作の確認_status、resultKeyを指定して初期化する")]
        public void Case027()
        {
            var result = new TICO.GAUDI.Commons.DirectMethodResponse(1, "ResultKey");

            Assert.Equal(1, result.Status);
            Assert.Empty(result.Results);
        }

        [Fact(DisplayName = "No028_正常系：正常動作の確認_status、resultValueを指定して初期化する")]
        public void Case028()
        {
            var result = new TICO.GAUDI.Commons.DirectMethodResponse(
                status: 1,
                resultValue: "ResultValue");

            Assert.Equal(1, result.Status);
            Assert.Empty(result.Results);
        }

        [Fact(DisplayName = "No029_正常系：正常動作の確認_status、resultKey、resultValueを指定して初期化する")]
        public void Case029()
        {
            var result = new TICO.GAUDI.Commons.DirectMethodResponse(1, "ResultKey", "ResultValue");

            Assert.Equal(1, result.Status);
            Assert.Equal("ResultValue", result.Results["ResultKey"]);
        }
    }
}