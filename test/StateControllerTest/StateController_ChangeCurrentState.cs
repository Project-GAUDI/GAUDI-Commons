using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(StateController_ChangeCurrentState))]
    [CollectionDefinition(nameof(StateController_ChangeCurrentState), DisableParallelization = true)]
    public class StateController_ChangeCurrentState
    {
        private readonly ITestOutputHelper _output = null;
        private StateController _target = null;
        static private Object counterLock = new Object();
        private int _started = 0;
        private int _finished = 0;

        public StateController_ChangeCurrentState(ITestOutputHelper output)
        {
            _output = output;
            _target = new StateController();

            lock (counterLock)
            {
                _started = 0;
                _finished = 0;
            }
        }

        [Theory(DisplayName = "Startからの遷移")]
        [InlineData(ApplicationState.Start, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Initialize, ApplicationStateChangeResult.Success)]
        [InlineData(ApplicationState.Ready, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Running, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Terminate, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.End, ApplicationStateChangeResult.Ignored)]
        public void StartToX(ApplicationState nextState, ApplicationStateChangeResult requiredResult)
        {
            // 初期設定
            // なし

            // テスト実行
            DoTest(ApplicationState.Start, nextState, requiredResult);
        }

        [Theory(DisplayName = "Initializeからの遷移")]
        [InlineData(ApplicationState.Start, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Initialize, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Ready, ApplicationStateChangeResult.Success)]
        [InlineData(ApplicationState.Running, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Terminate, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.End, ApplicationStateChangeResult.Ignored)]
        public void InitializeToX(ApplicationState nextState, ApplicationStateChangeResult requiredResult)
        {
            // 初期設定
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Initialize));  // Start -> Initialize

            // テスト実行
            DoTest(ApplicationState.Initialize, nextState, requiredResult);

        }

        [Theory(DisplayName = "Readyからの遷移")]
        [InlineData(ApplicationState.Start, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Initialize, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Ready, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Running, ApplicationStateChangeResult.Success)]
        [InlineData(ApplicationState.Terminate, ApplicationStateChangeResult.Success)]
        [InlineData(ApplicationState.End, ApplicationStateChangeResult.Ignored)]
        public void ReadyToX(ApplicationState nextState, ApplicationStateChangeResult requiredResult)
        {
            // 初期設定
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Initialize));  // Start -> Initialize
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Ready));  // Initialize -> Ready

            // テスト実行
            DoTest(ApplicationState.Ready, nextState, requiredResult);

        }

        [Theory(DisplayName = "Runningからの遷移")]
        [InlineData(ApplicationState.Start, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Initialize, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Ready, ApplicationStateChangeResult.Success)]
        [InlineData(ApplicationState.Running, ApplicationStateChangeResult.Success)]
        [InlineData(ApplicationState.Terminate, ApplicationStateChangeResult.Success)]
        [InlineData(ApplicationState.End, ApplicationStateChangeResult.Ignored)]
        public async Task RunningToX(ApplicationState nextState, ApplicationStateChangeResult requiredResult)
        {
            // 初期設定
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Initialize));  // Start -> Initialize
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Ready));  // Initialize -> Ready

            // 非同期でRunnningに遷移＋キューエントリ待ち後、Readyに戻す
            var preTask = Task.Run(ToRunningAndReady);

            // テスト実行
            while (_started <= 0)
            {
                await Task.Delay(100);
            }
            DoTest(ApplicationState.Running, nextState, requiredResult);

            await preTask;

            // 終了待ち
            await Task.Delay(1500);

        }

        [Theory(DisplayName = "Terminateからの遷移")]
        [InlineData(ApplicationState.Start, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Initialize, ApplicationStateChangeResult.Success)]
        [InlineData(ApplicationState.Ready, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Running, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Terminate, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.End, ApplicationStateChangeResult.Success)]
        public void TerminateToX(ApplicationState nextState, ApplicationStateChangeResult requiredResult)
        {
            // 初期設定
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Initialize));  // Start -> Initialize
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Ready));  // Initialize -> Ready
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Terminate));  // Ready -> Terminate

            // テスト実行
            DoTest(ApplicationState.Terminate, nextState, requiredResult);

        }

        [Theory(DisplayName = "Endからの遷移")]
        [InlineData(ApplicationState.Start, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Initialize, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Ready, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Running, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.Terminate, ApplicationStateChangeResult.Ignored)]
        [InlineData(ApplicationState.End, ApplicationStateChangeResult.Ignored)]
        public void EndToX(ApplicationState nextState, ApplicationStateChangeResult requiredResult)
        {
            // 初期設定
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Initialize));  // Start -> Initialize
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Ready));  // Initialize -> Ready
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Terminate));  // Ready -> Terminate
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.End));  // Ready -> End

            // テスト実行
            DoTest(ApplicationState.End, nextState, requiredResult);

        }

        [Fact(DisplayName = "Running中のキューがエントリ順に処理されるか")]
        public async Task RunningToReady_AllQeueuedDequeued()
        {
            // 初期設定
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Initialize));  // Start -> Initialize
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Ready));  // Initialize -> Ready

            // 非同期でRunnningに遷移＋キューエントリ待ち後、Readyに戻す
            var preTask = Task.Run(ToRunningAndReady);

            // テスト実行
            await Task.Delay(100);

            List<Task> tasks = new List<Task>();
            for (int icnt = 1; icnt <= 3; icnt++)
            {
                var required = icnt;
                var task = Task.Run(
                    async () =>
                    {
                        await ToRunningAndReadyMini();
                        Assert.Equal(required, _finished);
                    }
                );

                tasks.Add(task);
                await Task.Delay(50);
            }

            // Runningは並列可能なので、開始済みになっている想定
            Assert.Equal(4, _started);
            Assert.NotEqual(4, _finished);

            // 終了待ち
            await preTask;
            Task.WaitAll(tasks.ToArray());

            Assert.Equal(4, _finished);
        }

        [Fact(DisplayName = "Running中のキューにTerminateがエントリされた場合、その後のリクエストはIgnoreされるか？")]
        public async Task RunningToReady_IgnoredAfterTerminateEntry()
        {
            // 初期設定
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Initialize));  // Start -> Initialize
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Ready));  // Initialize -> Ready

            // 非同期でRunnningに遷移＋キューエントリ待ち後、Readyに戻す
            var preTask = Task.Run(ToRunningAndReady);

            // テスト実行
            while (_started <= 0)
            {
                await Task.Delay(100);
            }

            // 先にエントリするキュー
            List<Task> tasks = new List<Task>();

            for (int icnt = 1; icnt <= 3; icnt++)
            {
                var required = icnt;
                var task = Task.Run(
                    async () =>
                    {
                        await ToRunningAndReadyMini();
                        Assert.Equal(required, _finished);
                    }
                );

                tasks.Add(task);
                await Task.Delay(50);
            }

            // 開始済みが４になるまで待機
            while (_started < 4)
            {
                await Task.Delay(100);
            }

            // Terminateをキュー
            var termTask = Task.Run(
                async () =>
                {
                    Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Terminate));  // Ready -> Terminate
                    await Task.Delay(100);
                    Assert.Equal(4, _finished);
                }
            );
            tasks.Add(termTask);
            await Task.Delay(100);

            // 後からエントリするキュー
            for (int icnt = 4; icnt <= 6; icnt++)
            {
                var required = icnt;
                var task = Task.Run(
                    async () =>
                    {
                        await ToRunningAndReadyMiniIgnored();
                        await Task.Delay(10);
                        Assert.True(_started <= 4);
                    }
                );

                tasks.Add(task);
                await Task.Delay(10);
            }


            // 終了待ち
            await preTask;
            Task.WaitAll(tasks.ToArray());

            Assert.Equal(4, _finished);

        }




        /// <summary>
        /// テスト実処理
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="nextState"></param>
        /// <param name="requiredResult"></param>
        protected void DoTest(ApplicationState currentState, ApplicationState nextState, ApplicationStateChangeResult requiredResult)
        {
            var current = _target.CurrentState;
            Assert.Equal(currentState, current);

            var result = _target.ChangeState(nextState);

            Assert.Equal(requiredResult, result);
            if (ApplicationStateChangeResult.Success == requiredResult)
            {
                // ステート遷移済み
                Assert.Equal(nextState, _target.CurrentState);
            }
            else if (ApplicationStateChangeResult.Ignored == requiredResult)
            {
                // ステート遷移なし
                Assert.Equal(current, _target.CurrentState);
            }
        }

        protected async Task ToRunningAndReady()
        {
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Running));  // Ready -> Running

            lock (counterLock) { _started++; }

            await Task.Delay(1000);

            lock (counterLock) { _finished++; }

            // ここは失敗OKとする。（テスト側の遷移が成功(Running->Readyなど)するとこちらは失敗になる為）
            _target.ChangeState(ApplicationState.Ready);  // Running -> Ready

        }

        protected async Task ToRunningAndReadyMini()
        {
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Running));  // Ready -> Running
            lock (counterLock) { _started++; }

            await Task.Delay(50);

            lock (counterLock) { _finished++; }
            Assert.Equal(ApplicationStateChangeResult.Success, _target.ChangeState(ApplicationState.Ready));  // Running -> Ready
        }

        protected async Task ToRunningAndReadyMiniIgnored()
        {
            Assert.Equal(ApplicationStateChangeResult.Ignored, _target.ChangeState(ApplicationState.Running));  // Ready -> Running
            await Task.Delay(50);
        }

    }
}
