using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.JsonSerializerTest;

[Collection(nameof(JsonSerializerFactory_GetDefaultSerializerType))]
public class JsonSerializerFactory_GetDefaultSerializerType
{
    private readonly ITestOutputHelper _output;

    public JsonSerializerFactory_GetDefaultSerializerType(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "JsonSerializer Case005_環境変数でデフォルトのシリアライザが設定されていない場合")]
    public void JsonSerializer_Case005()
    {
        System.Environment.SetEnvironmentVariable("IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER", "");

        JsonSerializerFactory jsonSerializerFactory = new();
        MethodInfo? mi = jsonSerializerFactory.GetType().GetMethod("GetDefaultSerializerType", BindingFlags.NonPublic | BindingFlags.Static);
        SerializerType? result = (SerializerType?)mi!.Invoke(null, []);

        Assert.Equal(SerializerType.NewtonsoftJson, result);
    }

    [Fact(DisplayName = "JsonSerializer Case006_環境変数でデフォルトのシリアライザとしてSysRuntimeSerializationが設定されている場合")]
    public void JsonSerializer_Case006()
    {
        System.Environment.SetEnvironmentVariable("IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER", "SYSRUNTIMESERIALIZATION");

        JsonSerializerFactory jsonSerializerFactory = new();
        MethodInfo? mi = jsonSerializerFactory.GetType().GetMethod("GetDefaultSerializerType", BindingFlags.NonPublic | BindingFlags.Static);
        SerializerType? result = (SerializerType?)mi!.Invoke(null, []);

        Assert.Equal(SerializerType.SysRuntimeSerialization, result);
    }

    [Fact(DisplayName = "JsonSerializer Case034_環境変数でデフォルトのシリアライザとしてSysRuntimeSerializationが設定されている場合")]
    public void JsonSerializer_Case034()
    {
        System.Environment.SetEnvironmentVariable("IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER", "sysruntimeserialization");

        JsonSerializerFactory jsonSerializerFactory = new();
        MethodInfo? mi = jsonSerializerFactory.GetType().GetMethod("GetDefaultSerializerType", BindingFlags.NonPublic | BindingFlags.Static);
        SerializerType? result = (SerializerType?)mi!.Invoke(null, []);

        Assert.Equal(SerializerType.SysRuntimeSerialization, result);
    }

    [Fact(DisplayName = "JsonSerializer Case007_環境変数でデフォルトのシリアライザとしてNewtonsoftJsonが設定されている場合")]
    public void JsonSerializer_Case007()
    {
        System.Environment.SetEnvironmentVariable("IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER", "NEWTONSOFTJSON");

        JsonSerializerFactory jsonSerializerFactory = new();
        MethodInfo? mi = jsonSerializerFactory.GetType().GetMethod("GetDefaultSerializerType", BindingFlags.NonPublic | BindingFlags.Static);
        SerializerType? result = (SerializerType?)mi!.Invoke(null, []);

        Assert.Equal(SerializerType.NewtonsoftJson, result);
    }

    [Fact(DisplayName = "JsonSerializer Case035_環境変数でデフォルトのシリアライザとしてNewtonsoftJsonが設定されている場合")]
    public void JsonSerializer_Case035()
    {
        System.Environment.SetEnvironmentVariable("IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER", "newtonsoftjson");

        JsonSerializerFactory jsonSerializerFactory = new();
        MethodInfo? mi = jsonSerializerFactory.GetType().GetMethod("GetDefaultSerializerType", BindingFlags.NonPublic | BindingFlags.Static);
        SerializerType? result = (SerializerType?)mi!.Invoke(null, []);

        Assert.Equal(SerializerType.NewtonsoftJson, result);
    }

    [Fact(DisplayName = "JsonSerializer Case008_環境変数でデフォルトのシリアライザとしてSysRuntimeSerialization、もしくは、NewtonsoftJson以外が設定されている場合")]
    public void JsonSerializer_Case008()
    {
        System.Environment.SetEnvironmentVariable("IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER", "OTHER");
        var serializer = JsonSerializerFactory.GetJsonSerializer();
        Assert.IsType<NewtonsoftJsonSerializer>(serializer);
    }
}