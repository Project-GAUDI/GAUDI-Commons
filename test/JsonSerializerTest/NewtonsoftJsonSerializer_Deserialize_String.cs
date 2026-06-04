using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.JsonSerializerTest;

[Collection(nameof(NewtonsoftJsonSerializer_Deserialize_String))]
public class NewtonsoftJsonSerializer_Deserialize_String
{
    private readonly ITestOutputHelper _output;

    public NewtonsoftJsonSerializer_Deserialize_String(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "JsonSerializer Case015_jsonStringをnullの状態で呼び出し")]
    public void JsonSerializer_Case015()
    {
        NewtonsoftJsonSerializer serializer = new();
        var ret = serializer.Deserialize<string>((string?)null);
        Assert.Null(ret);
    }

    [Fact(DisplayName = "JsonSerializer Case016_jsonStringに空文字をセットして呼び出し")]
    public void JsonSerializer_Case016()
    {
        NewtonsoftJsonSerializer serializer = new();
        var ret = serializer.Deserialize<string>("");
        Assert.Null(ret);
    }

    [Fact(DisplayName = "JsonSerializer Case017_jsonStringに値をセットして呼び出し")]
    public void JsonSerializer_Case017()
    {
        NewtonsoftJsonSerializer serializer = new();
        string testData = @"""a\""b\""c\\\\.<d></e>""";
        string expectedData = @"a""b""c\\.<d></e>";
        var ret = serializer.Deserialize<string>(testData);
        Assert.Equal(expectedData, ret);
    }

    [Fact(DisplayName = "JsonSerializer Case018_デシリアライズで予期せぬエラーが発生した場合")]
    public void JsonSerializer_Case018()
    {
        NewtonsoftJsonSerializer serializer = new();
        var ret = serializer.Deserialize<string>("INVALID STRING");
        Assert.Equal(default(string), ret);
    }
}