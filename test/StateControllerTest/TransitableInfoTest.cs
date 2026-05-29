using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.StateControllerTest;

[Collection(nameof(TransitableInfoTest))]
public class TransitableInfoTest
{
    private readonly ITestOutputHelper _output;

    public TransitableInfoTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact(DisplayName = "TransitableInfo No001_インスタンス生成時のプロパティの初期化確認")]
    public void TransitableInfo_Case001()
    {
        // Arrange: 入力値を設定
        var current = ApplicationState.Start;
        var next = ApplicationState.Initialize;
        var transitable = StateTransitable.Transitable;
        var transitableWhenTerminating = false;

        // Act: インスタンスを生成
        var transitableInfo = new TransitableInfo(current, next, transitable, transitableWhenTerminating);

        // Assert: 各プロパティが正しく初期化されていることを確認
        Assert.Equal(current, transitableInfo.currentState);
        Assert.Equal(next, transitableInfo.nextState);
        Assert.Equal(transitable, transitableInfo.transitable);
        Assert.Equal(transitableWhenTerminating, transitableInfo.transitableWhenTerminating);
    }

    [Fact(DisplayName = "TransitableInfo No002_プロパティが入力指定値で初期化されることを確認")]
    public void TransitableInfo_Case002()
    {
        // Arrange: 入力値を設定
        var current = ApplicationState.Ready;
        var next = ApplicationState.Running;
        var transitable = StateTransitable.Queueable;
        var transitableWhenTerminating = true;

        // Act: インスタンスを生成
        var transitableInfo = new TransitableInfo(current, next, transitable, transitableWhenTerminating);

        // Assert: 各プロパティが正しく初期化されていることを確認
        Assert.Equal(current, transitableInfo.currentState);
        Assert.Equal(next, transitableInfo.nextState);
        Assert.Equal(transitable, transitableInfo.transitable);
        Assert.Equal(transitableWhenTerminating, transitableInfo.transitableWhenTerminating);
    }

    [Fact(DisplayName = "TransitableInfo No003_デフォルト値確認とプロパティの初期化確認")]
    public void TransitableInfo_Case003()
    {
        // Arrange: 入力値を設定 (transitableWhenTerminatingは渡さない)
        var current = ApplicationState.Start;
        var next = ApplicationState.Initialize;
        var transitable = StateTransitable.Transitable;

        // Act: インスタンスを生成 (transitableWhenTerminatingのデフォルト値を使用)
        var transitableInfo = new TransitableInfo(current, next, transitable);

        // Assert: 各プロパティが正しく初期化されていることを確認
        Assert.Equal(current, transitableInfo.currentState);
        Assert.Equal(next, transitableInfo.nextState);
        Assert.Equal(transitable, transitableInfo.transitable);
        Assert.False(transitableInfo.transitableWhenTerminating);
    }
}
