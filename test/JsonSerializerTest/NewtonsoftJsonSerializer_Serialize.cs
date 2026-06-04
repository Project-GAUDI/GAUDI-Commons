using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.JsonSerializerTest;

[Collection(nameof(NewtonsoftJsonSerializer_Serialize))]
public class NewtonsoftJsonSerializer_Serialize
{
    private readonly ITestOutputHelper _output;

    public NewtonsoftJsonSerializer_Serialize(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "JsonSerializer Case009_targetをnullの状態で呼び出し")]
    public void JsonSerializer_Case009()
    {
        NewtonsoftJsonSerializer serializer = new();
        string? ret = serializer.Serialize<string?>(null);
        Assert.Null(ret);
    }

    [Fact(DisplayName = "JsonSerializer Case010_targetに値をセットして呼び出し")]
    public void JsonSerializer_Case010()
    {
        NewtonsoftJsonSerializer serializer = new();
        string testData = @"a""b""c\\.<d></e>";
        string expectedData = @"""a\""b\""c\\\\.<d></e>""";
        var ret = serializer.Serialize<string>(testData);
        Assert.Equal(expectedData, ret);
    }
}