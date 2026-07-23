using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

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

        // SysRuntimeSerialization は設定の大部分をサポートしない。
        // ただし Formatting=Indented はシリアライズ時にサポートする。
        // デフォルト値と異なる設定が渡された場合は警告を出力し、無視して処理を続行する。

        private static readonly JsonSerializerSettings _defaultSettings = new JsonSerializerSettings();

        /// <summary>
        /// 指定した設定でシリアライズ（Formatting をサポート、その他は警告して無視）
        /// </summary>
        public string Serialize<TargetType>(TargetType target, JsonSerializerSettings settings)
        {
            var byteAry = this.SerializeBytes(target, settings);
            return byteAry != null ? Encoding.UTF8.GetString(byteAry) : null;
        }

        /// <summary>
        /// 指定した設定でバイト列シリアライズ（Formatting をサポート、その他は警告して無視）
        /// </summary>
        public Byte[] SerializeBytes<TargetType>(TargetType target, JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                return this.SerializeBytes(target);
            }

            WarnUnsupportedSettings(settings);

            // Formatting=Indented の場合は JsonWriter 経由で整形出力
            bool useIndent = settings.Formatting == JsonFormatting.Indented;

            Byte[] retSerialized = null;

            if (target != null)
            {
                var serializer = new DataContractJsonSerializer(typeof(TargetType));
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        if (useIndent)
                        {
                            // インデント付き出力
                            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.UTF8, ownsStream: false, indent: true))
                            {
                                serializer.WriteObject(writer, target);
                                writer.Flush();
                            }
                        }
                        else
                        {
                            // コンパクト出力（既存動作）
                            serializer.WriteObject(ms, target);
                        }
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
        /// 指定した設定で文字列からデシリアライズ（非対応設定は警告して無視）
        /// </summary>
        public TargetType Deserialize<TargetType>(string jsonString, JsonSerializerSettings settings)
        {
            WarnUnsupportedSettings(settings);
            return this.Deserialize<TargetType>(jsonString);
        }

        /// <summary>
        /// 指定した設定でバイト列からデシリアライズ（非対応設定は警告して無視）
        /// </summary>
        public TargetType Deserialize<TargetType>(Byte[] jsonBytes, JsonSerializerSettings settings)
        {
            WarnUnsupportedSettings(settings);
            return this.Deserialize<TargetType>(jsonBytes);
        }

        /// <summary>
        /// デフォルト値と異なる設定項目を列挙して警告を出力する。
        /// すべてデフォルト値の場合は警告しない。
        /// Formatting はサポートされているため警告対象外。
        /// </summary>
        private static void WarnUnsupportedSettings(JsonSerializerSettings settings)
        {
            if (settings == null) return;

            var unsupported = new List<string>(8);

            if (settings.NullValueHandling != _defaultSettings.NullValueHandling)
                unsupported.Add(string.Format("{0}={1}", nameof(settings.NullValueHandling), settings.NullValueHandling));
            if (settings.DefaultValueHandling != _defaultSettings.DefaultValueHandling)
                unsupported.Add(string.Format("{0}={1}", nameof(settings.DefaultValueHandling), settings.DefaultValueHandling));
            // Formatting はサポートされているため警告しない
            if (settings.ReferenceLoopHandling != _defaultSettings.ReferenceLoopHandling)
                unsupported.Add(string.Format("{0}={1}", nameof(settings.ReferenceLoopHandling), settings.ReferenceLoopHandling));
            if (settings.DateParseHandling != _defaultSettings.DateParseHandling)
                unsupported.Add(string.Format("{0}={1}", nameof(settings.DateParseHandling), settings.DateParseHandling));
            if (settings.DateFormatHandling != _defaultSettings.DateFormatHandling)
                unsupported.Add(string.Format("{0}={1}", nameof(settings.DateFormatHandling), settings.DateFormatHandling));
            if (settings.StringEscapeHandling != _defaultSettings.StringEscapeHandling)
                unsupported.Add(string.Format("{0}={1}", nameof(settings.StringEscapeHandling), settings.StringEscapeHandling));
            if (settings.ExcludeEmptyCollections != _defaultSettings.ExcludeEmptyCollections)
                unsupported.Add(string.Format("{0}={1}", nameof(settings.ExcludeEmptyCollections), settings.ExcludeEmptyCollections));

            if (unsupported.Count == 0) return;

            Trace.TraceWarning(string.Format(
                "[{0}] The current serializer is {0}. The following settings are not supported and will be ignored: {1}",
                nameof(SysRuntimeJsonSerializer),
                string.Join(", ", unsupported)));
        }

    }
}
