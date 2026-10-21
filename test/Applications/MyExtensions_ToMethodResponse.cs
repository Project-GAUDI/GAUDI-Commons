using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// 既定シリアライザを決める環境変数を操作するため、JsonSerializerFactoryのテストと直列化する
[Collection(nameof(SharedLoggerState))]
public sealed class MyExtensions_ToMethodResponse
{
    private const string SerializerEnvName = "IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER";

    [Fact(DisplayName = "No750_正常系：正常動作の確認_thisMethodResponseにStatusおよび1件以上のResultsを設定した状態で、メソッドを呼び出す")]
    public void Case750()
    {
        using var env = new EnvironmentVariableScope(SerializerEnvName, null);
        var response = new DirectMethodResponse(200, "Result", "OK");
        response.Results.Add("Count", 1);

        var result = response.ToMethodResponse();

        Assert.Equal(200, result.Status);
        Assert.Equal(Encoding.UTF8.GetBytes("{\"Result\":\"OK\",\"Count\":1}"), result.Result);
    }

    [Fact(DisplayName = "No751_正常系：正常動作の確認_thisMethodResponseのResultsが空のDictionaryの状態で、メソッドを呼び出す")]
    public void Case751()
    {
        using var env = new EnvironmentVariableScope(SerializerEnvName, null);
        var response = new DirectMethodResponse(0);

        var result = response.ToMethodResponse();

        Assert.Equal(0, result.Status);
        Assert.Equal(Encoding.UTF8.GetBytes("{}"), result.Result);
    }

    [Fact(DisplayName = "No752_異常系：異常動作の確認_thisMethodResponseにnullを指定して、メソッドを呼び出す")]
    public void Case752()
    {
        DirectMethodResponse? response = null;

        Assert.Throws<NullReferenceException>(() => response!.ToMethodResponse());
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
