using System;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    public class DirectMethodCaller_GetRunner
    {
        [Fact(DisplayName = "No018_異常系：不正値の確認（string型）_methodNameにnullを指定してメソッドを呼び出す")]
        public void Case018()
        {
            var result = TestDirectMethodCaller.GetRunner((string)null!);

            Assert.Null(result);
        }

        [Fact(DisplayName = "No019_異常系：不正値の確認（string型）_methodNameに空文字を指定してメソッドを呼び出す")]
        public void Case019()
        {
            var result = TestDirectMethodCaller.GetRunner("");

            Assert.Null(result);
        }

        [Fact(DisplayName = "No020_異常系：異常動作の確認_methodNameにGetLogLevel、SetLogLevel以外の文字列を指定してメソッドを呼び出す")]
        public void Case020()
        {
            var exception = Assert.Throws<Exception>(() => TestDirectMethodCaller.GetRunner("UnknownMethod"));

            Assert.Contains("GetRunner failed.", exception.Message);
        }

        [Fact(DisplayName = "No021_正常系：正常動作の確認_methodNameにGetLogLevelを指定してメソッドを呼び出す")]
        public void Case021()
        {
            var result = TestDirectMethodCaller.GetRunner("GetLogLevel");

            Assert.IsType<TICO.GAUDI.Commons.DirectMethod_GetLogLevel>(result);
        }

        [Fact(DisplayName = "No022_正常系：正常動作の確認_methodNameにSetLogLevelを指定してメソッドを呼び出す")]
        public void Case022()
        {
            var result = TestDirectMethodCaller.GetRunner("SetLogLevel");

            Assert.IsType<TICO.GAUDI.Commons.DirectMethod_SetLogLevel>(result);
        }

        private sealed class TestDirectMethodCaller : TICO.GAUDI.Commons.DirectMethodCaller
        {
            public new static TICO.GAUDI.Commons.IDirectMethodRunner GetRunner(string methodName)
            {
                return TICO.GAUDI.Commons.DirectMethodCaller.GetRunner(methodName);
            }
        }
    }
}