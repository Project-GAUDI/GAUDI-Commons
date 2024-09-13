using System;
using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(Logger_SetOutputLogLevel))]
    [CollectionDefinition(nameof(Logger_SetOutputLogLevel), DisableParallelization = true)]
    public class Logger_SetOutputLogLevel
    {
        const string ConnectionString = "TestConnectionString";

        private readonly ITestOutputHelper _output;
        ILogger log = LoggerFactory.GetLogger(typeof(Logger_SetOutputLogLevel));


        public Logger_SetOutputLogLevel(ITestOutputHelper output)
        {
            _output = output;
            log.SetOutputLogLevel("INFO");
            log.SetMandatoryLogLevel("INFO", 0);

        }

        ///ログ出力レベル設定のテスト
        ///規定の文字列以外が引数に渡されたときにエラーが発生することを確認する。
        ///規定の文字列については、WriteLogTest_XXXの中で確認する。
        [Fact(DisplayName = "ログ出力レベル設定不正")]
        public void SetOutputLogLevelErrorTest()
        {
            

            Assert.Throws<ArgumentException>(() => log.SetOutputLogLevel("XXXX"));

        }
    }
}