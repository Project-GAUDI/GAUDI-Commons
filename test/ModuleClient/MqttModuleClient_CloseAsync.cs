using System.Reflection;
using HarmonyLib;
using TICO.GAUDI.Commons;
using uPLibrary.Networking.M2Mqtt;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class MqttModuleClient_CloseAsync
{
    [Fact(DisplayName = "No452_正常系：正常動作の確認_MyMqttClientが接続された状態で、メソッドを呼び出す")]
    public async Task Case452()
    {
        var client = NewMqttModuleClient();
        SetIsConnected(client.MyMqttClient, true);
        using var patch = new DisconnectPatch();

        await client.CloseAsync();

        Assert.Equal(1, DisconnectPatch.CallCount);
    }

    [Fact(DisplayName = "No453_正常系：正常動作の確認_MyMqttClientが接続されていない状態で、メソッドを呼び出す")]
    public async Task Case453()
    {
        var client = NewMqttModuleClient();
        SetIsConnected(client.MyMqttClient, false);
        using var patch = new DisconnectPatch();

        await client.CloseAsync();

        Assert.Equal(1, DisconnectPatch.CallCount);
    }

    private static MqttModuleClient NewMqttModuleClient()
    {
        return new MqttModuleClient("sas-token", "localhost", "device1");
    }

    private static void SetIsConnected(MqttClient client, bool value)
    {
        typeof(MqttClient).GetField("<IsConnected>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(client, value);
    }

    private sealed class DisconnectPatch : IDisposable
    {
        private readonly Harmony harmony;

        public static int CallCount { get; private set; }

        public DisconnectPatch()
        {
            CallCount = 0;

            harmony = new Harmony($"{nameof(MqttModuleClient_CloseAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(MqttClient), nameof(MqttClient.Disconnect)),
                prefix: new HarmonyMethod(typeof(DisconnectPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix()
        {
            CallCount++;
            return false;
        }
    }
}
