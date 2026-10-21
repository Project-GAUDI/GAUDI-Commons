using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class JsonMessage_SerializeRecordInfoByte
{
    [Fact(DisplayName = "No401_正常系：正常動作の確認_messageにRecordDataを設定済みの有効なRecordInfoインスタンスを指定して、メソッドを呼び出す")]
    public void Case401()
    {
        var record = NewRecordInfo();

        var result = JsonMessage.SerializeRecordInfoByte(record);

        AssertSerializedRecordInfo(result);
    }

    [Fact(DisplayName = "No402_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case402()
    {
        var result = JsonMessage.SerializeRecordInfoByte(null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No403_異常系：異常動作の確認_messageにシリアライズ処理中に例外が発生するデータを指定して、メソッドを呼び出す")]
    public void Case403()
    {
        var circular = new List<object>();
        circular.Add(circular);
        var record = new JsonMessage.RecordInfo
        {
            RecordData = new List<object> { circular }
        };

        var result = JsonMessage.SerializeRecordInfoByte(record);

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

    private static void AssertSerializedRecordInfo(byte[]? bytes)
    {
        Assert.NotNull(bytes);
        var result = JsonMessage.DeserializeRecordInfo(Encoding.UTF8.GetString(bytes!));
        Assert.NotNull(result);
        Assert.NotNull(result!.RecordHeader);
        Assert.Equal("header", Assert.Single(result.RecordHeader));
        Assert.NotNull(result.RecordData);
        Assert.Equal("data", Assert.Single(result.RecordData).ToString());
    }
}