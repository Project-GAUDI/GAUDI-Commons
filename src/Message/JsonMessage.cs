using System;
using System.Collections.Generic;
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
            /// レコードヘッダーリスト
            /// </summary>
            [DataMember(Name = "RecordHeader", Order = 0)]
            public List<string> RecordHeader;

            /// <summary>
            /// レコードデータリスト
            /// </summary>
            [DataMember(Name = "RecordData", Order = 1)]
            public List<string> RecordData;
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

    }
}
