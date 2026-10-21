using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(SharedLoggerState))]
    public class DirectMethodCaller_Run
    {
        [Fact(DisplayName = "No014_異常系：異常動作の確認_存在しないDirectMethod名を指定してメソッドを呼び出す")]
        public async Task Case014()
        {
            var request = new TICO.GAUDI.Commons.DirectMethodRequest("UnknownMethod", "");

            var result = await TICO.GAUDI.Commons.DirectMethodCaller.Run(request);

            Assert.Equal(-1, result.Status);
            Assert.Contains("Direct Method(UnknownMethod) execution failed.", result.Results["Error"] as string);
        }

        [Fact(DisplayName = "No015_異常系：異常動作の確認_空のDirectMethod名を指定してメソッドを呼び出す")]
        public async Task Case015()
        {
            var request = new TICO.GAUDI.Commons.DirectMethodRequest("", "");

            var result = await TICO.GAUDI.Commons.DirectMethodCaller.Run(request);

            Assert.Equal(-1, result.Status);
            Assert.Contains("Direct Method() execution failed.", result.Results["Error"] as string);
        }

        [Fact(DisplayName = "No016_異常系：異常動作の確認_SetLogLevelに不正なJSON文字列を指定してメソッドを呼び出す")]
        public async Task Case016()
        {
            var request = new TICO.GAUDI.Commons.DirectMethodRequest("SetLogLevel", "invalid");

            var result = await TICO.GAUDI.Commons.DirectMethodCaller.Run(request);

            Assert.Equal(-1, result.Status);
            Assert.Contains("Bad request.", result.Results["Error"] as string);
        }

        [Fact(DisplayName = "No017_正常系：正常動作の確認_LogLevel未設定でGetLogLevelに空文字を指定してメソッドを呼び出す")]
        public async Task Case017()
        {
            var originalLogLevel = Environment.GetEnvironmentVariable("LogLevel");
            Environment.SetEnvironmentVariable("LogLevel", null);
            ResetLoggerInitialization();

            try
            {
                var request = new TICO.GAUDI.Commons.DirectMethodRequest("GetLogLevel", "");

                var result = await TICO.GAUDI.Commons.DirectMethodCaller.Run(request);

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