using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class JsonMessage_DeserializeJsonMessage
{
    [Fact(DisplayName = "No382_正常系：正常動作の確認_messageにRecordListを含む有効なJSON文字列を指定して、メソッドを呼び出す")]
    public void Case382()
    {
        const string json = "{\"RecordList\":[{\"RecordHeader\":[\"header\"],\"RecordData\":[\"data\"]}]}";

        var result = JsonMessage.DeserializeJsonMessage(json);

        AssertRecordList(result);
    }

    [Fact(DisplayName = "No383_異常系：異常動作の確認_messageに不正なJSON文字列を指定して、メソッドを呼び出す")]
    public void Case383()
    {
        var result = JsonMessage.DeserializeJsonMessage("{ invalid json");

        Assert.Null(result);
    }

    [Fact(DisplayName = "No384_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case384()
    {
        var result = JsonMessage.DeserializeJsonMessage((string)null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No385_正常系：正常動作の確認_messageにRecordListを含む有効なJSONバイト配列を指定して、メソッドを呼び出す")]
    public void Case385()
    {
        var bytes = Encoding.UTF8.GetBytes("{\"RecordList\":[{\"RecordHeader\":[\"header\"],\"RecordData\":[\"data\"]}]}");

        var result = JsonMessage.DeserializeJsonMessage(bytes);

        AssertRecordList(result);
    }

    [Fact(DisplayName = "No386_異常系：異常動作の確認_messageに不正なJSONバイト配列を指定して、メソッドを呼び出す")]
    public void Case386()
    {
        var bytes = Encoding.UTF8.GetBytes("{ invalid json");

        var result = JsonMessage.DeserializeJsonMessage(bytes);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No387_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case387()
    {
        var result = JsonMessage.DeserializeJsonMessage((byte[])null!);

        Assert.Null(result);
    }

    private static void AssertRecordList(JsonMessage? message)
    {
        Assert.NotNull(message);
        Assert.NotNull(message!.RecordList);
        var record = Assert.Single(message.RecordList);
        Assert.NotNull(record.RecordHeader);
        Assert.Equal("header", Assert.Single(record.RecordHeader));
        Assert.NotNull(record.RecordData);
        Assert.Equal("data", Assert.Single(record.RecordData).ToString());
    }
}