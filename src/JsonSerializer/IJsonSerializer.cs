using System;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// JSONシリアライザインターフェース
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// シリアライズ
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <typeparam name="TargetType">対象オブジェクトタイプ</typeparam>
        /// <returns>シリアライズされた文字列</returns>
        string Serialize<TargetType>(TargetType target);
        
        /// <summary>
        /// バイト列シリアライズ
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <typeparam name="TargetType">対象オブジェクトタイプ</typeparam>
        /// <returns>シリアライズされた文字列</returns>
        Byte[] SerializeBytes<TargetType>(TargetType target);
        
        /// <summary>
        /// デシリアライズ
        /// </summary>
        /// <param name="jsonString">対象JSON文字列</param>
        /// <typeparam name="TargetType">デシリアライズ後オブジェクトタイプ</typeparam>
        /// <returns>デシリアライズされたオブジェクト</returns>
        TargetType Deserialize<TargetType>(string jsonString);

        /// <summary>
        /// デシリアライズ
        /// </summary>
        /// <param name="jsonBytes">対象JSONバイトデータ列</param>
        /// <typeparam name="TargetType">デシリアライズ後オブジェクトタイプ</typeparam>
        /// <returns>デシリアライズされたオブジェクト</returns>
        TargetType Deserialize<TargetType>(Byte[] jsonBytes);
    } 
}
