using System.Threading.Tasks;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    public class DirectMethod_SetLogLevel_ParseRequest
    {
        [Fact(DisplayName = "No004_異常系：異常動作の確認_requestJSONにJSON形式でない文字列を入れて、メソッドを呼び出す")]
        public async Task Case004()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_SetLogLevel();
            var requestJson = "invalid";

            var result = await testInstance.ParseRequest(requestJson);
            var response = testInstance.GetResult();

            Assert.False(result);
            Assert.Equal(-1, response.Status);
            Assert.Contains("Bad request.", response.Results["Error"] as string);
        }

        [Fact(DisplayName = "No005_異常系：異常動作の確認_requestJSONにEnableSecがnumberでない文字列を入れて、メソッドを呼び出す")]
        public async Task Case005()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_SetLogLevel();
            var requestJson = "{\"EnableSec\":\"invalid\",\"LogLevel\":\"Info\"}";

            var result = await testInstance.ParseRequest(requestJson);
            var response = testInstance.GetResult();

            Assert.False(result);
            Assert.Equal(-1, response.Status);
            Assert.Contains("Bad request.", response.Results["Error"] as string);
        }

        [Fact(DisplayName = "No006_異常系：異常動作の確認_requestJSONに空の文字列を入れて、メソッドを呼び出す")]
        public async Task Case006()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_SetLogLevel();
            var requestJson = "";

            var result = await testInstance.ParseRequest(requestJson);
            var response = testInstance.GetResult();

            Assert.False(result);
            Assert.Equal(-1, response.Status);
            Assert.Contains("Bad request.", response.Results["Error"] as string);
        }

        [Fact(DisplayName = "No007_異常系：異常動作の確認_requestJSONにLogLevelの指定がなく、EnableSecが0でない文字列を入れて、メソッドを呼び出す")]
        public async Task Case007()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_SetLogLevel();
            var requestJson = "{\"EnableSec\":1}";

            var result = await testInstance.ParseRequest(requestJson);
            var response = testInstance.GetResult();

            Assert.False(result);
            Assert.Equal(-1, response.Status);
            Assert.Contains("Bad request.", response.Results["Error"] as string);
        }

        [Fact(DisplayName = "No008_異常系：異常動作の確認_requestJSONにEnableSecが-1以上でない文字列を入れて、メソッドを呼び出す")]
        public async Task Case008()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_SetLogLevel();
            var requestJson = "{\"LogLevel\":\"Info\",\"EnableSec\":-2}";

            var result = await testInstance.ParseRequest(requestJson);
            var response = testInstance.GetResult();

            Assert.False(result);
            Assert.Equal(-1, response.Status);
            Assert.Contains("Bad request.", response.Results["Error"] as string);
        }

        [Fact(DisplayName = "No009_正常系：正常動作の確認_requestJSONにLogLevelの指定がなく、EnableSecが0の文字列を入れて、メソッドを呼び出す")]
        public async Task Case009()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_SetLogLevel();
            var requestJson = "{\"EnableSec\":0}";

            var result = await testInstance.ParseRequest(requestJson);

            Assert.True(result);
        }

        [Fact(DisplayName = "No010_正常系：正常動作の確認_requestJSONにLogLevelを指定し、EnableSecが-1の文字列を入れて、メソッドを呼び出す")]
        public async Task Case010()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_SetLogLevel();
            var requestJson = "{\"LogLevel\":\"Info\",\"EnableSec\":-1}";

            var result = await testInstance.ParseRequest(requestJson);

            Assert.True(result);
        }

        [Fact(DisplayName = "No011_正常系：正常動作の確認_requestJSONにLogLevelを指定し、EnableSecが1以上の文字列を入れて、メソッドを呼び出す")]
        public async Task Case011()
        {
            var testInstance = new TICO.GAUDI.Commons.DirectMethod_SetLogLevel();
            var requestJson = "{\"LogLevel\":\"Info\",\"EnableSec\":1}";

            var result = await testInstance.ParseRequest(requestJson);

            Assert.True(result);
        }
    }
}