using HarmonyLib;
using Newtonsoft.Json;
using System.Text;

namespace TICO.GAUDI.Commons.Test.MessageTest.JsonMessageTest;

[Collection(nameof(JsonMessageTest))]
public class JsonMessage_SerializeJsonMessageByte : JsonMessageTesterBase
{
    [Fact(DisplayName = "Message No074_引数\"message\"がnull")]
    public void Message_Case074()
    {
        var result = JsonMessage.SerializeJsonMessageByte(null);
        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No075_引数\"message\"がnull以外、かつ、正常終了する")]
    public void Message_Case075()
    {
        var message = CreateDummyMessage();
        var serialized = JsonConvert.SerializeObject(message, Formatting.None);
        var expected = Encoding.UTF8.GetBytes(serialized);

        // シリアライズ結果を検証するため、明示的にNewtonsoft.Jsonを利用したシリアライザを利用するように設定
        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, ENVVALUE_SERIALIZER_NEWTONSOFT);

        var result = JsonMessage.SerializeJsonMessageByte(message);

        Assert.Equal(expected, result);

        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, null);
    }

    [Fact(DisplayName = "Message No076_引数\"message\"がnull以外、かつ、例外が発生する")]
    public void Message_Case076()
    {
        var input = CreateDummyMessage();

        var harmony = new Harmony(Guid.NewGuid().ToString());
        harmony.PatchAll();

        var result = JsonMessage.SerializeJsonMessageByte(input);

        Assert.Null(result);
        harmony.UnpatchAll(harmony.Id);
    }
}
