using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(Logger_IsLogLevelToOutput))]
    [CollectionDefinition(nameof(Logger_IsLogLevelToOutput), DisableParallelization = true)]
    public class Logger_IsLogLevelToOutput
    {
        const string ConnectionString = "TestConnectionString";

        private readonly ITestOutputHelper _output;
        ILogger log = LoggerFactory.GetLogger(typeof(Logger_IsLogLevelToOutput));


        public Logger_IsLogLevelToOutput(ITestOutputHelper output)
        {
            _output = output;
            log.SetOutputLogLevel("INFO");
            log.SetMandatoryLogLevel("INFO",0);
        }

        ///ログ出力可否判定テストその１
        ///ログ出力可否判定がTrueの場合
        [Fact(DisplayName = "ログ出力可否判定1：True")]
        public void IsLogLevelToOutputTest_TrueReturned001()
        {
            log.SetOutputLogLevel("INFO");
            Assert.True(log.IsLogLevelToOutput(ILogger.LogLevel.WARN));
        }

        ///ログ出力可否判定テストその２
        ///ログ出力可否判定がTrueの場合（同値）
        [Fact(DisplayName = "ログ出力可否判定2：True")]
        public void IsLogLevelToOutputTest_TrueReturned002()
        {
            
            log.SetOutputLogLevel("INFO");
            Assert.True(log.IsLogLevelToOutput(ILogger.LogLevel.INFO));
        }

        ///ログ出力可否判定テストその３
        ///ログ出力可否判定がFalseの場合
        [Fact(DisplayName = "ログ出力可否判定3：False")]
        public void IsLogLevelToOutputTest_FalseReturned001()
        {
            log.SetOutputLogLevel("INFO");
            Assert.False(log.IsLogLevelToOutput(ILogger.LogLevel.TRACE));
        }
    }
}
