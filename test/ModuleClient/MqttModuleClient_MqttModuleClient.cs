using System.Reflection;
using TICO.GAUDI.Commons;
using uPLibrary.Networking.M2Mqtt;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(MqttModuleClientHarmonyCollection))]
public sealed class MqttModuleClient_MqttModuleClient
{
    [Fact(DisplayName = "No447_正常系：正常動作の確認_sas、hostName、deviceを指定して、コンストラクタを呼び出す")]
    public void Case447()
    {
        using var env = new EnvironmentVariablesScope(
            ("IOTEDGE_GATEWAYHOSTNAME", null),
            ("IOTEDGE_DEVICEID", null),
            ("IOTEDGE_MODULEID", null));

        var client = new MqttModuleClient("sas-token", "localhost", "device1");

        Assert.NotNull(client.MyMqttClient);
        Assert.Equal("localhost", GetBrokerHostName(client.MyMqttClient));
    }

    [Fact(DisplayName = "No448_正常系：正常動作の確認_sas、hostNameを指定して、コンストラクタを呼び出す")]
    public void Case448()
    {
        using var env = new EnvironmentVariablesScope(
            ("IOTEDGE_GATEWAYHOSTNAME", null),
            ("IOTEDGE_DEVICEID", "env-device"),
            ("IOTEDGE_MODULEID", null));

        var client = new MqttModuleClient("sas-token", "localhost");

        Assert.NotNull(client.MyMqttClient);
        Assert.Equal("localhost", GetBrokerHostName(client.MyMqttClient));
        Assert.Equal("env-device", GetPrivateField(client, "deviceId"));
    }

    [Fact(DisplayName = "No449_正常系：正常動作の確認_sasを指定して、コンストラクタを呼び出す")]
    public void Case449()
    {
        using var env = new EnvironmentVariablesScope(
            ("IOTEDGE_GATEWAYHOSTNAME", "localhost"),
            ("IOTEDGE_DEVICEID", "env-device"),
            ("IOTEDGE_MODULEID", "env-module"));

        var client = new MqttModuleClient("sas-token");

        Assert.NotNull(client.MyMqttClient);
        Assert.Equal("localhost", GetBrokerHostName(client.MyMqttClient));
        Assert.Equal("env-device", GetPrivateField(client, "deviceId"));
        Assert.Equal("env-module", GetPrivateField(client, "moduleId"));
    }

    [Fact(DisplayName = "No450_正常系：正常動作の確認_環境変数が設定された状態で、sas、deviceを指定して、コンストラクタを呼び出す")]
    public void Case450()
    {
        using var env = new EnvironmentVariablesScope(
            ("IOTEDGE_GATEWAYHOSTNAME", "localhost"),
            ("IOTEDGE_DEVICEID", "env-device"),
            ("IOTEDGE_MODULEID", "env-module"));

        var client = new MqttModuleClient("sas-token", device: "device1");

        Assert.NotNull(client.MyMqttClient);
        Assert.Equal("localhost", GetBrokerHostName(client.MyMqttClient));
        Assert.Equal("device1", GetPrivateField(client, "deviceId"));
        Assert.Null(GetPrivateField(client, "moduleId"));
    }

    [Fact(DisplayName = "No451_異常系：異常動作の確認_環境変数が設定されていない状態で、sas、deviceを指定して、コンストラクタを呼び出す")]
    public void Case451()
    {
        using var env = new EnvironmentVariablesScope(
            ("IOTEDGE_GATEWAYHOSTNAME", null),
            ("IOTEDGE_DEVICEID", null),
            ("IOTEDGE_MODULEID", null));

        Assert.Throws<ArgumentNullException>(() => new MqttModuleClient("sas-token", device: "device1"));
    }

    private static string? GetBrokerHostName(MqttClient client)
    {
        var field = typeof(MqttClient).GetField("brokerHostName", BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (string?)field.GetValue(client);
    }

    private static string? GetPrivateField(MqttModuleClient client, string name)
    {
        var field = typeof(MqttModuleClient).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (string?)field.GetValue(client);
    }

    private sealed class EnvironmentVariablesScope : IDisposable
    {
        private readonly List<(string Name, string? OriginalValue)> originalValues = new();

        public EnvironmentVariablesScope(params (string Name, string? Value)[] variables)
        {
            foreach (var (name, value) in variables)
            {
                originalValues.Add((name, Environment.GetEnvironmentVariable(name)));
                Environment.SetEnvironmentVariable(name, value);
            }
        }

        public void Dispose()
        {
            foreach (var (name, originalValue) in originalValues)
            {
                Environment.SetEnvironmentVariable(name, originalValue);
            }
        }
    }
}
