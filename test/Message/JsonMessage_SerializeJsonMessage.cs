using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class JsonMessage_SerializeJsonMessage
{
    [Fact(DisplayName = "No388_正常系：正常動作の確認_messageにRecordListを設定済みの有効なJsonMessageインスタンスを指定して、メソッドを呼び出す")]
    public void Case388()
    {
        var message = NewJsonMessage();

        var result = JsonMessage.SerializeJsonMessage(message);

        AssertSerializedJsonMessage(result);
    }

    [Fact(DisplayName = "No389_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case389()
    {
        var result = JsonMessage.SerializeJsonMessage(null!);

        Assert.Null(result);
    }

    private static JsonMessage NewJsonMessage()
    {
        return new JsonMessage
        {
            RecordList = new List<JsonMessage.RecordInfo>
            {
                new JsonMessage.RecordInfo
                {
                    RecordHeader = new List<string> { "header" },
                    RecordData = new List<object> { "data" }
                }
            }
        };
    }

    private static void AssertSerializedJsonMessage(string? json)
    {
        Assert.NotNull(json);
        var result = JsonMessage.DeserializeJsonMessage(json!);
        Assert.NotNull(result);
        Assert.NotNull(result!.RecordList);
        var record = Assert.Single(result.RecordList);
        Assert.NotNull(record.RecordHeader);
        Assert.Equal("header", Assert.Single(record.RecordHeader));
        Assert.NotNull(record.RecordData);
        Assert.Equal("data", Assert.Single(record.RecordData).ToString());
    }
}