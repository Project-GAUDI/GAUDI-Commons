using HarmonyLib;

namespace TICO.GAUDI.Commons.Test.MessageTest.JsonMessageTest.Stub;

/// <summary>
/// JsonMessage が依存する JsonSerializerFactory をスタブ化するためのパッチ
/// </summary>
/// <remarks>
/// 改修時は<see href="https://harmony.pardeike.net/articles/intro.html">ドキュメント</see>参照
/// </remarks>
[HarmonyPatch(typeof(JsonSerializerFactory))]
public class JsonSerializerFactoryPatch
{
    /// <summary>
    /// GetJsonSerializer が呼ばれる前に自動的に差し込まれ、常に例外をスローする
    /// JsonSerializerFactory の異常挙動をシミュレートするために利用
    /// </summary>
    [HarmonyPrefix, HarmonyPatch(nameof(JsonSerializerFactory.GetJsonSerializer))]
    public static bool ThrowExceptionOnGetJsonSerializerPrefix()
    {
        throw new Exception("Throw Exception From Mocking JsonSerializerFactory");
    }
}
