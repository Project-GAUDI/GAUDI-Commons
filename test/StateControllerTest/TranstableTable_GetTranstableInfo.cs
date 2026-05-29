using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.StateControllerTest;

[Collection(nameof(TranstableTable_GetTranstableInfo))]
public class TranstableTable_GetTranstableInfo
{
    private readonly ITestOutputHelper _output;

    public TranstableTable_GetTranstableInfo(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "TranstableTable No006_状態遷移情報の取得")]
    public void TranstableTable_Case006()
    {
        // Arrange: 状態遷移情報を作成
        var currentState = ApplicationState.Start;
        var nextState = ApplicationState.Initialize;

        var transitableInfo = new TransitableInfo(
            currentState, 
            nextState, 
            StateTransitable.Transitable
        );

        // TranstableTable インスタンスを作成
        var transtableTable = new TranstableTable
        {
            transitableInfo
        };

        // Act: GetTranstableInfo メソッドで状態遷移情報を取得
        var retrievedInfo = transtableTable.GetTranstableInfo(currentState, nextState);

        // Assert: 取得した状態遷移情報が正しいか確認
        Assert.NotNull(retrievedInfo);
        Assert.Equal(transitableInfo, retrievedInfo);
    }

    [Fact(DisplayName = "TranstableTable No008_存在しない状態遷移情報の取得")]
    public void TranstableTable_Case008()
    {
        // Arrange: TranstableTable インスタンスを作成（情報は追加しない状態）
        var transtableTable = new TranstableTable();

        // テスト対象の currentState と nextState
        var currentState = ApplicationState.Ready;
        var nextState = ApplicationState.End;

        // Act: GetTranstableInfo メソッドで未登録の状態遷移情報を取得
        var retrievedInfo = transtableTable.GetTranstableInfo(currentState, nextState);

        // Assert: 取得した結果が null であることを確認
        Assert.Null(retrievedInfo);
    }

    [Fact(DisplayName = "TranstableTable No009_currentStateのみ存在する状態遷移情報の取得")]
    public void TranstableTable_Case009()
    {
        // Arrange: TranstableTable に一部状態遷移情報を追加
        var transtableTable = new TranstableTable();

        // currentState と nextState を定義
        var currentState = ApplicationState.Start;
        var existingNextState = ApplicationState.Initialize;
        var missingNextState = ApplicationState.End;

        // 状態遷移情報を追加 (currentState -> existingNextState)
        transtableTable.Add(new TransitableInfo(
            currentState,
            existingNextState,
            StateTransitable.Transitable
        ));

        // Act: 存在しない nextState の情報を取得
        var retrievedInfo = transtableTable.GetTranstableInfo(currentState, missingNextState);

        // Assert: null が返されることを確認
        Assert.Null(retrievedInfo);
    }
}