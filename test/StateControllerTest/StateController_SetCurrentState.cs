using Xunit;
using Xunit.Abstractions;
using System.Reflection;
using System.Collections.Concurrent;

namespace TICO.GAUDI.Commons.Test.StateControllerTest;

[Collection(nameof(StateController_SetCurrentState))]
public class StateController_SetCurrentState
{
    private readonly ITestOutputHelper _output;

    public StateController_SetCurrentState(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "StateController No010_同一状態への遷移を設定するテスト")]
    public void StateController_Case010()
    {
       // Arrange: StateControllerインスタンスを作成し、初期状態を設定
        var stateController = new StateController();

        var tasksInCurrentStateField = stateController.GetType().GetField("tasksInCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);
        tasksInCurrentStateField!.SetValue(stateController, 1);

        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Start);

        var setCurrentStateMethod = stateController.GetType().GetMethod("SetCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);
        // Act: SetCurrentStateメソッドを呼び出す
        setCurrentStateMethod!.Invoke(stateController, new object[] { ApplicationState.Start });

        // フィールド tasksInCurrentState の値を取得
        var updatedTasksInCurrentState = tasksInCurrentStateField.GetValue(stateController);

        // Assert: テスト結果を確認
        Assert.Equal(ApplicationState.Start, stateController.CurrentState);
        Assert.Equal(2, updatedTasksInCurrentState);
    }

    [Fact(DisplayName = "StateController No011_異なる状態への遷移を設定するテスト")]
    public void StateController_Case011()
    {
        // Arrange: StateControllerインスタンスを作成し、初期状態を設定
        var stateController = new StateController();

        // プライベートフィールド `tasksInCurrentState` を設定し、初期値を 2 に変更
        var tasksInCurrentStateField = stateController.GetType().GetField("tasksInCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);
        // tasksInCurrentState を 2 に設定する
        tasksInCurrentStateField!.SetValue(stateController, 2);

        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Start);

        var setCurrentStateMethod = stateController.GetType().GetMethod("SetCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: SetCurrentStateメソッドを呼び出す
        setCurrentStateMethod!.Invoke(stateController, new object[] { ApplicationState.Ready });

        // フィールド tasksInCurrentState の値を取得
        var updatedTasksInCurrentState = tasksInCurrentStateField.GetValue(stateController);

        // Assert: テスト結果を確認
        Assert.Equal(ApplicationState.Start, stateController.CurrentState);
        Assert.Equal(1, updatedTasksInCurrentState);
    }

    [Fact(DisplayName = "StateController No012_異なる状態への遷移を設定するテスト")]
    public void StateController_Case012()
    {
        // Arrange: StateControllerインスタンスを作成し、初期状態を設定
        var stateController = new StateController();

        // フィールド tasksInCurrentState の存在確認
        var tasksInCurrentStateField = stateController.GetType().GetField("tasksInCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);
        // tasksInCurrentState を 1 に設定する
        tasksInCurrentStateField!.SetValue(stateController, 1);

        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Start);
        
        var setCurrentStateMethod = stateController.GetType().GetMethod("SetCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: SetCurrentStateメソッドを呼び出して状態遷移
        setCurrentStateMethod!.Invoke(stateController, new object[] { ApplicationState.Ready });

        // フィールド tasksInCurrentState の値を取得
        var updatedTasksInCurrentState = tasksInCurrentStateField.GetValue(stateController);

        // Assert: テスト結果の検証
        Assert.Equal(ApplicationState.Ready, stateController.CurrentState);
        Assert.Equal(1, updatedTasksInCurrentState);
    }

    [Fact(DisplayName = "StateController No013_後処理: ApplicationState.Start のテスト")]
    public void StateController_Case013()
    {
        // Arrange: StateControllerインスタンスを作成し、状態をカスタマイズ
        var stateController = new StateController();

        // プライベートフィールド関連を設定
        var tasksInCurrentField = stateController.GetType().GetField("tasksInCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);
        tasksInCurrentField!.SetValue(stateController, 1);

        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Ready);

        var setCurrentStateMethod = stateController.GetType().GetMethod("SetCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: SetCurrentStateメソッドを呼び出して、次の状態をApplicationState.Startに設定
        setCurrentStateMethod!.Invoke(stateController, new object[] { ApplicationState.Start });
        
        // Get updated tasksInCurrentState and currentState
        var updatedTasksInCurrentState = tasksInCurrentField.GetValue(stateController);

        // Assert: テスト結果を検証
        Assert.Equal(ApplicationState.Start, stateController.CurrentState);
        Assert.Equal(1, updatedTasksInCurrentState);
    }

    [Fact(DisplayName = "StateController No014_後処理: ApplicationState.Ready のテスト")]
    public void StateController_Case014()
    {
        // Arrange: StateControllerインスタンスを作成
        var stateController = new StateController();

        // Reflectionを使用してプライベートフィールドを設定
        var tasksInCurrentField = stateController.GetType().GetField("tasksInCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);
        tasksInCurrentField!.SetValue(stateController, 1);

        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Start);

        var setCurrentStateMethod = stateController.GetType().GetMethod("SetCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: SetCurrentStateメソッドを呼び出して、次の状態としてApplicationState.Readyを設定
        setCurrentStateMethod!.Invoke(stateController, new object[] { ApplicationState.Ready });

        // Get updated tasksInCurrentState and currentState
        var updatedTasksInCurrentState = tasksInCurrentField.GetValue(stateController);

        // Assert: 結果を検証
        Assert.Equal(ApplicationState.Ready, stateController.CurrentState);
        Assert.Equal(1, updatedTasksInCurrentState);
    }

    [Fact(DisplayName = "StateController No015_後処理: ApplicationState.Running のテスト")]
    public void StateController_Case015()
    {
        // Arrange: StateControllerインスタンスの作成と初期設定
        var stateController = new StateController();

        // プライベートフィールドの設定
        var tasksInCurrentField = stateController.GetType().GetField("tasksInCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);
        tasksInCurrentField!.SetValue(stateController, 1);

        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Ready);

        var setCurrentStateMethod = stateController.GetType().GetMethod("SetCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: SetCurrentStateメソッドを呼び出し、次の状態としてApplicationState.Runningを設定
        setCurrentStateMethod!.Invoke(stateController, new object[] { ApplicationState.Running });

        // Get updated tasksInCurrentState and currentState
        var updatedTasksInCurrentState = tasksInCurrentField.GetValue(stateController);

        // Assert: テスト結果を検証
        Assert.Equal(ApplicationState.Running, stateController.CurrentState);
        Assert.Equal(1, updatedTasksInCurrentState);
    }

    [Fact(DisplayName = "StateController No016_後処理: ApplicationState.End のテスト")]
    public void StateController_Case016()
    {
        // Arrange: StateControllerインスタンスを作成し、初期状態を設定
        var stateController = new StateController();

        // プライベートフィールドの設定
        var tasksInCurrentField = stateController.GetType().GetField("tasksInCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);
        tasksInCurrentField!.SetValue(stateController, 1);

        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Ready);

        var setCurrentStateMethod = stateController.GetType().GetMethod("SetCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: SetCurrentStateメソッドを呼び出し、次の状態としてApplicationState.Endを設定
        setCurrentStateMethod!.Invoke(stateController, new object[] { ApplicationState.End });

        // Get updated tasksInCurrentState and currentState
        var updatedTasksInCurrentState = tasksInCurrentField.GetValue(stateController);

        // Assert: テスト結果を検証
        Assert.Equal(ApplicationState.End, stateController.CurrentState);
        Assert.Equal(1, updatedTasksInCurrentState);
    }

    [Fact(DisplayName = "StateController No017_後処理: ApplicationState.Initialize のテスト")]
    public void StateController_Case017()
    {
        // Arrange: StateControllerインスタンスを作成
        var stateController = new StateController();

        // プライベートフィールドの設定
        var tasksInCurrentField = stateController.GetType().GetField("tasksInCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);
        tasksInCurrentField!.SetValue(stateController, 1);

        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Start);

        var isTerminatingField = stateController.GetType().GetField("isTerminating", BindingFlags.NonPublic | BindingFlags.Instance);
        isTerminatingField!.SetValue(stateController, true); // 初期値としてtrueを設定

        var stateQueueField = stateController.GetType().GetField("stateQueue", BindingFlags.NonPublic | BindingFlags.Instance);
        var stateQueue = new ConcurrentQueue<string>();
        stateQueue.Enqueue("TestQueueItem"); // キューにアイテムを追加
        stateQueueField!.SetValue(stateController, stateQueue);

        var setCurrentStateMethod = stateController.GetType().GetMethod("SetCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: SetCurrentStateメソッドを呼び出し、次の状態としてApplicationState.Initializeを設定
        setCurrentStateMethod!.Invoke(stateController, new object[] { ApplicationState.Initialize });

        // Get updated stateController properties
        var updatedTasksInCurrentState = tasksInCurrentField.GetValue(stateController);
        var isTerminating = isTerminatingField.GetValue(stateController);
        var updatedStateQueue = stateQueueField.GetValue(stateController) as ConcurrentQueue<string>;

        // Assert: 結果を検証
        Assert.Equal(ApplicationState.Initialize, stateController.CurrentState);
        Assert.Equal(1, updatedTasksInCurrentState);
        Assert.False((bool)isTerminating!);
        Assert.Empty(updatedStateQueue);
    }

    [Fact(DisplayName = "StateController No018_後処理: ApplicationState.Terminate のテスト")]
    public void StateController_Case018()
    {
        // Arrange: StateControllerインスタンスを作成
        var stateController = new StateController();

        // プライベートフィールドの設定
        var tasksInCurrentField = stateController.GetType().GetField("tasksInCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);
        tasksInCurrentField!.SetValue(stateController, 1);

        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Running);

        var stateQueueField = stateController.GetType().GetField("stateQueue", BindingFlags.NonPublic | BindingFlags.Instance);
        var stateQueue = new ConcurrentQueue<string>();
        stateQueue.Enqueue("TestQueueItem");
        stateQueueField!.SetValue(stateController, stateQueue);

        var setCurrentStateMethod = stateController.GetType().GetMethod("SetCurrentState", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: SetCurrentStateメソッドを呼び出す
        setCurrentStateMethod!.Invoke(stateController, new object[] { ApplicationState.Terminate });

        // Get updated stateController properties
        var updatedTasksInCurrentState = tasksInCurrentField.GetValue(stateController);
        var updatedStateQueue = stateQueueField.GetValue(stateController) as ConcurrentQueue<string>;

        // Assert: テスト結果を検証
        Assert.Equal(ApplicationState.Terminate, stateController.CurrentState);
        Assert.Equal(1, updatedTasksInCurrentState);
        Assert.Empty(updatedStateQueue);
    }
}
