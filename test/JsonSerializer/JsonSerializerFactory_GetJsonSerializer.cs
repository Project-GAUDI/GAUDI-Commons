using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// 既定シリアライザの環境変数を変更するため、既定シリアライザを使うApplicationEngine系テストと直列化する
[Collection(nameof(SharedLoggerState))]
public sealed class JsonSerializerFactory_GetJsonSerializer
{
    private const string EnvName = "IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER";

    [Fact(DisplayName = "No502_正常系：正常動作の確認_serializerTypeにSysRuntimeSerializationを指定して、メソッドを呼び出す")]
    public void Case502()
    {
        var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.SysRuntimeSerialization);

        Assert.IsType<SysRuntimeJsonSerializer>(serializer);
    }

    [Fact(DisplayName = "No503_正常系：正常動作の確認_serializerTypeにNewtonsoftJsonを指定して、メソッドを呼び出す")]
    public void Case503()
    {
        var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.NewtonsoftJson);

        Assert.IsType<NewtonsoftJsonSerializer>(serializer);
    }

    [Fact(DisplayName = "No504_正常系：デフォルト値の確認_ENV:IOTEDGE_COMMON_DEFAULT_JSONSERIALIZERを未指定とし、serializerTypeを省略してメソッドを呼び出す")]
    public void Case504()
    {
        using var env = new EnvironmentVariableScope(EnvName, null);

        var serializer = JsonSerializerFactory.GetJsonSerializer();

        Assert.IsType<NewtonsoftJsonSerializer>(serializer);
    }

    [Fact(DisplayName = "No505_正常系：正常値の確認（string型）_ENV:IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER=SYSRUNTIMESERIALIZATIONの状態で、serializerTypeにDefaultを指定してメソッドを呼び出す")]
    public void Case505()
    {
        using var env = new EnvironmentVariableScope(EnvName, "SYSRUNTIMESERIALIZATION");

        var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.Default);

        Assert.IsType<SysRuntimeJsonSerializer>(serializer);
    }

    [Fact(DisplayName = "No506_正常系：正常値の確認（string型）_ENV:IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER=NEWTONSOFTJSONの状態で、serializerTypeにDefaultを指定してメソッドを呼び出す")]
    public void Case506()
    {
        using var env = new EnvironmentVariableScope(EnvName, "NEWTONSOFTJSON");

        var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.Default);

        Assert.IsType<NewtonsoftJsonSerializer>(serializer);
    }

    [Fact(DisplayName = "No507_正常系：正常値の確認（string型）_ENV:IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER=sysruntimeserializationの状態で、serializerTypeにDefaultを指定してメソッドを呼び出す")]
    public void Case507()
    {
        using var env = new EnvironmentVariableScope(EnvName, "sysruntimeserialization");

        var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.Default);

        Assert.IsType<SysRuntimeJsonSerializer>(serializer);
    }

    [Fact(DisplayName = "No508_異常系：不正値の確認（string型）_ENV:IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER=INVALIDの状態で、serializerTypeにDefaultを指定してメソッドを呼び出す")]
    public void Case508()
    {
        using var env = new EnvironmentVariableScope(EnvName, "INVALID");

        var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.Default);

        Assert.IsType<NewtonsoftJsonSerializer>(serializer);
    }

    [Fact(DisplayName = "No509_異常系：異常動作の確認_serializerTypeに未定義の列挙値を指定して、メソッドを呼び出す")]
    public void Case509()
    {
        var serializer = JsonSerializerFactory.GetJsonSerializer((SerializerType)999);

        Assert.Null(serializer);
    }

    private sealed class EnvironmentVariableScope : IDisposable
    {
        private readonly string name;
        private readonly string? originalValue;

        public EnvironmentVariableScope(string name, string? value)
        {
            this.name = name;
            originalValue = Environment.GetEnvironmentVariable(name);
            Environment.SetEnvironmentVariable(name, value);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(name, originalValue);
        }
    }
}
