namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetConnectionDeviceId : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No067_フィールド\"message\"がnull")]
    public void Message_Case067()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);

        var result = obj.GetConnectionDeviceId();

        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No068_フィールド\"message\"がnull以外")]
    public void Message_Case068()
    {
        var uuid = Guid.NewGuid().ToString();

        var message = new Message();
        var pi = typeof(Message).GetProperty(nameof(Message.ConnectionDeviceId), BindingFlags.Instance | BindingFlags.Public);
        pi!.SetValue(message, uuid);

        var obj = new IotMessage();
        SetMessageProperty(obj, message);

        var result = obj.GetConnectionDeviceId();
        Assert.Equal(uuid, result);
    }
}
