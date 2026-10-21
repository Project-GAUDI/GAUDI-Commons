using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class Util_GetEnvironmentVariable
{
    private const string EnvName = "TEST_VALUE";

    [Fact(DisplayName = "No630_正常系：正常動作の確認_ENV:TEST_VALUE=123の状態で、nameに\"TEST_VALUE\"、Tをintとしてメソッドを呼び出す")]
    public void Case630()
    {
        using var env = new EnvironmentVariableScope(EnvName, "123");

        var result = Util.GetEnvironmentVariable<int>(EnvName, true);

        Assert.Equal(123, result);
    }

    [Fact(DisplayName = "No631_正常系：正常動作の確認_ENV:TEST_VALUE=MQTTの状態で、nameに\"TEST_VALUE\"、TをTransportProtocolとしてメソッドを呼び出す")]
    public void Case631()
    {
        using var env = new EnvironmentVariableScope(EnvName, "MQTT");

        var result = Util.GetEnvironmentVariable<TransportProtocol>(EnvName, true);

        Assert.Equal(TransportProtocol.Mqtt, result);
    }

    [Fact(DisplayName = "No632_正常系：正常動作の確認_ENV:TEST_VALUE=mqttの状態で、nameに\"TEST_VALUE\"、TをTransportProtocolとしてメソッドを呼び出す")]
    public void Case632()
    {
        using var env = new EnvironmentVariableScope(EnvName, "mqtt");

        var result = Util.GetEnvironmentVariable<TransportProtocol>(EnvName, true);

        Assert.Equal(TransportProtocol.Mqtt, result);
    }

    [Fact(DisplayName = "No633_正常系：正常動作の確認_ENV:TEST_VALUEを未指定とし、nameに\"TEST_VALUE\"、isRequiredにfalse、defaultValueに10を指定して、メソッドを呼び出す")]
    public void Case633()
    {
        using var env = new EnvironmentVariableScope(EnvName, null);

        var result = Util.GetEnvironmentVariable<int>(EnvName, false, 10);

        Assert.Equal(10, result);
    }

    [Fact(DisplayName = "No634_異常系：異常動作の確認_ENV:TEST_VALUEを空文字列とし、nameに\"TEST_VALUE\"、isRequiredにtrueを指定して、メソッドを呼び出す")]
    public void Case634()
    {
        using var env = new EnvironmentVariableScope(EnvName, "");

        Assert.Throws<ArgumentException>(() => Util.GetEnvironmentVariable<int>(EnvName, true));
    }

    [Fact(DisplayName = "No635_異常系：異常動作の確認_ENV:TEST_VALUEを未指定とし、nameに\"TEST_VALUE\"、isRequiredにtrueを指定して、メソッドを呼び出す")]
    public void Case635()
    {
        using var env = new EnvironmentVariableScope(EnvName, null);

        Assert.Throws<ArgumentException>(() => Util.GetEnvironmentVariable<int>(EnvName, true));
    }

    [Fact(DisplayName = "No636_異常系：異常動作の確認_ENV:TEST_VALUE=ABCの状態で、nameに\"TEST_VALUE\"、Tをintとしてメソッドを呼び出す")]
    public void Case636()
    {
        using var env = new EnvironmentVariableScope(EnvName, "ABC");

        Assert.Throws<ArgumentException>(() => Util.GetEnvironmentVariable<int>(EnvName, true));
    }

    [Fact(DisplayName = "No637_異常系：異常動作の確認_ENV:TEST_VALUE=Invalidの状態で、nameに\"TEST_VALUE\"、TをTransportProtocolとしてメソッドを呼び出す")]
    public void Case637()
    {
        using var env = new EnvironmentVariableScope(EnvName, "Invalid");

        Assert.Throws<ArgumentException>(() => Util.GetEnvironmentVariable<TransportProtocol>(EnvName, true));
    }

    [Fact(DisplayName = "No638_異常系：異常動作の確認_ENV:TEST_VALUE=999の状態で、nameに\"TEST_VALUE\"、TをTransportProtocolとしてメソッドを呼び出す")]
    public void Case638()
    {
        using var env = new EnvironmentVariableScope(EnvName, "999");

        Assert.Throws<ArgumentException>(() => Util.GetEnvironmentVariable<TransportProtocol>(EnvName, true));
    }

    [Fact(DisplayName = "No639_正常系：正常動作の確認_ENV:TEST_VALUE=1の状態で、nameに\"TEST_VALUE\"、Tをint、conditionにvalue=>value>0を指定して、メソッドを呼び出す")]
    public void Case639()
    {
        using var env = new EnvironmentVariableScope(EnvName, "1");

        var result = Util.GetEnvironmentVariable<int>(EnvName, true, condition: value => value > 0);

        Assert.Equal(1, result);
    }

    [Fact(DisplayName = "No640_異常系：異常動作の確認_ENV:TEST_VALUE=0の状態で、nameに\"TEST_VALUE\"、Tをint、conditionにvalue=>value>0を指定して、メソッドを呼び出す")]
    public void Case640()
    {
        using var env = new EnvironmentVariableScope(EnvName, "0");

        Assert.Throws<ArgumentException>(() => Util.GetEnvironmentVariable<int>(EnvName, true, condition: value => value > 0));
    }

    [Fact(DisplayName = "No641_異常系：異常動作の確認_nameにnullを指定して、メソッドを呼び出す")]
    public void Case641()
    {
        Assert.Throws<ArgumentNullException>(() => Util.GetEnvironmentVariable<int>(null!, true));
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
