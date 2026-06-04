using HarmonyLib;
using Newtonsoft.Json;
using System.Text;

namespace TICO.GAUDI.Commons.Test.MessageTest.JsonMessageTest;

[Collection(nameof(JsonMessageTest))]
public class JsonMessage_DeserializeRecordInfo : JsonMessageTesterBase
{
    [Fact(DisplayName = "Message No077_引数が\"message(string)\"、かつ、正常終了する")]
    public void Message_Case077()
    {
        var input = CreateDummyRecord();
        var serialized = JsonConvert.SerializeObject(input);

        var result = JsonMessage.DeserializeRecordInfo(serialized);
        Assert.IsType<JsonMessage.RecordInfo>(result);

        var expectedHeaders = input.RecordHeader;
        var expectedData = input.RecordData;

        Assert.Equal(expectedHeaders.Count, result.RecordHeader.Count);
        Assert.Equal(expectedData.Count, result.RecordData.Count);
        for (var j = 0; j < expectedHeaders.Count; ++j)
        {
            Assert.Equal(expectedHeaders[j], result.RecordHeader[j]);
        }
        for (var j = 0; j < expectedData.Count; ++j)
        {
            Assert.Equal(expectedData[j], result.RecordData[j]);
        }
    }

    [Fact(DisplayName = "Message No078_引数が\"message(string)\"、かつ、例外が発生する")]
    public void Message_Case078()
    {
        var input = CreateDummyRecord();
        var serialized = JsonConvert.SerializeObject(input);

        var harmony = new Harmony(Guid.NewGuid().ToString());
        harmony.PatchAll();

        var result = JsonMessage.DeserializeRecordInfo(serialized);

        Assert.Null(result);
        harmony.UnpatchAll(harmony.Id);
    }

    [Fact(DisplayName = "Message No079_引数が\"message(byte[])\"、かつ、正常終了する")]
    public void Message_Case079()
    {
        var input = CreateDummyRecord();
        var serialized = JsonConvert.SerializeObject(input);
        var bytes = Encoding.UTF8.GetBytes(serialized);

        var result = JsonMessage.DeserializeRecordInfo(bytes);
        Assert.IsType<JsonMessage.RecordInfo>(result);

        var expectedHeaders = input.RecordHeader;
        var expectedData = input.RecordData;

        Assert.Equal(expectedHeaders.Count, result.RecordHeader.Count);
        Assert.Equal(expectedData.Count, result.RecordData.Count);
        for (var j = 0; j < expectedHeaders.Count; ++j)
        {
            Assert.Equal(expectedHeaders[j], result.RecordHeader[j]);
        }
        for (var j = 0; j < expectedData.Count; ++j)
        {
            Assert.Equal(expectedData[j], result.RecordData[j]);
        }
    }

    [Fact(DisplayName = "Message No080_引数が\"message(byte[])\"、かつ、例外が発生する")]
    public void Message_Case080()
    {
        var input = CreateDummyRecord();
        var serialized = JsonConvert.SerializeObject(input);
        var bytes = Encoding.UTF8.GetBytes(serialized);

        var harmony = new Harmony(Guid.NewGuid().ToString());
        harmony.PatchAll();

        var result = JsonMessage.DeserializeRecordInfo(bytes);

        Assert.Null(result);
        harmony.UnpatchAll(harmony.Id);
    }
}
