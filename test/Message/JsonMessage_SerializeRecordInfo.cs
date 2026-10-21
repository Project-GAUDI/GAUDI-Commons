using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class JsonMessage_SerializeRecordInfo
{
    [Fact(DisplayName = "No399_正常系：正常動作の確認_messageにRecordDataを設定済みの有効なRecordInfoインスタンスを指定して、メソッドを呼び出す")]
    public void Case399()
    {
        var record = NewRecordInfo();

        var result = JsonMessage.SerializeRecordInfo(record);

        AssertSerializedRecordInfo(result);
    }

    [Fact(DisplayName = "No400_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case400()
    {
        var result = JsonMessage.SerializeRecordInfo(null!);

        Assert.Null(result);
    }

    private static JsonMessage.RecordInfo NewRecordInfo()
    {
        return new JsonMessage.RecordInfo
        {
            RecordHeader = new List<string> { "header" },
            RecordData = new List<object> { "data" }
        };
    }

    private static void AssertSerializedRecordInfo(string? json)
    {
        Assert.NotNull(json);
        var result = JsonMessage.DeserializeRecordInfo(json!);
        Assert.NotNull(result);
        Assert.NotNull(result!.RecordHeader);
        Assert.Equal("header", Assert.Single(result.RecordHeader));
        Assert.NotNull(result.RecordData);
        Assert.Equal("data", Assert.Single(result.RecordData).ToString());
    }
}