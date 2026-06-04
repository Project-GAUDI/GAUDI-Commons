using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.JsonSerializerTest;

[Collection(nameof(NewtonsoftJsonSerializer_SerializeBytes))]
public class NewtonsoftJsonSerializer_SerializeBytes
{
    private readonly ITestOutputHelper _output;

    public NewtonsoftJsonSerializer_SerializeBytes(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "JsonSerializer Case012_targetをnullの状態で呼び出し")]
    public void JsonSerializer_Case012()
    {
        NewtonsoftJsonSerializer serializer = new();
        byte[]? ret = serializer.SerializeBytes<string?>(null);
        Assert.Null(ret);
    }

    [Fact(DisplayName = "JsonSerializer Case013_targetに値をセットして呼び出し")]
    public void JsonSerializer_Case013()
    {
        string testData = @"a""b""c\\.<d></e>";
        byte[] expectedData = [34, 97, 92, 34, 98, 92, 34, 99, 92, 92, 92, 92, 46, 60, 100, 62, 60, 47, 101, 62, 34];

        NewtonsoftJsonSerializer serializer = new();
        byte[] ret = serializer.SerializeBytes<string>(testData);
        Assert.Equal(expectedData, ret);
    }
}