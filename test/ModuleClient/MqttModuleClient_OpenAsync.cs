using HarmonyLib;
using TICO.GAUDI.Commons;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Messages;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(MqttModuleClientHarmonyCollection))]
public sealed class MqttModuleClient_OpenAsync
{
    [Fact(DisplayName = "No455_正常系：正常動作の確認_moduleId=null、MyMqttClient.Connectの結果がCONN_ACCEPTEDになる状態で、メソッドを呼び出す")]
    public async Task Case455()
    {
        using var env = new EnvironmentVariablesScope(("IOTEDGE_IOTHUBHOSTNAME", "myhub.azure-devices.net"));
        var client = new MqttModuleClient("sas-token", "localhost", "device1");
        using var patch = new ConnectPatch(MqttMsgConnack.CONN_ACCEPTED);

        await client.OpenAsync();

        var call = Assert.Single(ConnectPatch.Calls);
        Assert.Equal("device1", call.ClientId);
        Assert.Equal("myhub.azure-devices.net/device1/?api-version=2018-06-30", call.Username);
        Assert.Equal("sas-token", call.Password);
    }

    [Fact(DisplayName = "No456_正常系：正常動作の確認_moduleId!=null、MyMqttClient.Connectの結果がCONN_ACCEPTEDになる状態で、メソッドを呼び出す")]
    public async Task Case456()
    {
        using var env = new EnvironmentVariablesScope(
            ("IOTEDGE_IOTHUBHOSTNAME", "myhub.azure-devices.net"),
            ("IOTEDGE_DEVICEID", "env-device"),
            ("IOTEDGE_MODULEID", "env-module"));
        var client = new MqttModuleClient("sas-token", "localhost");
        using var patch = new ConnectPatch(MqttMsgConnack.CONN_ACCEPTED);

        await client.OpenAsync();

        var call = Assert.Single(ConnectPatch.Calls);
        Assert.Equal("env-device/env-module", call.ClientId);
        Assert.Equal("myhub.azure-devices.net/env-device/env-module/?api-version=2018-06-30", call.Username);
        Assert.Equal("sas-token", call.Password);
    }

    [Fact(DisplayName = "No457_異常系：異常動作の確認_moduleId=null、MyMqttClient.Connectの結果がCONN_ACCEPTED以外になる状態で、メソッドを呼び出す")]
    public async Task Case457()
    {
        using var env = new EnvironmentVariablesScope(("IOTEDGE_IOTHUBHOSTNAME", "myhub.azure-devices.net"));
        var client = new MqttModuleClient("sas-token", "localhost", "device1");
        using var patch = new ConnectPatch(MqttMsgConnack.CONN_REFUSED_NOT_AUTHORIZED);

        await Assert.ThrowsAsync<MqttCommunicationException>(() => client.OpenAsync());
    }

    [Fact(DisplayName = "No458_異常系：異常動作の確認_moduleId!=null、MyMqttClient.Connectの結果がCONN_ACCEPTED以外になる状態で、メソッドを呼び出す")]
    public async Task Case458()
    {
        using var env = new EnvironmentVariablesScope(
            ("IOTEDGE_IOTHUBHOSTNAME", "myhub.azure-devices.net"),
            ("IOTEDGE_DEVICEID", "env-device"),
            ("IOTEDGE_MODULEID", "env-module"));
        var client = new MqttModuleClient("sas-token", "localhost");
        using var patch = new ConnectPatch(MqttMsgConnack.CONN_REFUSED_NOT_AUTHORIZED);

        await Assert.ThrowsAsync<MqttCommunicationException>(() => client.OpenAsync());
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

    private sealed class ConnectPatch : IDisposable
    {
        private readonly Harmony harmony;

        public static List<ConnectCall> Calls { get; } = new();

        private static byte resultToReturn;

        public ConnectPatch(byte result)
        {
            Calls.Clear();
            resultToReturn = result;

            harmony = new Harmony($"{nameof(MqttModuleClient_OpenAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(MqttClient), nameof(MqttClient.Connect), new[] { typeof(string), typeof(string), typeof(string) }),
                prefix: new HarmonyMethod(typeof(ConnectPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(string __0, string __1, string __2, ref byte __result)
        {
            Calls.Add(new ConnectCall(__0, __1, __2));
            __result = resultToReturn;
            return false;
        }
    }

    private sealed record ConnectCall(string ClientId, string Username, string Password);
}
