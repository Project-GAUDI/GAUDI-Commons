namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetMessage : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No053_フィールド\"message\"がnull")]
    public void Message_Case053()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);

        var result = obj.GetMessage();

        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No054_フィールド\"message\"がnull以外")]
    public void Message_Case054()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);

        var result = obj.GetMessage();

        Assert.Equal(message, result);
    }

    #region 単体テスト仕様書外の既存テスト
    [Fact]
    public void GetMessageTest001()
    {
        // Arrange
        IotMessage MyIotMessage = new IotMessage();
        Message expectedMessage = MyIotMessage.GetMessage();

        // Act
        Message actualMessage = MyIotMessage.GetMessage();

        // Assert
        Assert.Equal(expectedMessage, actualMessage);
    }
    #endregion
}