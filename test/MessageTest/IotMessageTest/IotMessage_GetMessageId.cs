namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetMessageId : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No055_フィールド\"message\"がnull")]
    public void Message_Case055()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);

        var result = obj.GetMessageId();

        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No056_フィールド\"message\"がnull以外")]
    public void Message_Case056()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        var expected = message!.MessageId;

        var result = obj.GetMessageId();

        Assert.Equal(expected, result);
    }

    #region 単体テスト仕様書外の既存テスト
    [Fact]
    public void GetMessageIdTest001()
    {
        // Arrange
        IotMessage MyIotMessage = new IotMessage();

        // Act
        string actualMessageId = MyIotMessage.GetMessageId();

        // Assert
        Assert.Null(actualMessageId);
    }
    #endregion
}
