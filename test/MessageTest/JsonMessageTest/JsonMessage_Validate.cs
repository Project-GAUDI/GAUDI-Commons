using Newtonsoft.Json;

namespace TICO.GAUDI.Commons.Test.MessageTest.JsonMessageTest;

[Collection(nameof(JsonMessageTest))]
public class JsonMessage_Validate : JsonMessageTesterBase
{
    [Fact(DisplayName = "Validate_正常なJsonMessage_trueを返す")]
    public void Validate_ValidMessage_ReturnsTrue()
    {
        var message = CreateDummyMessage();

        var result = JsonMessage.Validate(message);

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate_RecordListがnull_falseを返す")]
    public void Validate_RecordListNull_ReturnsFalse()
    {
        var message = new JsonMessage() { RecordList = null };

        var result = JsonMessage.Validate(message);

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate_RecordDataがnull_falseを返す")]
    public void Validate_RecordDataNull_ReturnsFalse()
    {
        var message = new JsonMessage()
        {
            RecordList = [new JsonMessage.RecordInfo() { RecordData = null }]
        };

        var result = JsonMessage.Validate(message);

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate_messageがnull_allowEmptyBody=false_falseを返す")]
    public void Validate_MessageNull_NotAllowEmpty_ReturnsFalse()
    {
        var result = JsonMessage.Validate(null, allowEmptyBody: false);

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate_messageがnull_allowEmptyBody=true_trueを返す")]
    public void Validate_MessageNull_AllowEmpty_ReturnsTrue()
    {
        var result = JsonMessage.Validate(null, allowEmptyBody: true);

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate_RecordList空配列_trueを返す")]
    public void Validate_EmptyRecordList_ReturnsTrue()
    {
        var message = new JsonMessage() { RecordList = [] };

        var result = JsonMessage.Validate(message);

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate_RecordData空配列_trueを返す")]
    public void Validate_EmptyRecordData_ReturnsTrue()
    {
        var message = new JsonMessage()
        {
            RecordList = [new JsonMessage.RecordInfo() { RecordData = new List<object>() }]
        };

        var result = JsonMessage.Validate(message);

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate_複数RecordInfoのうち1つのRecordDataがnull_falseを返す")]
    public void Validate_OneRecordDataNull_ReturnsFalse()
    {
        var message = new JsonMessage()
        {
            RecordList =
            [
                new JsonMessage.RecordInfo() { RecordData = new List<object> { "data1" } },
                new JsonMessage.RecordInfo() { RecordData = null }
            ]
        };

        var result = JsonMessage.Validate(message);

        Assert.False(result);
    }
}
