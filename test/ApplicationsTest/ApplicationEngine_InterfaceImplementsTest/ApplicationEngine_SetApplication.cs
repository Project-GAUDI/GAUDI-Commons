using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    public class ApplicationEngine_SetApplication
    {
        private class ApplicationEngineTester : ApplicationEngine
        {
            public IApplicationMain GetApplicationMain()
            {
                return this.applicationMain;
            }
        }

        private readonly ITestOutputHelper _output;

        public ApplicationEngine_SetApplication(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact(DisplayName = "正常系:任意のインスタンスをセット")]
        public void MainSet_CanGetInstance()
        {
            ApplicationEngineTester tester = new ApplicationEngineTester();
            IApplicationEngine engine = tester;
            IApplicationMain appMain = new StubApplicationMain();
            engine.SetApplication(appMain);
            Assert.NotNull(tester.GetApplicationMain());
            Assert.Same(tester.GetApplicationMain(), appMain);
        }

        [Fact(DisplayName = "未設定")]
        public void NotSet_Null()
        {
            ApplicationEngineTester tester = new ApplicationEngineTester();
            IApplicationEngine engine = tester;
            // engine.SetApplication(null);
            Assert.Null(tester.GetApplicationMain());
        }

        [Fact(DisplayName = "null設定")]
        public void NullSet_Null()
        {
            ApplicationEngineTester tester = new ApplicationEngineTester();
            IApplicationEngine engine = tester;
            engine.SetApplication(null);
            Assert.Null(tester.GetApplicationMain());
        }
    }
}
