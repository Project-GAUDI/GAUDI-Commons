namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetConnectionModuleId : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No069_フィールド\"message\"がnull")]
    public void Message_Case069()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);

        var result = obj.GetConnectionDeviceId();

        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No070_フィールド\"message\"がnull以外")]
    public void Message_Case070()
    {
        var uuid = Guid.NewGuid().ToString();

        var message = new Message();
        var pi = typeof(Message).GetProperty(nameof(Message.ConnectionModuleId), BindingFlags.Instance | BindingFlags.Public);
        pi!.SetValue(message, uuid);

        var obj = new IotMessage();
        SetMessageProperty(obj, message);

        var result = obj.GetConnectionModuleId();
        Assert.Equal(uuid, result);
    }

    #region 単体テスト仕様書外の既存テスト
    [Fact]
    public void GetConnectionModuleIdTest001()
    {
        // Arrange
        IotMessage MyIotMessage = new IotMessage();
        //Act
        string connnectionmoduleId = MyIotMessage.GetConnectionModuleId();
        //Assert
        Assert.True(string.IsNullOrEmpty(connnectionmoduleId));
    }
    #endregion
}
