using Newtonsoft.Json;
using System.Text;

namespace TICO.GAUDI.Commons.Test.MessageTest.JsonMessageTest;

[Collection(nameof(JsonMessageTest))]
public class JsonMessage_RecordHeaderOptional : JsonMessageTesterBase
{
    [Fact(DisplayName = "RecordHeader_nullでシリアライズ_JSONにRecordHeaderキーが含まれない")]
    public void Serialize_RecordHeaderNull_OmittedFromJson()
    {
        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, ENVVALUE_SERIALIZER_NEWTONSOFT);

        var record = new JsonMessage.RecordInfo()
        {
            RecordHeader = null,
            RecordData = new List<object> { "data1", "data2" }
        };

        var result = JsonMessage.SerializeRecordInfo(record);

        Assert.DoesNotContain("RecordHeader", result);
        Assert.Contains("RecordData", result);

        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, null);
    }

    [Fact(DisplayName = "RecordHeader_設定ありでシリアライズ_JSONにRecordHeaderキーが含まれる")]
    public void Serialize_RecordHeaderPresent_IncludedInJson()
    {
        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, ENVVALUE_SERIALIZER_NEWTONSOFT);

        var record = new JsonMessage.RecordInfo()
        {
            RecordHeader = new List<string> { "header1" },
            RecordData = new List<object> { "data1" }
        };

        var result = JsonMessage.SerializeRecordInfo(record);

        Assert.Contains("\"RecordHeader\":[\"header1\"]", result);
        Assert.Contains("\"RecordData\":[\"data1\"]", result);

        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, null);
    }

    [Fact(DisplayName = "RecordHeader_なしJSONをデシリアライズ_RecordHeaderがnullになる")]
    public void Deserialize_NoRecordHeader_ResultIsNull()
    {
        var json = "{\"RecordData\":[\"data1\",\"data2\"]}";

        var result = JsonMessage.DeserializeRecordInfo(json);

        Assert.NotNull(result);
        Assert.Null(result.RecordHeader);
        Assert.Equal(2, result.RecordData.Count);
        Assert.Equal("data1", result.RecordData[0]);
        Assert.Equal("data2", result.RecordData[1]);
    }

    [Fact(DisplayName = "RecordHeader_ありJSONをデシリアライズ_RecordHeaderが取得できる")]
    public void Deserialize_WithRecordHeader_ResultHasHeader()
    {
        var json = "{\"RecordHeader\":[\"h1\",\"h2\"],\"RecordData\":[\"d1\"]}";

        var result = JsonMessage.DeserializeRecordInfo(json);

        Assert.NotNull(result);
        Assert.NotNull(result.RecordHeader);
        Assert.Equal(2, result.RecordHeader.Count);
        Assert.Equal("h1", result.RecordHeader[0]);
        Assert.Equal("h2", result.RecordHeader[1]);
    }
}

[Collection(nameof(JsonMessageTest))]
public class JsonMessage_RecordDataMultiType : JsonMessageTesterBase
{
    [Fact(DisplayName = "RecordData_多型配列のシリアライズ_JSON型が保持される")]
    public void Serialize_MixedTypes_PreservesJsonTypes()
    {
        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, ENVVALUE_SERIALIZER_NEWTONSOFT);

        var record = new JsonMessage.RecordInfo()
        {
            RecordData = new List<object> { "text", 123L, 45.6, true, null }
        };

        var result = JsonMessage.SerializeRecordInfo(record);

        Assert.Contains("\"text\"", result);
        Assert.Contains("123", result);
        Assert.Contains("45.6", result);
        Assert.Contains("true", result);
        Assert.Contains("null", result);

        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, null);
    }

    [Fact(DisplayName = "RecordData_多型配列のデシリアライズ_各要素の型が保持される")]
    public void Deserialize_MixedTypes_PreservesTypes()
    {
        var json = "{\"RecordData\":[\"text\",123,45.6,true,null]}";

        var result = JsonMessage.DeserializeRecordInfo(json);

        Assert.NotNull(result);
        Assert.Equal(5, result.RecordData.Count);
        Assert.IsType<string>(result.RecordData[0]);
        Assert.Equal("text", result.RecordData[0]);
        // Newtonsoft.Json deserializes integers as long
        Assert.IsType<long>(result.RecordData[1]);
        Assert.Equal(123L, result.RecordData[1]);
        // Newtonsoft.Json deserializes decimals as double
        Assert.IsType<double>(result.RecordData[2]);
        Assert.Equal(45.6, result.RecordData[2]);
        Assert.IsType<bool>(result.RecordData[3]);
        Assert.Equal(true, result.RecordData[3]);
        Assert.Null(result.RecordData[4]);
    }

    [Fact(DisplayName = "RecordData_多型配列のラウンドトリップ_型が保持される")]
    public void RoundTrip_MixedTypes_PreservesTypes()
    {
        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, ENVVALUE_SERIALIZER_NEWTONSOFT);

        var original = new JsonMessage.RecordInfo()
        {
            RecordData = new List<object> { "hello", 42L, 3.14, false, null }
        };

        var serialized = JsonMessage.SerializeRecordInfo(original);
        var deserialized = JsonMessage.DeserializeRecordInfo(serialized);

        Assert.Equal(original.RecordData.Count, deserialized.RecordData.Count);
        Assert.Equal("hello", deserialized.RecordData[0]);
        Assert.Equal(42L, deserialized.RecordData[1]);
        Assert.Equal(3.14, deserialized.RecordData[2]);
        Assert.Equal(false, deserialized.RecordData[3]);
        Assert.Null(deserialized.RecordData[4]);

        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, null);
    }

    [Fact(DisplayName = "RecordData_JsonMessage全体の多型ラウンドトリップ")]
    public void RoundTrip_JsonMessage_MixedTypes()
    {
        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, ENVVALUE_SERIALIZER_NEWTONSOFT);

        var message = new JsonMessage()
        {
            RecordList =
            [
                new JsonMessage.RecordInfo()
                {
                    RecordHeader = new List<string> { "timestamp" },
                    RecordData = new List<object> { "sensor1", 25.5, true }
                },
                new JsonMessage.RecordInfo()
                {
                    RecordData = new List<object> { "sensor2", 100L, null }
                }
            ]
        };

        var serialized = JsonMessage.SerializeJsonMessage(message);
        var deserialized = JsonMessage.DeserializeJsonMessage(serialized);

        Assert.Equal(2, deserialized.RecordList.Count);

        // First record has RecordHeader
        Assert.NotNull(deserialized.RecordList[0].RecordHeader);
        Assert.Equal("timestamp", deserialized.RecordList[0].RecordHeader[0]);
        Assert.Equal("sensor1", deserialized.RecordList[0].RecordData[0]);
        Assert.Equal(25.5, deserialized.RecordList[0].RecordData[1]);
        Assert.Equal(true, deserialized.RecordList[0].RecordData[2]);

        // Second record has no RecordHeader
        Assert.Null(deserialized.RecordList[1].RecordHeader);
        Assert.Equal("sensor2", deserialized.RecordList[1].RecordData[0]);
        Assert.Equal(100L, deserialized.RecordList[1].RecordData[1]);
        Assert.Null(deserialized.RecordList[1].RecordData[2]);

        Environment.SetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER, null);
    }
}
