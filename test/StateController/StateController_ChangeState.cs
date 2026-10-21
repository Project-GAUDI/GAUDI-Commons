using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    public class StateController_ChangeState
    {
        [Fact(DisplayName = "No031_正常系：正常動作の確認_currentState=StartでnextState=Runningを指定する")]
        public void Case031()
        {
            var testInstance = new TICO.GAUDI.Commons.StateController();

            var result = testInstance.ChangeState(TICO.GAUDI.Commons.ApplicationState.Running);

            Assert.Equal(TICO.GAUDI.Commons.ApplicationStateChangeResult.Ignored, result);
            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.Start, testInstance.CurrentState);
        }

        [Fact(DisplayName = "No032_正常系：正常動作の確認_currentState=StartでnextState=Initializeを指定する")]
        public void Case032()
        {
            var testInstance = new TICO.GAUDI.Commons.StateController();

            var result = testInstance.ChangeState(TICO.GAUDI.Commons.ApplicationState.Initialize);

            Assert.Equal(TICO.GAUDI.Commons.ApplicationStateChangeResult.Success, result);
            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.Initialize, testInstance.CurrentState);
        }

        [Fact(DisplayName = "No033_正常系：正常動作の確認_currentState=ReadyでnextState=Terminateを指定する")]
        public void Case033()
        {
            var testInstance = new TICO.GAUDI.Commons.StateController();
            testInstance.ChangeState(TICO.GAUDI.Commons.ApplicationState.Initialize);
            testInstance.ChangeState(TICO.GAUDI.Commons.ApplicationState.Ready);

            var result = testInstance.ChangeState(TICO.GAUDI.Commons.ApplicationState.Terminate);

            Assert.Equal(TICO.GAUDI.Commons.ApplicationStateChangeResult.Success, result);
            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.Terminate, testInstance.CurrentState);
            Assert.True(testInstance.IsTerminating);
        }

        [Fact(DisplayName = "No034_正常系：正常動作の確認_currentState=TerminateでnextState=Runningを指定する")]
        public void Case034()
        {
            var testInstance = new TICO.GAUDI.Commons.StateController();
            testInstance.ChangeState(TICO.GAUDI.Commons.ApplicationState.Initialize);
            testInstance.ChangeState(TICO.GAUDI.Commons.ApplicationState.Ready);
            testInstance.ChangeState(TICO.GAUDI.Commons.ApplicationState.Terminate);

            var result = testInstance.ChangeState(TICO.GAUDI.Commons.ApplicationState.Running);

            Assert.Equal(TICO.GAUDI.Commons.ApplicationStateChangeResult.Ignored, result);
            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.Terminate, testInstance.CurrentState);
        }
    }
}