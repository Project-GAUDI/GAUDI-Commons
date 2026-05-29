using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.JsonSerializerTest;

[Collection(nameof(SysRuntimeJsonSerializer_Serialize))]
public class SysRuntimeJsonSerializer_Serialize
{
    private readonly ITestOutputHelper _output;

    public SysRuntimeJsonSerializer_Serialize(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "JsonSerializer Case021_targetをnullの状態で呼び出し")]
    public void JsonSerializer_Case021()
    {
        SysRuntimeJsonSerializer serializer = new();
        string? ret = serializer.Serialize<string?>(null);
        Assert.Null(ret);
    }

    [Fact(DisplayName = "JsonSerializer Case022_targetに値をセットして呼び出し")]
    public void JsonSerializer_Case022()
    {
        SysRuntimeJsonSerializer serializer = new();
        string testData = @"a""b""c\\.<d></e>";
        string expectedData = @"""a\""b\""c\\\\.<d><\/e>""";
        var ret = serializer.Serialize<string>(testData);
        Assert.Equal(expectedData, ret);
    }
}