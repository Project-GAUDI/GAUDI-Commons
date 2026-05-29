using HarmonyLib;
using System.Reflection;
using TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest.Stub;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest;

public abstract class MqttModuleClientTesterBase: IDisposable
{
    protected const string ENV_IOTEDGE_GATEWAYHOSTNAME = "IOTEDGE_GATEWAYHOSTNAME";

    protected const string ENV_IOTEDGE_IOTHUBHOSTNAME = "IOTEDGE_IOTHUBHOSTNAME";

    protected const string ENV_IOTEDGE_DEVICEID = "IOTEDGE_DEVICEID";

    protected const string ENV_IOTEDGE_MODULEID = "IOTEDGE_MODULEID";

    protected const string FI_PATCH_CALLBACK = "patchCallback";

    protected const string FI_INPUT_NAMES = "inputNames";

    protected const string FI_DICT_CALLBACK_PROPS = "dictCallbackProps";

    protected readonly string _sas = $"token-{Guid.NewGuid()}";

    protected readonly string _gatewayHostName = $"{Guid.NewGuid()}.dummy-gateway.org";

    protected readonly string _iothubHostName = $"{Guid.NewGuid()}.dummy-iothub.org";

    protected readonly string _deviceId = $"device-{Guid.NewGuid()}";

    protected readonly string _moduleId = $"module-{Guid.NewGuid()}";

    protected readonly Harmony _harmony = new (Guid.NewGuid().ToString());

    protected MqttModuleClientTesterBase()
    {
        MockMqttClient();
    }

    private void MockMqttClient()
    {
        MqttClientPatch.PatchConstructor(_harmony);
        MqttClientPatch.PatchSubscribeMethod(_harmony);
    }

    private static FieldInfo GetFieldInfo(string field)
    {
        return typeof(MqttModuleClient).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    protected static T? GetFieldValue<T>(MqttModuleClient client, string field)
    {
        return (T?)GetFieldInfo(field).GetValue(client);
    }

    protected static void SetFieldValue<T>(MqttModuleClient client, string field, T value)
    {
        GetFieldInfo(field).SetValue(client, value);
    }

    protected static string? GetDeviceId(MqttModuleClient client)
    {
        return GetFieldValue<string?>(client, "deviceId");
    }

    protected static string? GetModuleId(MqttModuleClient client)
    {
        return GetFieldValue<string?>(client, "moduleId");
    }

    public virtual void Dispose()
    {
        MqttClientProxy.Clear();
        _harmony.UnpatchAll(_harmony.Id);

        Environment.SetEnvironmentVariable(ENV_IOTEDGE_GATEWAYHOSTNAME, null);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_IOTHUBHOSTNAME, null);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, null);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, null);
    }
}
