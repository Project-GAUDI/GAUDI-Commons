using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// JSONメッセージクラス
    /// </summary>
    [DataContract]
    public class JsonMessage
    {
        /// <summary>
        /// レコード情報リスト
        /// </summary>
        [DataMember(Name = "RecordList")]
        public List<RecordInfo> RecordList;

        /// <summary>
        /// レコード情報クラス
        /// </summary>
        [DataContract]
        public class RecordInfo
        {
            /// <summary>
            /// レコードヘッダーリスト（任意。将来廃止予定）
            /// </summary>
            [DataMember(Name = "RecordHeader", Order = 0, EmitDefaultValue = false)]
            public List<string> RecordHeader;

            /// <summary>
            /// レコードデータリスト
            /// </summary>
            [DataMember(Name = "RecordData", Order = 1)]
            public List<object> RecordData;
        }

        /// <summary>
        /// JSON文字列から JSONメッセージへデシリアライズする。
        /// デシリアライズに失敗した場合、null を返す。
        /// </summary>
        /// <param name="message">JSON文字列</param>
        /// <returns>JSONメッセージ</returns>
        public static JsonMessage DeserializeJsonMessage(string message)
        {
            try
            {
                IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
                return serializer.Deserialize<JsonMessage>(message);
            }
            catch(Exception)
            {
                return null;
            }
            
        }

        /// <summary>
        /// JSONバイト配列から JSONメッセージへデシリアライズする。
        /// デシリアライズに失敗した場合、null を返す。
        /// </summary>
        /// <param name="message">JSONバイト配列</param>
        /// <returns>JSONメッセージ</returns>
        public static JsonMessage DeserializeJsonMessage(byte[] message)
        {
            try
            {
                IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
                return serializer.Deserialize<JsonMessage>(message);
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// JSONメッセージからJSON文字列へシリアライズする。
        /// </summary>
        /// <param name="message">JSONメッセージ</param>
        /// <returns>JSON文字列</returns>
        public static string SerializeJsonMessage(JsonMessage message)
        {
            IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
            return serializer.Serialize<JsonMessage>(message);
        }

        /// <summary>
        /// JSON メッセージから JSON バイト配列へシリアライズする。
        /// シリアライズに失敗した場合、null を返す。
        /// </summary>
        /// <param name="message">JSONメッセージ</param>
        /// <returns>JSONバイト配列</returns>
        public static byte[] SerializeJsonMessageByte(JsonMessage message)
        {
            if(message == null){
                return null;
            }

            try
            {  
                IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
                return serializer.SerializeBytes<JsonMessage>(message);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// JSON 文字列から RecordInfo クラスへデシリアライズする。
        /// デシリアライズに失敗した場合、null を返す。
        /// </summary>
        /// <param name="message">JSON文字列</param>
        /// <returns>RecordInfoクラス</returns>
        public static JsonMessage.RecordInfo DeserializeRecordInfo(string message)
        {
            try
            {
                IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
                return serializer.Deserialize<JsonMessage.RecordInfo>(message);
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// JSONバイト配列からRecordInfoクラスへデシリアライズする。
        /// デシリアライズに失敗した場合、null を返す。
        /// </summary>
        /// <param name="message">JSONバイト配列</param>
        /// <returns>RecordInfoクラス</returns>
        public static JsonMessage.RecordInfo DeserializeRecordInfo(byte[] message)
        {
            try
            {
                IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
                return serializer.Deserialize<JsonMessage.RecordInfo>(message);
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// RecordInfoクラスからJSON文字列へシリアライズする。
        /// </summary>
        /// <param name="message">RecordInfoクラス</param>
        /// <returns>JSON文字列</returns>
        public static string SerializeRecordInfo(JsonMessage.RecordInfo message)
        {
            IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
            return serializer.Serialize<JsonMessage.RecordInfo>(message);
        }

        /// <summary>
        /// RecordInfoクラスからJSONバイト配列へシリアライズする。
        /// シリアライズに失敗した場合、null を返す。
        /// </summary>
        /// <param name="message">RecordInfoクラス</param>
        /// <returns>JSONバイト配列</returns>
        public static byte[] SerializeRecordInfoByte(JsonMessage.RecordInfo message)
        {
            if(message == null){
                return null;
            }

            try
            {
                IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
                return serializer.SerializeBytes<JsonMessage.RecordInfo>(message);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// JsonMessage のバリデーションを行う。
        /// RecordList および各 RecordInfo.RecordData が null でないことを検証する。
        /// </summary>
        /// <param name="message">検証対象の JsonMessage</param>
        /// <param name="allowEmptyBody">true の場合、message が null でも有効とする（EOF用）</param>
        /// <returns>有効(true)／無効(false)</returns>
        public static bool Validate(JsonMessage message, bool allowEmptyBody = false)
        {
            if (message == null)
            {
                return allowEmptyBody;
            }

            if (message.RecordList == null)
            {
                return false;
            }

            return message.RecordList.All(record => record?.RecordData != null);
        }

    }
}
