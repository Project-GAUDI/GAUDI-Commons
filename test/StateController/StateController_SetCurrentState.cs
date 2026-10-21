using System.Collections.Concurrent;
using System.Reflection;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    public class StateController_SetCurrentState
    {
        [Fact(DisplayName = "No035_正常系：正常動作の確認_currentState=Start、tasksInCurrentState=1、isTerminating=true、キューありでStartを指定する")]
        public void Case035()
        {
            var testInstance = CreateController(TICO.GAUDI.Commons.ApplicationState.Start, 1, true, true);

            testInstance.SetState(TICO.GAUDI.Commons.ApplicationState.Start);

            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.Start, testInstance.CurrentState);
            Assert.Equal(2, testInstance.TaskCount);
            Assert.True(testInstance.IsTerminating);
            Assert.Equal(1, testInstance.QueueCount);
        }

        [Fact(DisplayName = "No036_正常系：正常動作の確認_currentState=Start、tasksInCurrentState=1、isTerminating=true、キューありでInitializeを指定する")]
        public void Case036()
        {
            var testInstance = CreateController(TICO.GAUDI.Commons.ApplicationState.Start, 1, true, true);

            testInstance.SetState(TICO.GAUDI.Commons.ApplicationState.Initialize);

            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.Initialize, testInstance.CurrentState);
            Assert.Equal(1, testInstance.TaskCount);
            Assert.False(testInstance.IsTerminating);
            Assert.Equal(0, testInstance.QueueCount);
        }

        [Fact(DisplayName = "No037_正常系：正常動作の確認_currentState=Initialize、tasksInCurrentState=1でReadyを指定する")]
        public void Case037()
        {
            var testInstance = CreateController(TICO.GAUDI.Commons.ApplicationState.Initialize, 1, false, false);

            testInstance.SetState(TICO.GAUDI.Commons.ApplicationState.Ready);

            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.Ready, testInstance.CurrentState);
            Assert.Equal(1, testInstance.TaskCount);
        }

        [Fact(DisplayName = "No038_正常系：正常動作の確認_currentState=Ready、tasksInCurrentState=1でRunningを指定する")]
        public void Case038()
        {
            var testInstance = CreateController(TICO.GAUDI.Commons.ApplicationState.Ready, 1, false, false);

            testInstance.SetState(TICO.GAUDI.Commons.ApplicationState.Running);

            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.Running, testInstance.CurrentState);
            Assert.Equal(1, testInstance.TaskCount);
        }

        [Fact(DisplayName = "No039_正常系：正常動作の確認_currentState=Running、tasksInCurrentState=1、キューありでTerminateを指定する")]
        public void Case039()
        {
            var testInstance = CreateController(TICO.GAUDI.Commons.ApplicationState.Running, 1, false, true);

            testInstance.SetState(TICO.GAUDI.Commons.ApplicationState.Terminate);

            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.Terminate, testInstance.CurrentState);
            Assert.Equal(1, testInstance.TaskCount);
            Assert.Equal(0, testInstance.QueueCount);
        }

        [Fact(DisplayName = "No040_正常系：正常動作の確認_currentState=Terminate、tasksInCurrentState=1でEndを指定する")]
        public void Case040()
        {
            var testInstance = CreateController(TICO.GAUDI.Commons.ApplicationState.Terminate, 1, false, false);

            testInstance.SetState(TICO.GAUDI.Commons.ApplicationState.End);

            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.End, testInstance.CurrentState);
            Assert.Equal(1, testInstance.TaskCount);
        }

        [Fact(DisplayName = "No041_正常系：正常動作の確認_currentState=Start、tasksInCurrentState=2、isTerminating=true、キューありでInitializeを指定する")]
        public void Case041()
        {
            var testInstance = CreateController(TICO.GAUDI.Commons.ApplicationState.Start, 2, true, true);

            testInstance.SetState(TICO.GAUDI.Commons.ApplicationState.Initialize);

            Assert.Equal(TICO.GAUDI.Commons.ApplicationState.Start, testInstance.CurrentState);
            Assert.Equal(1, testInstance.TaskCount);
            Assert.False(testInstance.IsTerminating);
            Assert.Equal(0, testInstance.QueueCount);
        }

        [Fact(DisplayName = "No042_正常系：正常動作の確認_currentState=Start、tasksInCurrentState=1で未定義の状態を指定する")]
        public void Case042()
        {
            var testInstance = CreateController(TICO.GAUDI.Commons.ApplicationState.Start, 1, false, false);
            var undefinedState = (TICO.GAUDI.Commons.ApplicationState)6;

            var exception = Record.Exception(() => testInstance.SetState(undefinedState));

            Assert.Null(exception);
            Assert.Equal(undefinedState, testInstance.CurrentState);
            Assert.Equal(1, testInstance.TaskCount);
        }

        private static TestStateController CreateController(
            TICO.GAUDI.Commons.ApplicationState currentState,
            int taskCount,
            bool isTerminating,
            bool hasQueueItem)
        {
            var testInstance = new TestStateController();
            testInstance.SetFields(currentState, taskCount, isTerminating, hasQueueItem);
            return testInstance;
        }

        private sealed class TestStateController : TICO.GAUDI.Commons.StateController
        {
            public int TaskCount => tasksInCurrentState;
            public int QueueCount => GetQueue().Count;

            public void SetFields(
                TICO.GAUDI.Commons.ApplicationState state,
                int taskCount,
                bool terminating,
                bool hasQueueItem)
            {
                currentState = state;
                tasksInCurrentState = taskCount;
                isTerminating = terminating;
                var queue = GetQueue();
                queue.Clear();
                if (hasQueueItem)
                {
                    queue.Enqueue("test");
                }
            }

            public void SetState(TICO.GAUDI.Commons.ApplicationState state)
            {
                SetCurrentState(state);
            }

            private ConcurrentQueue<string> GetQueue()
            {
                var field = typeof(TICO.GAUDI.Commons.StateController).GetField(
                    "stateQueue",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                return (ConcurrentQueue<string>)field!.GetValue(this)!;
            }
        }
    }
}