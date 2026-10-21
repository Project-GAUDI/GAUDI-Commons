using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(IotHubModuleClientHarmonyCollection))]
public sealed class IotHubModuleClient_SetConnectionStatusChangedHandlerAsync
{
    [Fact(DisplayName = "No438_正常系：正常動作の確認_handlerを指定して、メソッドを呼び出す")]
    public async Task Case438()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient);
        var received = new List<(IotConnectionStatus Status, IotConnectionStatusChangeReason Reason)>();
        Task Handler(IotConnectionStatus status, IotConnectionStatusChangeReason reason)
        {
            received.Add((status, reason));
            return Task.CompletedTask;
        }

        var task = client.SetConnectionStatusChangedHandlerAsync(Handler);
        await task;

        Assert.True(task.IsCompletedSuccessfully);
        var registeredHandler = Assert.Single(ModuleClientPatch.Calls);
        Assert.NotNull(registeredHandler);

        registeredHandler(ConnectionStatus.Connected, ConnectionStatusChangeReason.Connection_Ok);

        var received1 = Assert.Single(received);
        Assert.Equal((IotConnectionStatus)ConnectionStatus.Connected, received1.Status);
        Assert.Equal((IotConnectionStatusChangeReason)ConnectionStatusChangeReason.Connection_Ok, received1.Reason);
        Assert.Equal((IotConnectionStatus)ConnectionStatus.Connected, client.ConnectionStatus);
    }

    private static IotHubModuleClient NewIotHubModuleClient(ModuleClient moduleClient)
    {
        var client = (IotHubModuleClient)Activator.CreateInstance(
            typeof(IotHubModuleClient),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: Array.Empty<object>(),
            culture: null)!;

        typeof(IotHubModuleClient).GetProperty("MyModuleClient", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(client, moduleClient);
        return client;
    }

    private sealed class ModuleClientPatch : IDisposable
    {
        private readonly Harmony harmony;

        public ModuleClient ModuleClient { get; }

        public static List<ConnectionStatusChangesHandler> Calls { get; } = new();

        public ModuleClientPatch()
        {
            Calls.Clear();
            ModuleClient = (ModuleClient)RuntimeHelpers.GetUninitializedObject(typeof(ModuleClient));

            harmony = new Harmony($"{nameof(IotHubModuleClient_SetConnectionStatusChangedHandlerAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.SetConnectionStatusChangesHandler), new[] { typeof(ConnectionStatusChangesHandler) }),
                prefix: new HarmonyMethod(typeof(ModuleClientPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(ConnectionStatusChangesHandler __0)
        {
            Calls.Add(__0);
            return false;
        }
    }
}
