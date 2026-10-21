using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(SharedLoggerState))]
    public class DirectMethod_GetLogLevel_GetResult
    {
        [Fact(DisplayName = "No003_正常系：デフォルト値の確認_ENV:LogLevelが設定されていない状態で、Run()を呼び出した後、GetResult()を呼び出す")]
        public async Task Case003()
        {
            var originalLogLevel = Environment.GetEnvironmentVariable("LogLevel");
            Environment.SetEnvironmentVariable("LogLevel", null);
            ResetLoggerInitialization();

            try
            {
                var testInstance = new TICO.GAUDI.Commons.DirectMethod_GetLogLevel();
                await testInstance.Run();

                var result = testInstance.GetResult();

                Assert.Equal(0, result.Status);
                Assert.Equal("info", result.Results["CurrentLogLevel"]);
            }
            finally
            {
                Environment.SetEnvironmentVariable("LogLevel", originalLogLevel);
                ResetLoggerInitialization();
            }
        }

        private static void ResetLoggerInitialization()
        {
            var field = typeof(TICO.GAUDI.Commons.LoggerFactory).GetField(
                "isLoggerInitialized",
                BindingFlags.Static | BindingFlags.NonPublic);
            field!.SetValue(null, false);
        }
    }
}