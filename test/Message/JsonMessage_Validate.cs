using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class JsonMessage_Validate
{
    [Fact(DisplayName = "No404_正常系：正常動作の確認_messageにnull、allowEmptyBodyを指定せずメソッドを呼び出す")]
    public void Case404()
    {
        var result = JsonMessage.Validate(null!);

        Assert.False(result);
    }

    [Fact(DisplayName = "No405_正常系：正常動作の確認_messageにnull、allowEmptyBody=trueを指定して、メソッドを呼び出す")]
    public void Case405()
    {
        var result = JsonMessage.Validate(null!, true);

        Assert.True(result);
    }

    [Fact(DisplayName = "No406_正常系：正常動作の確認_messageのRecordListがnullの有効なJsonMessageインスタンスを指定して、メソッドを呼び出す")]
    public void Case406()
    {
        var message = new JsonMessage();

        var result = JsonMessage.Validate(message);

        Assert.False(result);
    }

    [Fact(DisplayName = "No407_正常系：正常動作の確認_messageのRecordListが要素数0の空リストであるJsonMessageインスタンスを指定して、メソッドを呼び出す")]
    public void Case407()
    {
        var message = new JsonMessage
        {
            RecordList = new List<JsonMessage.RecordInfo>()
        };

        var result = JsonMessage.Validate(message);

        Assert.True(result);
    }

    [Fact(DisplayName = "No408_正常系：正常動作の確認_messageのRecordList内の全RecordInfo.RecordDataが非nullであるJsonMessageインスタンスを指定して、メソッドを呼び出す")]
    public void Case408()
    {
        var message = new JsonMessage
        {
            RecordList = new List<JsonMessage.RecordInfo>
            {
                new JsonMessage.RecordInfo { RecordData = new List<object> { "data1" } },
                new JsonMessage.RecordInfo { RecordData = new List<object> { "data2" } }
            }
        };

        var result = JsonMessage.Validate(message);

        Assert.True(result);
    }

    [Fact(DisplayName = "No409_正常系：正常動作の確認_messageのRecordList内に1つ以上RecordDataがnullのRecordInfoが含まれるJsonMessageインスタンスを指定して、メソッドを呼び出す")]
    public void Case409()
    {
        var message = new JsonMessage
        {
            RecordList = new List<JsonMessage.RecordInfo>
            {
                new JsonMessage.RecordInfo { RecordData = new List<object> { "data" } },
                new JsonMessage.RecordInfo { RecordData = null! }
            }
        };

        var result = JsonMessage.Validate(message);

        Assert.False(result);
    }

    [Fact(DisplayName = "No410_正常系：正常動作の確認_messageのRecordList内にRecordInfo自体がnullの要素が含まれるJsonMessageインスタンスを指定して、メソッドを呼び出す")]
    public void Case410()
    {
        var message = new JsonMessage
        {
            RecordList = new List<JsonMessage.RecordInfo>
            {
                new JsonMessage.RecordInfo { RecordData = new List<object> { "data" } },
                null!
            }
        };

        var result = JsonMessage.Validate(message);

        Assert.False(result);
    }
}