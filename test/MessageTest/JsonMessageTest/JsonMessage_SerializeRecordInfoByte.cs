using HarmonyLib;
using Newtonsoft.Json;
using System.Text;

namespace TICO.GAUDI.Commons.Test.MessageTest.JsonMessageTest;

[Collection(nameof(JsonMessageTest))]
public class JsonMessage_SerializeRecordInfoByte : JsonMessageTesterBase
{
    [Fact(DisplayName = "Message No082_引数\"message\"がnull")]
    public void Message_Case082()
    {
        var result = JsonMessage.SerializeRecordInfoByte(null);
        Assert.Null(result);
    }

    [Fact(DisplayName = "Message No083_引数\"message\"がnull以外、かつ、正常終了する")]
    public void Message_Case083()
    {
        var record = CreateDummyRecord();
        var serialized = JsonConvert.SerializeObject(record, Formatting.None);

        // シリアライズ結果を検証するため、明示的にNewtonsoft.Jsonを利用したシリアライザを利用するように設定
        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, ENVVALUE_SERIALIZER_NEWTONSOFT);

        var result = JsonMessage.SerializeRecordInfoByte(record);

        var expected = Encoding.UTF8.GetBytes(serialized);
        Assert.Equal(expected, result);

        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, null);
    }

    [Fact(DisplayName = "Message No084_引数\"message\"がnull以外、かつ、例外が発生する")]
    public void Message_Case084()
    {
        var input = CreateDummyRecord();
        var harmony = new Harmony(Guid.NewGuid().ToString());
        harmony.PatchAll();

        var result = JsonMessage.SerializeRecordInfoByte(input);

        Assert.Null(result);
        harmony.UnpatchAll(harmony.Id);
    }
}
