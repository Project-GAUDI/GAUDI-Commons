using System;
using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    public class ApplicationEngine_RunAsync
    {
        internal class ApplicationEngineTester : StubApplicationEngine
        {
        }

        private readonly ITestOutputHelper _output;

        public ApplicationEngine_RunAsync(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Application未設定")]
        public async void NotSetApplication_ExceptionThrown()
        {
            ApplicationEngineTester tester = new ApplicationEngineTester();
            IApplicationEngine engine = tester;

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await engine.RunAsync();
            });

            Assert.Equal("Error: ApplicationMain not set.", exception.Message);
        }

        [Fact(DisplayName = "Application設定済み")]
        public async void Normal_Success()
        {
            ApplicationEngineTester tester = new ApplicationEngineTester();
            IApplicationEngine engine = tester;
            engine.SetApplication(new StubApplicationMain());

            // Exceptionが起きない
            await engine.RunAsync();
        }
    }
}
