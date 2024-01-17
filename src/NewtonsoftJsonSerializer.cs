using Newtonsoft.Json;
using System;
using System.Text;


namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Newtonsoft.Json のシリアライザのラッパークラス
    /// </summary>
    /// <remarks>
    /// シリアライズ時”（ダブルクウォート）、\（バックスラッシュ）は、"\"エスケープされる。
    /// /（スラッシュ）は、エスケープされないので、System.Runtimeのシリアライザからの置き換え対象としている。
    /// </remarks>
    public class NewtonsoftJsonSerializer : IJsonSerializer
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
                                         
            try
            {  
                if ( target != null ) 
                {
                    // Newtonsoft.Json のシリアライザ設定を実施
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                    // エスケープオプションをデフォルトに設定。
                    // 参考）
                    // Default: ”（ダブルクウォート）、\（バックスラッシュ）は、"\"エスケープされる。
                    // EscapeHtml： Default + Htmlに関連する記号等をユニコード("\u")エスケープ
                    // EscapeNonAscii： Default + 非Ascii文字列を全てユニコード("\u")エスケープ
                    serializerSettings.StringEscapeHandling = StringEscapeHandling.Default;

                    // シリアライズ実行
                    retSerialized = JsonConvert.SerializeObject(target, serializerSettings);
                }
            }
            catch (Exception )
            {
                retSerialized = null;
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

            // シリアライズ実行
            string serialized = this.Serialize<TargetType>(target);
            if ( serialized != null ) 
            {
                retSerialized =  Encoding.UTF8.GetBytes( serialized );
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
            TargetType retDeserialized = default(TargetType);

            try
            {  
                if ( jsonString != null ) 
                {
                    // デシリアライズ実行
                    retDeserialized = JsonConvert.DeserializeObject<TargetType>(jsonString);
                }
            }
            catch (Exception )
            {
                retDeserialized = default(TargetType);
            }
            
            return retDeserialized; 
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

            // デシリアライズ実行
            if ( jsonBytes != null ) 
            {
                var jsonString =  Encoding.UTF8.GetString( jsonBytes );
                retDeserialized = this.Deserialize<TargetType>(jsonString);
            }

            return retDeserialized; 
        }

    } 
}
