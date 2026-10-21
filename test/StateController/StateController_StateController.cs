using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    public class StateController_StateController
    {
        [Fact(DisplayName = "No030_正常系：正常動作の確認_初期化する")]
        public void Case030()
        {
            var result = new TICO.GAUDI.Commons.StateController();

            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.Start, result.CurrentState);
            Assert.False(result.IsTerminating);
        }
    }
}