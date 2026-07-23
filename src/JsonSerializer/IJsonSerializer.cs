using System;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// JSONシリアライザインターフェース
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// オブジェクトを JSON 文字列にシリアライズする。
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <typeparam name="TargetType">対象オブジェクトタイプ</typeparam>
        /// <returns>シリアライズされた文字列</returns>
        string Serialize<TargetType>(TargetType target);

        /// <summary>
        /// オブジェクトを JSON バイト配列にシリアライズする。
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <typeparam name="TargetType">対象オブジェクトタイプ</typeparam>
        /// <returns>シリアライズされたバイト配列</returns>
        Byte[] SerializeBytes<TargetType>(TargetType target);

        /// <summary>
        /// JSON 文字列をオブジェクトにデシリアライズする。
        /// </summary>
        /// <param name="jsonString">対象JSON文字列</param>
        /// <typeparam name="TargetType">デシリアライズ後オブジェクトタイプ</typeparam>
        /// <returns>デシリアライズされたオブジェクト</returns>
        TargetType Deserialize<TargetType>(string jsonString);

        /// <summary>
        /// JSON バイト配列をオブジェクトにデシリアライズする。
        /// </summary>
        /// <param name="jsonBytes">対象JSONバイト配列</param>
        /// <typeparam name="TargetType">デシリアライズ後オブジェクトタイプ</typeparam>
        /// <returns>デシリアライズされたオブジェクト</returns>
        TargetType Deserialize<TargetType>(Byte[] jsonBytes);

        /// <summary>
        /// 指定した設定でオブジェクトを JSON 文字列にシリアライズする。
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <param name="settings">シリアライズ設定</param>
        /// <typeparam name="TargetType">対象オブジェクトタイプ</typeparam>
        /// <returns>シリアライズされた文字列</returns>
        string Serialize<TargetType>(TargetType target, JsonSerializerSettings settings);

        /// <summary>
        /// 指定した設定でオブジェクトを JSON バイト配列にシリアライズする。
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <param name="settings">シリアライズ設定</param>
        /// <typeparam name="TargetType">対象オブジェクトタイプ</typeparam>
        /// <returns>シリアライズされたバイト配列</returns>
        Byte[] SerializeBytes<TargetType>(TargetType target, JsonSerializerSettings settings);

        /// <summary>
        /// 指定した設定で JSON 文字列をオブジェクトにデシリアライズする。
        /// </summary>
        /// <param name="jsonString">対象 JSON 文字列</param>
        /// <param name="settings">デシリアライズ設定</param>
        /// <typeparam name="TargetType">デシリアライズ後オブジェクトタイプ</typeparam>
        /// <returns>デシリアライズされたオブジェクト</returns>
        TargetType Deserialize<TargetType>(string jsonString, JsonSerializerSettings settings);

        /// <summary>
        /// 指定した設定で JSON バイト配列をオブジェクトにデシリアライズする。
        /// </summary>
        /// <param name="jsonBytes">対象 JSON バイト配列</param>
        /// <param name="settings">デシリアライズ設定</param>
        /// <typeparam name="TargetType">デシリアライズ後オブジェクトタイプ</typeparam>
        /// <returns>デシリアライズされたオブジェクト</returns>
        TargetType Deserialize<TargetType>(Byte[] jsonBytes, JsonSerializerSettings settings);

    }
}
