using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.StateControllerTest;

[Collection(nameof(StateController_ChangeState))]
public class StateController_ChangeState
{
    private readonly ITestOutputHelper _output;

    public StateController_ChangeState(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact(DisplayName = "StateController No002_自己遷移を設定する")]
    public void StateController_Case002()
    {
        // Arrange: StateControllerを初期化
        var stateController = new StateController();

        // Act: 現在の状態が ApplicationState.Start のとき、次の状態を ApplicationState.Start に変更
        var result = stateController.ChangeState(ApplicationState.Start);

        // Assert: ApplicationStateChangeResult.Ignored が返されることを検証
        Assert.Equal(ApplicationStateChangeResult.Ignored, result);
    }

    [Fact(DisplayName = "StateController No003_正常遷移を設定する")]
    public void StateController_Case003()
    {
       // Arrange: StateControllerを初期化
        var stateController = new StateController();

        // Act: 現在の状態が ApplicationState.Start のとき、次の状態を ApplicationState.Initialize に遷移
        var result = stateController.ChangeState(ApplicationState.Initialize);

        // Assert: ApplicationStateChangeResult.Success が返されることを検証
        Assert.Equal(ApplicationStateChangeResult.Success, result);
        
        // Assert: 現在の状態が ApplicationState.Initialize に変更されていることを確認
        Assert.Equal(ApplicationState.Initialize, stateController.CurrentState);
    }

    [Fact(DisplayName = "StateController No004_未許可な状態遷移を設定する")]
    public void StateController_Case004()
    {
        // Arrange: StateControllerを初期化
        var stateController = new StateController();

        // Act: 現在の状態を ApplicationState.Start のままで、次の状態を ApplicationState.Ready に遷移しようとする
        var result = stateController.ChangeState(ApplicationState.Ready);

        // Assert: ApplicationStateChangeResult.Ignored が返されることを確認
        Assert.Equal(ApplicationStateChangeResult.Ignored, result);

        // Assert: 現在の状態は変更されない（ApplicationState.Start のままであることを確認）
        Assert.Equal(ApplicationState.Start, stateController.CurrentState);
    }

    [Fact(DisplayName = "StateController No005_キューイング可能な状態遷移を設定する")]
    public void StateController_Case005()
    {
        // Arrange: StateControllerを初期化し、現在の状態を ApplicationState.Ready に設定する
        var stateController = new StateController();

        // 現在の状態を Ready にするため、正常遷移を実行
        var initializeResult = stateController.ChangeState(ApplicationState.Initialize);
        Assert.Equal(ApplicationStateChangeResult.Success, initializeResult);

        var readyResult = stateController.ChangeState(ApplicationState.Ready);
        Assert.Equal(ApplicationStateChangeResult.Success, readyResult);

        // Act: 現在の状態が Ready のとき、次の状態を Terminate に設定
        var terminateResult = stateController.ChangeState(ApplicationState.Terminate);

        // Assert: ApplicationStateChangeResult.Success が返されることを確認
        Assert.Equal(ApplicationStateChangeResult.Success, terminateResult);

        // Assert: 現在の状態が Terminate に変更されていることを確認
        Assert.Equal(ApplicationState.Terminate, stateController.CurrentState);
    }

    [Fact(DisplayName = "StateController No006_キューイング可能な状態遷移を設定する")]
    public async Task StateController_Case006()
    {
        // Arrange: StateControllerを初期化し、現在の状態を ApplicationState.Running に設定する
        var stateController = new StateController();

        var initializeResult = stateController.ChangeState(ApplicationState.Initialize);
        Assert.Equal(ApplicationStateChangeResult.Success, initializeResult);

        var readyResult = stateController.ChangeState(ApplicationState.Ready);
        Assert.Equal(ApplicationStateChangeResult.Success, readyResult);

        var runningResult = stateController.ChangeState(ApplicationState.Running);
        Assert.Equal(ApplicationStateChangeResult.Success, runningResult);

        // フィールドを直接操作するためのリフレクションを取得 (currentStateを変更するため)
        var currentStateField = typeof(StateController).GetField("currentState", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(currentStateField);

        // Act: 非同期で ChangeState を呼び出し、別スレッドで currentState に Ready を設定
        var result = ApplicationStateChangeResult.Ignored;

        // 特殊な処理を行う：Terminate への遷移を非同期で試みるタスク
        var terminateTask = Task.Run(() =>
        {
            // Terminate への遷移を試みる
            // 【重要】このメソッド内でループ処理に入った後、状態変更が行われるタイミングを制御し、
            // CurrentState が最終的に Terminate になることを保証する
            result = stateController.ChangeState(ApplicationState.Terminate);
        });

        // 別スレッドで currentState を Ready に変更するタスク
        var changeStateTask = Task.Run(async () =>
        {
            // 1000ms後に currentState を Ready に変更
            // 【重要】この操作は、ループ処理に入った後にステートを変更していることを確認する
            // これにより、ループ内で正しい状態遷移が行われるかを検証する
            await Task.Delay(1000);
            currentStateField!.SetValue(stateController, ApplicationState.Ready);
        });

        // 両タスクを待機
        await Task.WhenAll(terminateTask, changeStateTask);

        // Assert: Terminate の成功チェック
        // 【重要】CurrentState が Terminate に設定され、ChangeState 内のループ処理が通過したことを確認
        // このチェックにより、ループ処理が正しく動作したことが保証する
        Assert.Equal(ApplicationStateChangeResult.Success, result);

        // Assert: 現在の状態が Terminate に変更されていることを確認
        // 【重要】CurrentState の値が Terminate に設定されていることを通じて、ループ処理とステート変更が完了したことを確認する
        Assert.Equal(ApplicationState.Terminate, stateController.CurrentState);
    }

    [Fact(DisplayName = "StateController No007_transitableWhenTerminating が有効な状態遷移を設定する")]
    public async Task StateController_Case007()
    {
        // Arrange: StateControllerを初期化
        var stateController = new StateController();

        // currentState をリフレクションで Terminate に設定
        var currentStateField = stateController.GetType().GetField("currentState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(currentStateField); // リフレクションフィールドが取得できたか確認
        currentStateField!.SetValue(stateController, ApplicationState.Terminate);

        // Act: End への遷移を試みる
        var endResult = stateController.ChangeState(ApplicationState.End);

        // Assert: 結果が成功であることを確認
        Assert.Equal(ApplicationStateChangeResult.Success, endResult);

        // Assert: 現在の状態が End になっていることを確認
        Assert.Equal(ApplicationState.End, stateController.CurrentState);

        await Task.CompletedTask;
    }

    [Fact(DisplayName = "StateController No008_特例遷移以外の状態遷移を設定する")]
    public void StateController_Case008()
    {
        // Arrange: StateController を初期化
        var stateController = new StateController();

        // currentState をリフレクションを使って直接 Terminate に設定
        var currentStateField = typeof(StateController).GetField("currentState",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(currentStateField); // リフレクションフィールドが取得できたか確認
        currentStateField!.SetValue(stateController, ApplicationState.Terminate);

        // Act: Ready への遷移を試みる（特例遷移ではない状態の確認）
        var readyResultFromTerminate = stateController.ChangeState(ApplicationState.Ready);

        // Assert: Ready への遷移結果が Ignored である (遷移が無視される) ことを確認
        Assert.Equal(ApplicationStateChangeResult.Ignored, readyResultFromTerminate);

        // Assert: 現在の状態が Terminate のままであることを確認
        Assert.Equal(ApplicationState.Terminate, stateController.CurrentState);
    }
    
    [Fact(DisplayName = "StateController No009_複数スレッドから同時にChangeStateを呼び出す", Skip ="競合状態が発生せず、同期的に処理が実行されたことの確認が不可能なためスキップ")]
    public async Task StateController_Case009()
    {
        // StateController 初期化
        var stateController = new StateController();

        // 初期状態を Ready に設定して、競合を減少
        stateController.ChangeState(ApplicationState.Start);
        stateController.ChangeState(ApplicationState.Ready);

        const int ThreadCount = 10; // スレッド数
        var tasks = new Task[ThreadCount];
        var stateHistory = new List<ApplicationState>();
        var resultHistory = new List<ApplicationStateChangeResult>();
        var historyLock = new object();

        async Task PerformStateChange(int threadId)
        {
            var nextState = threadId % 2 == 0 ? ApplicationState.Initialize : ApplicationState.Ready;
            var result = await Task.Run(() => stateController.ChangeState(nextState));

            lock (historyLock)
            {
                resultHistory.Add(result);
                stateHistory.Add(stateController.CurrentState);

                // ログを出力してデバッグ用情報を記録
                _output.WriteLine($"Thread {threadId}: Requested {nextState}, Result {result}, CurrentState {stateController.CurrentState}");
            }

            Assert.True(result == ApplicationStateChangeResult.Success || result == ApplicationStateChangeResult.Ignored, 
                $"Unexpected result on thread {threadId}: {result}");
        }

        for (int i = 0; i < ThreadCount; i++)
        {
            tasks[i] = PerformStateChange(i);
        }

        // タイムアウト設定
        var timeoutTask = Task.Delay(5000);
        var allTasks = Task.WhenAll(tasks);
        var completedTask = await Task.WhenAny(allTasks, timeoutTask);
        Assert.True(completedTask == allTasks, "Test timed out.");

        // Final 状態確認
        lock (historyLock)
        {
            Assert.Contains(ApplicationState.Initialize, stateHistory);
            Assert.Contains(ApplicationState.Ready, stateHistory);
        }

        Assert.Equal(ThreadCount, resultHistory.Count);
    }
}
