namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_SetMessageId : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No057_フィールド\"message\"がnull")]
    public void Message_Case057()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);

        obj.SetMessageId(Guid.NewGuid().ToString());
    }

    [Fact(DisplayName = "Message No058_フィールド\"message\"がnull以外")]
    public void Message_Case058()
    {
        var uuid = Guid.NewGuid().ToString();
        using var obj = new IotMessage();
        obj.SetMessageId(uuid);

        var message = GetMessageProperty(obj);
        var actual = message!.MessageId;

        Assert.Equal(uuid, actual);
    }

    #region 単体テスト仕様書外の既存テスト
    [Fact]
    public void SetMessageIdTest001()
    {
        IotMessage MyIotMessage = new IotMessage();
        MyIotMessage.SetMessageId("test");

        Assert.Equal("test", MyIotMessage.GetMessageId());
    }
    #endregion
}
