using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotHubModuleClient_OpenAsync
{
    [Fact(DisplayName = "No418_正常系：正常動作の確認_メソッドを呼び出す")]
    public async Task Case418()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient);

        await client.OpenAsync();

        Assert.Equal(1, ModuleClientPatch.OpenAsyncCallCount);
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

        public static int OpenAsyncCallCount { get; private set; }

        public ModuleClientPatch()
        {
            OpenAsyncCallCount = 0;
            ModuleClient = (ModuleClient)RuntimeHelpers.GetUninitializedObject(typeof(ModuleClient));

            harmony = new Harmony($"{nameof(IotHubModuleClient_OpenAsync)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.OpenAsync), Type.EmptyTypes),
                prefix: new HarmonyMethod(typeof(ModuleClientPatch), nameof(OpenAsyncPrefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool OpenAsyncPrefix(ref Task __result)
        {
            OpenAsyncCallCount++;
            __result = Task.CompletedTask;
            return false;
        }
    }
}