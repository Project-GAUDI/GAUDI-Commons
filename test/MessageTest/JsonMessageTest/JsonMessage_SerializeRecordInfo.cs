using Newtonsoft.Json;

namespace TICO.GAUDI.Commons.Test.MessageTest.JsonMessageTest;

[Collection(nameof(JsonMessageTest))]
public class SerializeRecordInfo : JsonMessageTesterBase
{
    [Fact(DisplayName = "Message No081_正常終了する")]
    public void Message_Case081()
    {
        var record = CreateDummyRecord();
        var serialized = JsonConvert.SerializeObject(record, Formatting.None);

        // シリアライズ結果を検証するため、明示的にNewtonsoft.Jsonを利用したシリアライザを利用するように設定
        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, ENVVALUE_SERIALIZER_NEWTONSOFT);

        var result = JsonMessage.SerializeRecordInfo(record);

        Assert.Equal(serialized, result);

        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, null);
    }
}
