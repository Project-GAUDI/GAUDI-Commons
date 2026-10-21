using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotHubModuleClient_UpdateReportedPropertiesAsync
{
    [Fact(DisplayName = "No435_正常系：正常動作の確認_defaultSendTopic=Iothubの状態で、reportedPropertiesを指定して、メソッドを呼び出す")]
    public async Task Case435()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Iothub);
        var reportedProperties = new TwinCollection();

        await client.UpdateReportedPropertiesAsync(reportedProperties);

        var call = Assert.Single(ModuleClientPatch.Calls);
        Assert.Same(reportedProperties, call);
    }

    [Fact(DisplayName = "No436_正常系：正常動作の確認_reportedProperties、transportTopic=Iothubを指定して、メソッドを呼び出す")]
    public async Task Case436()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Mqtt);
        var reportedProperties = new TwinCollection();

        await InvokeProtectedUpdateReportedPropertiesAsync(client, reportedProperties, TransportTopic.Iothub);

        var call = Assert.Single(ModuleClientPatch.Calls);
        Assert.Same(reportedProperties, call);
    }

    [Fact(DisplayName = "No437_異常系：異常動作の確認_reportedProperties、transportTopic=Mqttを指定して、メソッドを呼び出す")]
    public async Task Case437()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient, TransportTopic.Iothub);
        var reportedProperties = new TwinCollection();

        await Assert.ThrowsAsync<NotImplementedException>(
            () => InvokeProtectedUpdateReportedPropertiesAsync(client, reportedProperties, TransportTopic.Mqtt));
    }

    private static IotHubModuleClient NewIotHubModuleClient(ModuleClient moduleClient, TransportTopic defaultSendTopic)
    {
        var client = (IotHubModuleClient)Activator.CreateInstance(
            typeof(IotHubModuleClient),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: Array.Empty<object>(),
            culture: null)!;

        typeof(IotHubModuleClient).GetProperty("MyModuleClient", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(client, moduleClient);
        typeof(IotHubModuleClient).GetField("defaultSendTopic", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(client, defaultSendTopic);
        return client;
    }

    private static Task InvokeProtectedUpdateReportedPropertiesAsync(IotHubModuleClient client, TwinCollection reportedProperties, TransportTopic transportTopic)
    {
        var method = typeof(IotHubModuleClient).GetMethod(
            "UpdateReportedPropertiesAsync",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(TwinCollection), typeof(TransportTopic) },
            modifiers: null)!;
        return (Task)method.Invoke(client, new object[] { reportedProperties, transportTopic })!;
    }

    private sealed class ModuleClientPatch : IDisposable
    {
        private readonly Harmony harmony;

        public ModuleClient ModuleClient { get; }

        public static List<TwinCollection> Calls { get; } = new();

        public ModuleClientPatch()
        {
            Calls.Clear();
            ModuleClient = (ModuleClient)RuntimeHelpers.GetUninitializedObject(typeof(ModuleClient));

            harmony = new Harmony($"{nameof(IotHubModuleClient_UpdateReportedPropertiesAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.UpdateReportedPropertiesAsync), new[] { typeof(TwinCollection) }),
                prefix: new HarmonyMethod(typeof(ModuleClientPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(TwinCollection __0, ref Task __result)
        {
            Calls.Add(__0);
            __result = Task.CompletedTask;
            return false;
        }
    }
}
