using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.JsonSerializerTest;

[Collection(nameof(SysRuntimeJsonSerializer_Deserialize_String))]
public class SysRuntimeJsonSerializer_Deserialize_String
{
    private readonly ITestOutputHelper _output;

    public SysRuntimeJsonSerializer_Deserialize_String(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "JsonSerializer Case027_jsonStringをnullの状態で呼び出し")]
    public void JsonSerializer_Case027()
    {
        SysRuntimeJsonSerializer serializer = new();
        Assert.ThrowsAny<Exception>(() =>
        {
            serializer.Deserialize<string>((string?)null);
        });
    }

    [Fact(DisplayName = "JsonSerializer Case028_jsonStringに空文字をセットして呼び出し")]
    public void JsonSerializer_Case028()
    {
        SysRuntimeJsonSerializer serializer = new();
        var ret = serializer.Deserialize<string>("");
        Assert.Equal(default(string), ret);
    }

    [Fact(DisplayName = "JsonSerializer Case029_jsonStringに値をセットして呼び出し")]
    public void JsonSerializer_Case029()
    {
        SysRuntimeJsonSerializer serializer = new();
        string testData = @"""a\""b\""c\\\\.<d><\/e>""";
        string expectedData = @"a""b""c\\.<d></e>";
        var ret = serializer.Deserialize<string>(testData);
        Assert.Equal(expectedData, ret);
    }

    [Fact(DisplayName = "JsonSerializer Case030_デシリアライズで予期せぬエラーが発生した場合")]
    public void JsonSerializer_Case030()
    {
        SysRuntimeJsonSerializer serializer = new();
        var ret = serializer.Deserialize<string>("INVALID STRING");
        Assert.Equal(default(string), ret);
    }
}