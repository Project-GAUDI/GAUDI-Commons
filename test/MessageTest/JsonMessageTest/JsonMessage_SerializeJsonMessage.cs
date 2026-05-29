using Newtonsoft.Json;

namespace TICO.GAUDI.Commons.Test.MessageTest.JsonMessageTest;

[Collection(nameof(JsonMessageTest))]
public class JsonMessage_SerializeJsonMessage : JsonMessageTesterBase
{
    [Fact(DisplayName = "Message No073_正常終了する")]
    public void Message_Case073()
    {
        var message = CreateDummyMessage();
        var expected = JsonConvert.SerializeObject(message, Formatting.None);

        // シリアライズ結果を検証するため、明示的にNewtonsoft.Jsonを利用したシリアライザを利用するように設定
        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, ENVVALUE_SERIALIZER_NEWTONSOFT);

        var result = JsonMessage.SerializeJsonMessage(message);

        Assert.Equal(expected, result);

        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, null);
    }
}
