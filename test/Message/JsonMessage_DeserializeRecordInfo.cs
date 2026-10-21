using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class JsonMessage_DeserializeRecordInfo
{
    [Fact(DisplayName = "No393_正常系：正常動作の確認_messageにRecordDataを含む有効なJSON文字列を指定して、メソッドを呼び出す")]
    public void Case393()
    {
        const string json = "{\"RecordHeader\":[\"header\"],\"RecordData\":[\"data\"]}";

        var result = JsonMessage.DeserializeRecordInfo(json);

        AssertRecordInfo(result);
    }

    [Fact(DisplayName = "No394_異常系：異常動作の確認_messageに不正なJSON文字列を指定して、メソッドを呼び出す")]
    public void Case394()
    {
        var result = JsonMessage.DeserializeRecordInfo("{ invalid json");

        Assert.Null(result);
    }

    [Fact(DisplayName = "No395_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case395()
    {
        var result = JsonMessage.DeserializeRecordInfo((string)null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No396_正常系：正常動作の確認_messageにRecordDataを含む有効なJSONバイト配列を指定して、メソッドを呼び出す")]
    public void Case396()
    {
        var bytes = Encoding.UTF8.GetBytes("{\"RecordHeader\":[\"header\"],\"RecordData\":[\"data\"]}");

        var result = JsonMessage.DeserializeRecordInfo(bytes);

        AssertRecordInfo(result);
    }

    [Fact(DisplayName = "No397_異常系：異常動作の確認_messageに不正なJSONバイト配列を指定して、メソッドを呼び出す")]
    public void Case397()
    {
        var bytes = Encoding.UTF8.GetBytes("{ invalid json");

        var result = JsonMessage.DeserializeRecordInfo(bytes);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No398_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case398()
    {
        var result = JsonMessage.DeserializeRecordInfo((byte[])null!);

        Assert.Null(result);
    }

    private static void AssertRecordInfo(JsonMessage.RecordInfo? record)
    {
        Assert.NotNull(record);
        Assert.NotNull(record!.RecordHeader);
        Assert.Equal("header", Assert.Single(record.RecordHeader));
        Assert.NotNull(record.RecordData);
        Assert.Equal("data", Assert.Single(record.RecordData).ToString());
    }
}