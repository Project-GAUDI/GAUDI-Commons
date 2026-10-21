using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class JsonMessage_SerializeJsonMessageByte
{
    [Fact(DisplayName = "No390_正常系：正常動作の確認_messageにRecordListを設定済みの有効なJsonMessageインスタンスを指定して、メソッドを呼び出す")]
    public void Case390()
    {
        var message = NewJsonMessage();

        var result = JsonMessage.SerializeJsonMessageByte(message);

        AssertSerializedJsonMessage(result);
    }

    [Fact(DisplayName = "No391_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case391()
    {
        var result = JsonMessage.SerializeJsonMessageByte(null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No392_異常系：異常動作の確認_messageにシリアライズ処理中に例外が発生するデータを指定して、メソッドを呼び出す")]
    public void Case392()
    {
        var circular = new List<object>();
        circular.Add(circular);
        var message = new JsonMessage
        {
            RecordList = new List<JsonMessage.RecordInfo>
            {
                new JsonMessage.RecordInfo
                {
                    RecordData = new List<object> { circular }
                }
            }
        };

        var result = JsonMessage.SerializeJsonMessageByte(message);

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

    private static void AssertSerializedJsonMessage(byte[]? bytes)
    {
        Assert.NotNull(bytes);
        var result = JsonMessage.DeserializeJsonMessage(Encoding.UTF8.GetString(bytes!));
        Assert.NotNull(result);
        Assert.NotNull(result!.RecordList);
        var record = Assert.Single(result.RecordList);
        Assert.NotNull(record.RecordHeader);
        Assert.Equal("header", Assert.Single(record.RecordHeader));
        Assert.NotNull(record.RecordData);
        Assert.Equal("data", Assert.Single(record.RecordData).ToString());
    }
}