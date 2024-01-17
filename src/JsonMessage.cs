using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// JSONメッセージクラス
    /// </summary>
    [DataContract]
    public class JsonMessage
    {
        [DataMember(Name = "RecordList")]
        public List<RecordInfo> RecordList;

        [DataContract]
        public class RecordInfo
        {
            [DataMember(Name = "RecordHeader", Order = 0)]
            public List<string> RecordHeader;

            [DataMember(Name = "RecordData", Order = 1)]
            public List<string> RecordData;
        }

        /// <summary>
        /// JSONメッセージのデシリアライズ
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
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
        /// JSONメッセージのデシリアライズ
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
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
        /// JSONメッセージのシリアライズ
        /// </summary>
        public static string SerializeJsonMessage(JsonMessage message)
        {
            IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
            return serializer.Serialize<JsonMessage>(message);
        }

        /// <summary>
        /// JSONメッセージのシリアライズ
        /// </summary>
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
        /// RecordInfoのデシリアライズ
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
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
        /// RecordInfoのデシリアライズ
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
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
        /// RecordInfoのシリアライズ
        /// </summary>
        public static string SerializeRecordInfo(JsonMessage.RecordInfo message)
        {
            IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
            return serializer.Serialize<JsonMessage.RecordInfo>(message);
        }

        /// <summary>
        /// RecordInfoのシリアライズ
        /// </summary>
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
