using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.JsonSerializerTest;

[Collection(nameof(SysRuntimeJsonSerializer_SerializeBytes))]
public class SysRuntimeJsonSerializer_SerializeBytes
{
    private readonly ITestOutputHelper _output;

    public SysRuntimeJsonSerializer_SerializeBytes(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "JsonSerializer Case024_targetをnullの状態で呼び出し")]
    public void JsonSerializer_Case024()
    {
        SysRuntimeJsonSerializer serializer = new();
        byte[]? ret = serializer.SerializeBytes<string?>(null);
        Assert.Null(ret);
    }

    [Fact(DisplayName = "JsonSerializer Case025_targetに値をセットして呼び出し")]
    public void JsonSerializer_Case025()
    {
        string testData = @"a""b""c\\.<d></e>";
        byte[] expectedData = [34, 97, 92, 34, 98, 92, 34, 99, 92, 92, 92, 92, 46, 60, 100, 62, 60, 92, 47, 101, 62, 34];

        SysRuntimeJsonSerializer serializer = new();
        byte[] ret = serializer.SerializeBytes<string>(testData);
        Assert.Equal(expectedData, ret);
    }

    [Fact(DisplayName = "JsonSerializer Case026_targetに値をセットして呼び出し")]
    public void JsonSerializer_Case026()
    {
        SysRuntimeJsonSerializer serializer = new();
        var target = new Case026NonDataContractClass
        {
            Id = 123,
            Name = "Test Name"
        };
        byte[]? ret = serializer.SerializeBytes<Case026NonDataContractClass>(target);
        Assert.Null(ret);
    }
    internal class Case026NonDataContractClass
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}