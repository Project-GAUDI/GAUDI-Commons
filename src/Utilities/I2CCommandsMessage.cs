using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// I2Cコマンドメッセージクラス
    /// </summary>
    [DataContract]
    public class I2CCommandsMessage
    {
        /// <summary>
        /// コマンドリスト
        /// </summary>
        [DataMember(Name = "CommandList")]
        public List<I2CCommand> CommandList;

        /// <summary>
        /// JSON文字列からI2CCommandsMessageへデシリアライズする。
        /// デシリアライズに失敗した場合、nullを返す。
        /// </summary>
        /// <param name="message">JSON文字列</param>
        /// <returns>I2CCommandsMessageインスタンス</returns>
        public static I2CCommandsMessage DeserializeJson(string message)
        {
            try
            {
                return DeserializeJson(Encoding.UTF8.GetBytes(message));

            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// JSONバイト配列からI2CCommandsMessageへデシリアライズする。
        /// デシリアライズに失敗した場合、nullを返す。
        /// </summary>
        /// <param name="message">JSONバイト配列</param>
        /// <returns>I2CCommandsMessageインスタンス</returns>
        public static I2CCommandsMessage DeserializeJson(byte[] message)
        {
            var serializer = new DataContractJsonSerializer(typeof(I2CCommandsMessage));
            try
            {
                using (var ms = new MemoryStream(message))
                {
                    return (I2CCommandsMessage)serializer.ReadObject(ms);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// I2CCommandsMessageをJSON文字列へシリアライズする。
        /// シリアライズに失敗した場合、nullを返す。
        /// </summary>
        /// <param name="message">I2CCommandsMessageインスタンス</param>
        /// <returns>JSON文字列</returns>
        public static string SerializeJson(I2CCommandsMessage message)
        {
            var byteAry = SerializeJsonBytes(message);
            if (byteAry != null)
            {
                return Encoding.UTF8.GetString(byteAry);
            }
            return null;
        }

        /// <summary>
        /// I2CCommandsMessageをJSONバイト配列へシリアライズする。
        /// シリアライズに失敗した場合、nullを返す。
        /// </summary>
        /// <param name="message">I2CCommandsMessageインスタンス</param>
        /// <returns>JSONバイト配列</returns>
        public static byte[] SerializeJsonBytes(I2CCommandsMessage message)
        {
            if (message == null)
            {
                return null;
            }

            var serializer = new DataContractJsonSerializer(typeof(I2CCommandsMessage));
            try
            {
                using (var ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, message);
                    return ms.ToArray();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// I2Cコマンドクラス
    /// </summary>
    [DataContract]
    public class I2CCommand
    {
        /// <summary>
        /// アクション
        /// </summary>
        [DataMember(Name = "Action")]
        public string Action { get; set; }

        /// <summary>
        /// アドレス
        /// </summary>
        [DataMember(Name = "Address")]
        public string Address { get; set; }

        /// <summary>
        /// コマンド
        /// </summary>
        [DataMember(Name = "Command")]
        public string Command { get; set; }

        /// <summary>
        /// データ
        /// </summary>
        [DataMember(Name = "Data")]
        public string Data { get; set; }

        /// <summary>
        /// フィルタ
        /// </summary>
        [DataMember(Name = "Filter")]
        public string Filter { get; set; }

        /// <summary>
        /// データ長
        /// </summary>
        [DataMember(Name = "Length")]
        public int Length { get; set; }

        /// <summary>
        /// インターバル
        /// </summary>
        [DataMember(Name = "Interval")]
        public int Interval { get; set; }
    }
}
