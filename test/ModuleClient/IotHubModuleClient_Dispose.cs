using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotHubModuleClient_Dispose
{
    [Fact(DisplayName = "No417_正常系：正常動作の確認_メソッドを呼び出す")]
    public void Case417()
    {
        using var patch = new ModuleClientPatch();
        var client = NewIotHubModuleClient(patch.ModuleClient);

        client.Dispose();

        Assert.Equal(1, ModuleClientPatch.DisposeCallCount);
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

        public static int DisposeCallCount { get; private set; }

        public ModuleClientPatch()
        {
            DisposeCallCount = 0;
            ModuleClient = (ModuleClient)RuntimeHelpers.GetUninitializedObject(typeof(ModuleClient));

            harmony = new Harmony($"{nameof(IotHubModuleClient_Dispose)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClient), nameof(ModuleClient.Dispose), Type.EmptyTypes),
                prefix: new HarmonyMethod(typeof(ModuleClientPatch), nameof(DisposePrefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool DisposePrefix()
        {
            DisposeCallCount++;
            return false;
        }
    }
}