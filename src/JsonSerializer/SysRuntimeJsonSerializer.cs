using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// System.Runtime のシリアライザのラッパークラス
    /// </summary>
    /// <remarks>
    /// シリアライズ時、/（スラッシュ）、”（ダブルクウォート）、\（バックスラッシュ）は、"\"エスケープされる。
    /// /（スラッシュ）がエスケープされる事でメッセージのupstream時に問題が発生した為、利用時は注意する事。
    /// </remarks>
    public class SysRuntimeJsonSerializer : IJsonSerializer
    {

        /// <summary>
        /// シリアライズ
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <typeparam name="TargetType">対象オブジェクトタイプ</typeparam>
        /// <returns>シリアライズされた文字列</returns>
        public string Serialize<TargetType>(TargetType target)
        {
            string retSerialized = null;

            // シリアライズ実行
            var byteAry = this.SerializeBytes<TargetType>(target);
            if (byteAry != null)
            {
                retSerialized = Encoding.UTF8.GetString(byteAry);
            }
            
            return retSerialized;
        }

        /// <summary>
        /// バイト列シリアライズ
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <typeparam name="TargetType">対象オブジェクトタイプ</typeparam>
        /// <returns>シリアライズされた文字列</returns>
        public Byte[] SerializeBytes<TargetType>(TargetType target)
        {
            Byte[] retSerialized = null;

            if (target != null)
            {
                // System.Runtime のシリアライザを生成
                var serializer = new DataContractJsonSerializer(typeof(TargetType));
                try
                {
                    // シリアライズ実行
                    using (var ms = new MemoryStream())
                    {
                        serializer.WriteObject(ms, target);
                        retSerialized = ms.ToArray();
                    }
                }
                catch (Exception)
                {
                    retSerialized = null;
                }
            }
            
            return retSerialized;
        }

        
        /// <summary>
        /// デシリアライズ
        /// </summary>
        /// <param name="jsonString">対象JSON文字列</param>
        /// <typeparam name="TargetType">デシリアライズ後オブジェクトタイプ</typeparam>
        /// <returns>デシリアライズされたオブジェクト</returns>
        public TargetType Deserialize<TargetType>(string jsonString)
        {
            // デシリアライズ実行
            return this.Deserialize<TargetType>(Encoding.UTF8.GetBytes(jsonString));
        }

        /// <summary>
        /// デシリアライズ
        /// </summary>
        /// <param name="jsonBytes">対象JSONバイトデータ列</param>
        /// <typeparam name="TargetType">デシリアライズ後オブジェクトタイプ</typeparam>
        /// <returns>デシリアライズされたオブジェクト</returns>
        public TargetType Deserialize<TargetType>(Byte[] jsonBytes)
        {
            TargetType retDeserialized = default(TargetType);

            // System.Runtime のシリアライザを生成
            var serializer = new DataContractJsonSerializer(typeof(TargetType));
            try
            {
                // デシリアライズ実行
                using (var ms = new MemoryStream(jsonBytes))
                {
                    retDeserialized = (TargetType)serializer.ReadObject(ms);
                }
            }
            catch (Exception)
            {
                retDeserialized = default(TargetType);
            }

            return retDeserialized; 
        }

    } 
}
