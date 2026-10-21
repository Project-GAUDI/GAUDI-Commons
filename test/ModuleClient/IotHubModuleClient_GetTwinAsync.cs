using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotHubModuleClient_GetTwinAsync
{
    [Fact(DisplayName = "No419_正常系：正常動作の確認_メソッドを呼び出す")]
    public async Task Case419()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient);

        var result = await client.GetTwinAsync();

        Assert.Equal(1, ModuleClientPatch.GetTwinAsyncCallCount);
        Assert.Same(ModuleClientPatch.Twin, result);
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

        public static int GetTwinAsyncCallCount { get; private set; }

        public static Twin Twin { get; private set; } = null!;

        public ModuleClientPatch()
        {
            GetTwinAsyncCallCount = 0;
            Twin = new Twin();
            ModuleClient = (ModuleClient)RuntimeHelpers.GetUninitializedObject(typeof(ModuleClient));

            harmony = new Harmony($"{nameof(IotHubModuleClient_GetTwinAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.GetTwinAsync), Type.EmptyTypes),
                prefix: new HarmonyMethod(typeof(ModuleClientPatch), nameof(GetTwinAsyncPrefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool GetTwinAsyncPrefix(ref Task<Twin> __result)
        {
            GetTwinAsyncCallCount++;
            __result = Task.FromResult(Twin);
            return false;
        }
    }
}