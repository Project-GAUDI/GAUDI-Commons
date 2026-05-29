namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_Dispose : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No007_正常終了する")]
    public void Message_Case007()
    {
        var obj = new IotMessage();

        obj.Dispose();

        var message = GetMessageProperty(obj);
        Assert.Null(message);
    }
}
