using Newtonsoft.Json;

namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetProperties : IotMessageTesterBase
{
    private const string DUMMY_KEY1 = "dummy-key1";

    private const string DUMMY_VALUE1 = "dummy-value1";

    [Fact(DisplayName = "Message No038_フィールド\"message\"がnull")]
    public void Message_Case038()
    {
        // messageがnullの場合、Disposeに失敗するためusing句による自動破棄を行わない
        var obj = new IotMessage();
        SetMessageProperty(obj, null);

        var result = obj.GetProperties();

        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No039_フィールド\"message\"がnull以外")]
    public void Message_Case039()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.Properties.Add(DUMMY_KEY1, DUMMY_VALUE1);

        var properties = obj.GetProperties();

        var expected = JsonConvert.SerializeObject(message.Properties);
        var actual = JsonConvert.SerializeObject(properties);
        Assert.Equal(expected, actual);
    }
}
