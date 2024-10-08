using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(StateController_StateController))]
    [CollectionDefinition(nameof(StateController_StateController), DisableParallelization = true)]
    public class StateController_StateController
    {
        private readonly ITestOutputHelper _output;

        public StateController_StateController(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "正常系：初期ステートStart")]
        public void MethodNameNullInput_ExecutionFailedReturned()
        {
            StateController target = new StateController();

            Assert.Equal(ApplicationState.Start, target.CurrentState);
        }


    }
}
