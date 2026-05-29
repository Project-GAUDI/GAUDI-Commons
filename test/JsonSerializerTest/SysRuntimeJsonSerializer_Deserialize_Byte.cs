using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.JsonSerializerTest;

[Collection(nameof(SysRuntimeJsonSerializer_Deserialize_Byte))]
public class SysRuntimeJsonSerializer_Deserialize_Byte
{
    private readonly ITestOutputHelper _output;

    public SysRuntimeJsonSerializer_Deserialize_Byte(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "JsonSerializer Case031_バイト配列をセットして呼び出し")]
    public void JsonSerializer_Case031()
    {
        byte[] testData = [34, 97, 92, 34, 98, 92, 34, 99, 92, 92, 92, 92, 46, 60, 100, 62, 60, 92, 47, 101, 62, 34];
        string expectedData = @"a""b""c\\.<d></e>";

        SysRuntimeJsonSerializer serializer = new();
        string ret = serializer.Deserialize<string>(testData);
        Assert.Equal(expectedData, ret);
    }

    [Fact(DisplayName = "JsonSerializer Case032_nullをセットして呼び出し")]
    public void JsonSerializer_Case032()
    {
        SysRuntimeJsonSerializer serializer = new();

        var ret = serializer.Deserialize<string>((byte[]?)null);
        Assert.Equal(default(string), ret);
    }

    [Fact(DisplayName = "JsonSerializer Case033_デシリアライズで予期せぬエラーが発生した場合")]
    public void JsonSerializer_Case033()
    {
        SysRuntimeJsonSerializer serializer = new();

        IotMessage ret = serializer.Deserialize<IotMessage>(new byte[] { });
        Assert.Equal(default(IotMessage), ret);
    }
}