using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.JsonSerializerTest;

[Collection(nameof(NewtonsoftJsonSerializer_Deserialize_Byte))]
public class NewtonsoftJsonSerializer_Deserialize_Byte
{
    private readonly ITestOutputHelper _output;

    public NewtonsoftJsonSerializer_Deserialize_Byte(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "JsonSerializer Case019_jsonStringをnullの状態で呼び出し")]
    public void JsonSerializer_Case019()
    {
        NewtonsoftJsonSerializer serializer = new();
        var ret = serializer.Deserialize<string>((byte[]?)null);
        Assert.Equal(default(string), ret);
    }

    [Fact(DisplayName = "JsonSerializer Case020_jsonBytesに値をセットして呼び出し")]
    public void JsonSerializer_Case020()
    {
        byte[] testData = [34, 97, 92, 34, 98, 92, 34, 99, 92, 92, 92, 92, 46, 60, 100, 62, 60, 47, 101, 62, 34];
        string expectedData = @"a""b""c\\.<d></e>";

        NewtonsoftJsonSerializer serializer = new();
        string ret = serializer.Deserialize<string>(testData);
        Assert.Equal(expectedData, ret);
    }
}