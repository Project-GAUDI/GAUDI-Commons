using System;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Newtonsoft.Json.Linq;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// トランスポートプロトコル種別
    /// </summary>
    public enum TransportProtocol
    {
        /// <summary>
        /// AMQP プロトコル
        /// </summary>
        Amqp,
        /// <summary>
        /// MQTT プロトコル
        /// </summary>
        Mqtt,
        // Http1
    }

    /// <summary>
    /// ユーティリティクラス
    /// </summary>
    public static class Util
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(Util));

        /// <summary>
        /// 現在日時("yyyyMMddHHmmssfff")とGUIDを組み合わせて、ユニークなメッセージ ID を生成する。
        /// </summary>
        /// <returns>生成したメッセージID</returns>
        public static string GetMessageId()
        {
            return $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}{Guid.NewGuid().ToString("N")}";
        }

        /// <summary>
        /// 指定したトランスポートプロトコル種別で生成したトランスポート設定を取得する。
        /// 想定外のトランスポートプロトコル種別が指定された場合、nullを返す。
        /// </summary>
        /// <param name="transportProtocol">トランスポートプロトコル種別</param>
        /// <returns>トランスポート設定配列</returns>
        public static ITransportSettings[] GetTransportSettings(this TransportProtocol transportProtocol)
        {
            ITransportSettings[] settings;
            switch(transportProtocol)
            {
                case TransportProtocol.Amqp:
                    settings = new ITransportSettings[]{ new AmqpTransportSettings(TransportType.Amqp_Tcp_Only) };
                    break;
                case TransportProtocol.Mqtt:
                    settings = new ITransportSettings[]{ new MqttTransportSettings(TransportType.Mqtt_Tcp_Only) };
                    break;
                // case TransportProtocol.Http1:
                //     settings = new ITransportSettings[]{ new Http1TransportSettings() };
                //     break;
                default:
                    settings = null;
                    break;
            }

            return settings;
        }

        /// <summary>
        /// JObjectより必須値を取得し、指定した型で取得する。
        /// 指定したキー名が存在しない場合、Exceptionをスローする。
        /// 【非推奨】GetDesiredProperty、GetEnvironmentVariableを使用してください。
        /// </summary>
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="jobj">JObjectインスタンス</param>
        /// <param name="key">キー名</param>
        /// <returns>取得した値</returns>
        // [Obsolete("このメソッドは非推奨です。代わりに新しいメソッド：\"GetDesiredProperty\" or \"GetEnvironmentVariable\" を使用してください。", false)]
        public static T GetRequiredValue<T>(JObject jobj, string key)
        {
            T ret;

            if (jobj.TryGetValue(key, out JToken value))
            {
                ret = value.Value<T>();
            }
            else
            {
                throw new Exception($"Property {key} dose not exist.");
            }

            return ret;
        }

        /// <summary>
        /// 16進数文字列からバイト値へ変換する。
        /// 指定した16進数文字列が2文字でない場合、Exceptionをスローする。
        /// </summary>
        /// <param name="val">16進数文字列（2文字）</param>
        /// <returns>バイト値</returns>
        public static byte HexStringToByte(string val)
        {
            if (val.Length != 2)
            {
                throw new Exception($"Convert to byte failed. val:{val}");
            }

            return Convert.ToByte(val, 16);
        }

        /// <summary>
        /// 16進数文字列からバイト配列へ変換する。
        /// エラー処理は特にしていない。
        /// </summary>
        /// <param name="val">16進数文字列（"-"区切り）</param>
        /// <returns>バイト配列</returns>
        public static byte[] HexStringToBytes(string val)
        {
            var hs = val.Split('-');
            var ret = new byte[hs.Length];

            for (int i = 0; i < hs.Length; i++)
            {
                ret[i] = HexStringToByte(hs[i]);
            }

            return ret;
        }

        /// <summary>
        /// DesiredProperty より、指定した型でキーの値を取得する。
        /// 必須項目でキーが存在しない場合、Exception をスローする。
        /// 必須でない項目でキーが存在しない場合、デフォルト値を返す。
        /// 型・制約条件違反の場合、Exception をスローする。
        /// </summary>
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="properties">DesiredProperty</param>
        /// <param name="key">キー名</param>
        /// <param name="isRequired">必須項目フラグ</param>
        /// <param name="defaultValue">デフォルト値（デフォルト：Default）</param>
        /// <param name="condition">制約条件（デフォルト：null）</param>
        /// <returns>取得した値</returns>
        public static T GetDesiredProperty<T>(JObject properties, string key, bool isRequired, T defaultValue = default, Func<T, bool> condition = null)
        {
            if (!properties.ContainsKey(key))
            {
                if (isRequired)
                {
                    Logger.WriteLog(ILogger.LogLevel.ERROR, $"Required property '{key}' is not specified.");
                    throw new ArgumentException($"Required property '{key}' is not specified.");
                }
                else
                {
                    Logger.WriteLog(ILogger.LogLevel.INFO, $"Optional property '{key}' is not specified. Using default value: {defaultValue}.");
                    return defaultValue;
                }
            }

            try
            {
                var token = properties[key];
                if (isRequired && (token.Type == JTokenType.Null || (token.Type == JTokenType.String && string.IsNullOrWhiteSpace(token.ToString()))))
                {
                    throw new ArgumentException($"Required property '{key}' cannot be null or empty.");
                }
                
                var value = token.ToObject<T>();

                if (typeof(T).IsEnum)
                {
                    if (!Enum.IsDefined(typeof(T), value.ToString()))
                    {
                        throw new ArgumentException($"Property '{key}' must be a valid value of the enum '{typeof(T).Name}'.");
                    }
                }

                if (condition != null && !condition(value))
                {
                    throw new ArgumentException($"Property '{key}' did not meet the specified condition.");
                }

                Logger.WriteLog(ILogger.LogLevel.INFO, $"Property '{key}' retrieved successfully: {value}");
                return value;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error retrieving property '{key}': {ex.Message}";
                Logger.WriteLog(ILogger.LogLevel.ERROR, errorMessage);
                throw new ArgumentException(errorMessage);
            }
        }

        /// <summary>
        /// 環境変数より、指定した型で値を取得する。
        /// 必須項目でキーが存在しない場合、Exception をスローする。
        /// 必須でない項目でキーが存在しない場合、デフォルト値を返す。
        /// 型・制約条件違反の場合、Exception をスローする。
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="name">環境変数名</param>
        /// <param name="isRequired">必須項目フラグ</param>
        /// <param name="defaultValue">デフォルト値（デフォルト：Default）</param>
        /// <param name="condition">制約条件（デフォルト：null）</param>
        /// <returns>取得した値</returns>
        /// </summary>
        public static T GetEnvironmentVariable<T>(string name, bool isRequired, T defaultValue = default, Func<T, bool> condition = null)
        {            
            var value = Environment.GetEnvironmentVariable(name);

            if (value == null)
            {
                if (isRequired)
                {
                    Logger.WriteLog(ILogger.LogLevel.ERROR, $"Required environment variable '{name}' is not specified.");
                    throw new ArgumentException($"Required environment variable '{name}' is not specified.");
                }
                else
                {
                    Logger.WriteLog(ILogger.LogLevel.INFO, $"Optional environment variable '{name}' is not specified. Using default value: {defaultValue}.");
                    return defaultValue;
                }
            }

            try
            {
                T result;

                if (typeof(T).IsEnum)
                {
                    if (Enum.TryParse(typeof(T), value, true, out object enumResult) && Enum.IsDefined(typeof(T), enumResult))
                    {
                        result = (T)enumResult;
                    }
                    else
                    {
                        throw new ArgumentException($"Environment variable '{name}' must be a valid value of the enum '{typeof(T).Name}'. Value provided: '{value}'");
                    }
                }
                else
                {
                    result = (T)Convert.ChangeType(value, typeof(T));
                }

                if (condition != null && !condition(result))
                {
                    throw new ArgumentException($"Environment variable '{name}' did not meet the specified condition. Value provided: '{result}'");
                }

                Logger.WriteLog(ILogger.LogLevel.INFO, $"Environment variable '{name}' retrieved successfully: {result}");
                return result;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error retrieving environment variable '{name}': {ex.Message}";
                Logger.WriteLog(ILogger.LogLevel.ERROR, errorMessage);
                throw new ArgumentException(errorMessage);
            }
        }
    }
}