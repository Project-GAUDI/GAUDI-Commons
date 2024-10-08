using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(DirectMethod_GetLogLevel_ParseRequest))]
    [CollectionDefinition(nameof(DirectMethod_GetLogLevel_ParseRequest), DisableParallelization = true)]

    public class DirectMethod_GetLogLevel_ParseRequest
    {
        private readonly ITestOutputHelper _output;

        public DirectMethod_GetLogLevel_ParseRequest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "正常系：正常構成(null指定)")]
        public async void NormalInput_TrueReturned001()
        {
            DirectMethod_GetLogLevel target = new DirectMethod_GetLogLevel();
            
            var result = await target.ParseRequest(null);

            Assert.True(result);       
        }
        [Fact(DisplayName = "正常系：正常構成(空文字指定)")]
        public async void NormalInput_TrueReturned002()
        {
            DirectMethod_GetLogLevel target = new DirectMethod_GetLogLevel();
            
            var result = await target.ParseRequest("");

            Assert.True(result);       
        }

        [Fact(DisplayName = "正常系：正常構成(文字列指定)")]
        public async void NormalInput_TrueReturned003()
        {
            DirectMethod_GetLogLevel target = new DirectMethod_GetLogLevel();
            
            var result = await target.ParseRequest("teststring");

            Assert.True(result);       
        }
    }
}