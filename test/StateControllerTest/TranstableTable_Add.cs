using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.StateControllerTest;

[Collection(nameof(TranstableTable_Add))]
public class TranstableTable_Add
{
    private readonly ITestOutputHelper _output;

    public TranstableTable_Add(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "TranstableTable No001_状態遷移情報の追加テスト")]
    public void TranstableTable_Case001()
    {
        // Arrange: 状態遷移情報を作成
        var currentState = ApplicationState.Start;
        var nextState = ApplicationState.Initialize;

        // TransitableInfo オブジェクトを作成
        var transitableInfo = new TransitableInfo(
            currentState, 
            nextState, 
            StateTransitable.Transitable
        );

        // TranstableTable インスタンスを作成
        var transtableTable = new TranstableTable();

        // Act: Add メソッドを実行して新しいエントリを追加
        transtableTable.Add(transitableInfo);

        // Assert: currentState がキーとして登録されていることを確認
        Assert.True(transtableTable.ContainsKey(currentState));

        // Assert: currentState に関連付けられた nextState が存在することを確認
        Assert.True(transtableTable[currentState].ContainsKey(nextState));

        // Assert: 登録された TransitableInfo が正しいことを確認
        var addedInfo = transtableTable[currentState][nextState];
        Assert.Equal(transitableInfo, addedInfo);
    }

    [Fact(DisplayName = "TranstableTable No002_既存キーへの状態遷移情報の追加")]
    public void TranstableTable_Case002()
    {
        // Arrange: 初期の状態遷移情報を作成
        var currentState = ApplicationState.Start;

        var initialNextState = ApplicationState.Initialize;
        var initialTransitableInfo = new TransitableInfo(
            currentState, 
            initialNextState, 
            StateTransitable.Transitable
        );

        var additionalNextState = ApplicationState.Ready;
        var additionalTransitableInfo = new TransitableInfo(
            currentState, 
            additionalNextState, 
            StateTransitable.Queueable
        );

        // TranstableTable インスタンスを作成
        var transtableTable = new TranstableTable();

        // 初期情報を追加
        transtableTable.Add(initialTransitableInfo);

        // Act: 既存の currentState に additionalNextState を追加
        transtableTable.Add(additionalTransitableInfo);

        // Assert: currentState がキーとして保持されていることを確認
        Assert.True(transtableTable.ContainsKey(currentState));

        // Assert: currentState に対応するサブ辞書が存在し、その中に initialNextState があることを確認
        Assert.True(transtableTable[currentState].ContainsKey(initialNextState));
        
        // Assert: currentState に対応するサブ辞書に追加された additionalNextState があることを確認
        Assert.True(transtableTable[currentState].ContainsKey(additionalNextState));

        // Assert: 各情報が正しく保持されていることを確認
        var addedInitialInfo = transtableTable[currentState][initialNextState];
        Assert.Equal(initialTransitableInfo, addedInitialInfo);

        var addedAdditionalInfo = transtableTable[currentState][additionalNextState];
        Assert.Equal(additionalTransitableInfo, addedAdditionalInfo);
    }

    [Fact(DisplayName = "TranstableTable No003_新しいキーへの状態遷移情報の追加")]
    public void TranstableTable_Case003()
    {
        // Arrange: 初期の状態遷移情報を作成
        var existingCurrentState = ApplicationState.Start;
        var existingNextState = ApplicationState.Initialize;

        var existingTransitableInfo = new TransitableInfo(
            existingCurrentState, 
            existingNextState, 
            StateTransitable.Transitable
        );

        var newCurrentState = ApplicationState.Ready;  // 新しいキー
        var newNextState = ApplicationState.Running;

        var newTransitableInfo = new TransitableInfo(
            newCurrentState, 
            newNextState, 
            StateTransitable.Transitable
        );

        // TranstableTable インスタンスを作成
        var transtableTable = new TranstableTable
        {
            // 既存の状態遷移情報を追加
            existingTransitableInfo,

            // Act: 新しいキー（Ready）に関連する状態遷移情報を追加
            newTransitableInfo
        };

        // Assert: 既存のキーが保持されていることを確認
        Assert.True(transtableTable.ContainsKey(existingCurrentState));
        Assert.True(transtableTable[existingCurrentState].ContainsKey(existingNextState));
        var addedExistingInfo = transtableTable[existingCurrentState][existingNextState];
        Assert.Equal(existingTransitableInfo, addedExistingInfo);

        // Assert: 新しいキーが追加されていることを確認
        Assert.True(transtableTable.ContainsKey(newCurrentState));
        Assert.True(transtableTable[newCurrentState].ContainsKey(newNextState));
        var addedNewInfo = transtableTable[newCurrentState][newNextState];
        Assert.Equal(newTransitableInfo, addedNewInfo);
    }

    [Fact(DisplayName = "TranstableTable No004_重複登録時に例外がスローされる")]
    public void TranstableTable_Case004()
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
        var transtableTable = new TranstableTable();

        // 初回登録
        transtableTable.Add(transitableInfo);

        // Act & Assert: 同じ状態遷移情報を再登録すると例外がスローされる
        Assert.ThrowsAny<Exception>(() => transtableTable.Add(transitableInfo));
    }

    [Fact(DisplayName = "TranstableTable No005_nullの追加で例外が発生する")]
    public void TranstableTable_Case005()
    {
        // Arrange: TranstableTable インスタンスを作成
        var transtableTable = new TranstableTable();

        // Act & Assert: null を追加しようとすると例外が発生することを確認
        Assert.ThrowsAny<Exception>(() => transtableTable.Add(null));
    }
}