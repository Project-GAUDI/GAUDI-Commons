using Xunit;
using Xunit.Abstractions;
using System.Reflection;

namespace TICO.GAUDI.Commons.Test.StateControllerTest;

[Collection(nameof(StateController_GetTransitable))]
public class StateController_GetTransitable
{
    private readonly ITestOutputHelper _output;

    public StateController_GetTransitable(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "StateController No019_正常遷移の設定のテスト")]
    public void StateController_Case019()
    {
        // Arrange: StateControllerインスタンスを作成
        var stateController = new StateController();

        // Reflectionを使用してGetTransitableメソッドを取得
        var getTransitableMethod = stateController.GetType().GetMethod("GetTransitable", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: GetTransitableメソッドを呼び出し
        var result = getTransitableMethod!.Invoke(stateController, new object[] { ApplicationState.Initialize });

        // Assert: 結果を検証
        Assert.Equal(StateTransitable.Transitable, (StateTransitable)result!);
    }

    [Fact(DisplayName = "StateController No020_未定義の遷移を設定するテスト")]
    public void StateController_Case020()
    {
        // Arrange: StateControllerインスタンスを作成
        var stateController = new StateController();

        // Reflectionを使用してGetTransitableメソッドを取得
        var getTransitableMethod = stateController.GetType().GetMethod("GetTransitable", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: GetTransitableメソッドを呼び出し
        var result = getTransitableMethod!.Invoke(stateController, new object[] { ApplicationState.Ready });

        // Assert: 結果を検証
        Assert.Equal(StateTransitable.Undefined, (StateTransitable)result!);
    }

    [Fact(DisplayName = "StateController No021_transitableWhenTerminatingが有効な遷移を設定するテスト")]
    public void StateController_Case021()
    {
        // Arrange: StateControllerインスタンスを作成
        var stateController = new StateController();

        // 現在の状態を ApplicationState.Terminate に設定
        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Terminate);

        // isTerminating を true に設定
        var isTerminatingField = stateController.GetType().GetField("isTerminating", BindingFlags.NonPublic | BindingFlags.Instance);
        isTerminatingField!.SetValue(stateController, true);

        // Reflectionを使用してGetTransitableメソッドを取得
        var getTransitableMethod = stateController.GetType().GetMethod("GetTransitable", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: GetTransitableメソッドを呼び出し
        var result = getTransitableMethod!.Invoke(stateController, new object[] { ApplicationState.End });

        // Assert: 結果を検証
        Assert.Equal(StateTransitable.Transitable, (StateTransitable)result!);
    }

    [Fact(DisplayName = "StateController No022_キュー不可の遷移を設定するテスト")]
    public void StateController_Case022()
    {
        // Arrange: StateControllerインスタンスを作成
        var stateController = new StateController();

        // 現在の状態を ApplicationState.Terminate に設定
        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Terminate);

        // Reflectionを使用してGetTransitableメソッドを取得
        var getTransitableMethod = stateController.GetType().GetMethod("GetTransitable", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: GetTransitableメソッドを呼び出し
        var result = getTransitableMethod!.Invoke(stateController, new object[] { ApplicationState.Running });

        // Assert: 結果を検証
        Assert.Equal(StateTransitable.Unqueueable, (StateTransitable)result!);
    }

    [Fact(DisplayName = "StateController No023_特例以外の許可されていない遷移を設定するテスト")]
    public void StateController_Case023()
    {
        // Arrange: StateControllerインスタンスを作成
        var stateController = new StateController();

        // 現在の状態を ApplicationState.Terminate に設定
        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Terminate);

        // isTerminating を false に設定 (特例遷移を許可しない状況を模倣)
        var isTerminatingField = stateController.GetType().GetField("isTerminating", BindingFlags.NonPublic | BindingFlags.Instance);
        isTerminatingField!.SetValue(stateController, true);

        // Reflectionを使用してGetTransitableメソッドを取得
        var getTransitableMethod = stateController.GetType().GetMethod("GetTransitable", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: GetTransitableメソッドを呼び出し
        var result = getTransitableMethod!.Invoke(stateController, new object[] { ApplicationState.Ready });

        // Assert: 結果を検証
        Assert.Equal(StateTransitable.Untransitable, (StateTransitable)result!);
    }
    
    [Fact(DisplayName = "StateController No024_Initialize→Terminateが遷移できることを確認するテスト")]
    public void StateController_Case024()
    {
        // Arrange: StateControllerインスタンスを作成
        var stateController = new StateController();

        // 現在の状態を ApplicationState.Initialize に設定
        var currentStateField = stateController.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        currentStateField!.SetValue(stateController, ApplicationState.Initialize);

        // Reflectionを使用してGetTransitableメソッドを取得
        var getTransitableMethod = stateController.GetType().GetMethod("GetTransitable", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act: GetTransitableメソッドを呼び出し
        var result = getTransitableMethod!.Invoke(stateController, new object[] { ApplicationState.Terminate });

        // Assert: 結果を検証
        Assert.Equal(StateTransitable.Transitable, (StateTransitable)result!);
    }
}

