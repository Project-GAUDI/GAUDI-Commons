using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.StateControllerTest;

[Collection(nameof(StateController_StateController))]
public class StateController_StateController
{
    private readonly ITestOutputHelper _output;

    public StateController_StateController(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact(DisplayName = "StateController No001_コンストラクタのテスト")]
    public void StateController_Case001()
    {
        // Arrange: StateControllerインスタンスを作成
        var stateController = new StateController();

        // Assert: 初期状態が正しいことを確認
        Assert.Equal(ApplicationState.Start, stateController.CurrentState);
        Assert.False(stateController.IsTerminating);
    }
}
/* 単体テスト仕様書外の既存テストをコメントアウト
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
*/