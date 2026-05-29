using HarmonyLib;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest.Stub;

public static class MqttClientPatch
{
    public static void PatchConstructor(Harmony harmony)
    {
        var original = AccessTools.Constructor(typeof(MqttClient), [ typeof(string) ]);
        harmony.Patch(original, prefix: new HarmonyMethod(typeof(MqttClientPatch), nameof(MqttClientConstructorPrefix)));
    }

    public static void PatchSubscribeMethod(Harmony harmony)
    {
        var subscribeMethod = AccessTools.Method(typeof(MqttClient), nameof(MqttClient.Subscribe), [ typeof(string[]), typeof(byte[]) ]);
        harmony.Patch(subscribeMethod,
            prefix: new HarmonyMethod(typeof(MqttClientPatch), nameof(SubscribePrefix)),
            postfix: new HarmonyMethod(typeof(MqttClientPatch), nameof(SubscribePostfix)));
    }

    public static void PatchConnectMethod(Harmony harmony, bool willConnected = true)
    {
        var connectMethod = AccessTools.Method(typeof(MqttClient), nameof(MqttClient.Connect), [ typeof(string), typeof(string), typeof(string) ]);
        if (willConnected)
            harmony.Patch(connectMethod,
                prefix: new HarmonyMethod(typeof(MqttClientPatch), nameof(ConnectPrefix)),
                postfix: new HarmonyMethod(typeof(MqttClientPatch), nameof(ConnectSuccessPostfix)));
        else
            harmony.Patch(connectMethod,
                prefix: new HarmonyMethod(typeof(MqttClientPatch), nameof(ConnectPrefix)),
                postfix: new HarmonyMethod(typeof(MqttClientPatch), nameof(ConnectFailPostfix)));
    }

    public static void PatchPublishMethod(Harmony harmony)
    {
        var publishMethod = AccessTools.Method(typeof(MqttClient), nameof(MqttClient.Publish), [ typeof(string), typeof(byte[]) ]);
        harmony.Patch(publishMethod, prefix: new HarmonyMethod(typeof(MqttClientPatch), nameof(PublishPrefix)));
    }

    [HarmonyPrefix]
    private static bool MqttClientConstructorPrefix(string brokerHostName)
    {
        if (string.IsNullOrEmpty(brokerHostName))
            throw new ArgumentNullException(nameof(brokerHostName), "BrokerHostName is empty from mocking client");

        // 実在しないホスト名でのインスタンス作成ができないため、引数を保管
        MqttClientProxy.SaveConstructorArguments(brokerHostName);
        return false;
    }

    [HarmonyPrefix]
    public static bool SubscribePrefix(string[] topics, byte[] qosLevels)
    {
        MqttClientProxy.StackSubscribeCall(topics, qosLevels);
        return false;
    }

    [HarmonyPostfix]
    public static void SubscribePostfix(ref ushort __result)
    {
        __result = 0;
    }

    [HarmonyPrefix]
    public static bool ConnectPrefix(string clientId, string username, string password)
    {
        MqttClientProxy.SaveConnectArguments(clientId, username, password);
        return false;
    }

    [HarmonyPostfix]
    public static void ConnectSuccessPostfix(ref byte __result)
    {
        __result = MqttMsgConnack.CONN_ACCEPTED;
    }

    [HarmonyPostfix]
    public static void ConnectFailPostfix(ref byte __result)
    {
        __result = MqttMsgConnack.CONN_REFUSED_IDENT_REJECTED;
    }

    [HarmonyPrefix]
    public static bool PublishPrefix(string topic, byte[] message)
    {
        MqttClientProxy.SavePublishArguments(topic, message);
        return false;
    }
}
