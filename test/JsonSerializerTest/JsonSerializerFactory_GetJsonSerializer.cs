using Xunit;

namespace TICO.GAUDI.Commons.Test.JsonSerializerTest;

[Collection("JsonSerializerFactory")]
public class JsonSerializerFactory_GetJsonSerializer
{
    private readonly ITestOutputHelper _output;

    public JsonSerializerFactory_GetJsonSerializer(ITestOutputHelper output)
    {
        System.Environment.SetEnvironmentVariable("IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER", null);
        _output = output;
    }

    [Fact(DisplayName = "JsonSerializer Case001_環境変数(IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER)が未指定、かつ引数でデフォルトのシリアライザ(SerializerType.Default)が指定された場合")]
    public void JsonSerializer_Case001()
    {
        var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.Default);
        Assert.IsType<NewtonsoftJsonSerializer>(serializer);
    }

    [Fact(DisplayName = "JsonSerializer Case002_環境変数(IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER)が未指定、かつ引数でシリアライザが指定されなかった場合")]
    public void JsonSerializer_Case002()
    {
        var serializer = JsonSerializerFactory.GetJsonSerializer();
        _output.WriteLine($"env={Environment.GetEnvironmentVariable("IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER")}");
        Assert.IsType<NewtonsoftJsonSerializer>(serializer);
    }

    [Fact(DisplayName = "JsonSerializer Case003_環境変数(IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER)が未指定、かつ引数でSysRuntimeSerializationのシリアライザが指定された場合")]
    public void JsonSerializer_Case003()
    {
        var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.SysRuntimeSerialization);
        Assert.IsType<SysRuntimeJsonSerializer>(serializer);
    }

    [Fact(DisplayName = "JsonSerializer Case004_環境変数(IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER)が未指定、かつ引数でNewtonsoftJsonのシリアライザが指定された場合")]
    public void JsonSerializer_Case004()
    {
        var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.NewtonsoftJson);
        Assert.IsType<NewtonsoftJsonSerializer>(serializer);
    }
}