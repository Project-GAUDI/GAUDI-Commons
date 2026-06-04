using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.UtilitiesTest;

[Collection(nameof(Util_GetMessageId))]
public class Util_GetMessageId
{
    private readonly ITestOutputHelper _output;

    public Util_GetMessageId(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Util No001_メッセージID生成のチェック")]
    public void Util_Case001()
    {
        // Arrange & Act
        string messageId1 = Util.GetMessageId();
        string messageId2 = Util.GetMessageId();

        // Assert
        Assert.NotNull(messageId1);
        Assert.NotNull(messageId2);
        Assert.NotEqual(messageId1, messageId2); // 一意性の確認
        Assert.Matches(@"^\d{17}[0-9a-fA-F]{32}$", messageId1); // フォーマット確認
    }
}
