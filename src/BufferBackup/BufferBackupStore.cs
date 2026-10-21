using System;
using System.Collections.Generic;
using System.IO;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// ファイルシステムを使用した <see cref="IBufferBackupStore"/> の実装。
    /// </summary>
    internal sealed class BufferBackupStore : IBufferBackupStore
    {
        private static ILogger MyLogger { get; } = LoggerFactory.GetLogger(typeof(BufferBackupStore));

        private const string ModuleIdEnvName = "IOTEDGE_MODULEID";

        private const string DefaultModuleId = "default";

        private readonly string baseDirectory;

        private readonly JsonSerializerSettings serializerSettings;

        // 型情報の保持には TypeNameHandling が必要なため、Newtonsoft 実装を固定で使用する
        private readonly IJsonSerializer serializer =
            JsonSerializerFactory.GetJsonSerializer(SerializerType.NewtonsoftJson);

        internal BufferBackupStore(string baseDirectory, bool preserveDerivedTypes)
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                throw new ArgumentException("baseDirectory is null or empty.", nameof(baseDirectory));
            }

            this.baseDirectory = baseDirectory;
            this.serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = preserveDerivedTypes
                    ? JsonTypeNameHandling.Auto
                    : JsonTypeNameHandling.None,
            };
        }

        /// <summary>
        /// 退避ファイルのパスを取得する。
        /// </summary>
        public string GetBackupPath(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                ValidateKey(key, nameof(key));
            }

            string moduleId = GetModuleId();

            string fileName = string.IsNullOrWhiteSpace(key)
                ? $"{moduleId}_backup.json"
                : $"{moduleId}_{key}_backup.json";

            return Path.Combine(baseDirectory, fileName);
        }

        /// <summary>
        /// データをディスクへ退避する。
        /// </summary>
        public bool Save<TItem>(string key, IEnumerable<TItem> items)
        {
            string path = GetBackupPath(key);

            if (items == null)
            {
                MyLogger.Warn($"Backup skipped because items is null. Key={key}");
                return false;
            }

            var buffer = new List<TItem>(items);
            string tempPath = path + ".tmp";

            try
            {
                Directory.CreateDirectory(baseDirectory);

                string json = serializer.Serialize(buffer, serializerSettings);
                File.WriteAllText(tempPath, json);
                // 書き込み途中のファイルを復元対象にしないため、完了後に差し替える
                File.Move(tempPath, path, true);

                MyLogger.Info($"Buffer saved to disk. Key={key}, Count={buffer.Count}, Path={path}");
                return true;
            }
            catch (Exception ex)
            {
                MyLogger.Error($"Failed to save buffer to disk. Key={key}, Path={path}, {ex.Message}", isUpload: true);
                TryDeleteFile(tempPath);
                return false;
            }
        }

        /// <summary>
        /// 退避データをディスクから復元する。
        /// </summary>
        public List<TItem> Restore<TItem>(string key, bool deleteAfterRestore = true)
        {
            string path = GetBackupPath(key);

            try
            {
                if (!File.Exists(path))
                {
                    return null;
                }

                string json = File.ReadAllText(path);
                var restored = serializer.Deserialize<List<TItem>>(json, serializerSettings);

                if (deleteAfterRestore)
                {
                    File.Delete(path);
                }

                if (restored == null)
                {
                    MyLogger.Error($"Failed to deserialize backup file. Key={key}, Path={path}", isUpload: true);
                    return null;
                }

                MyLogger.Info($"Buffer restored from disk. Key={key}, Count={restored.Count}");
                return restored;
            }
            catch (Exception ex)
            {
                MyLogger.Error($"Failed to restore buffer from disk. Key={key}, Path={path}, {ex.Message}", isUpload: true);
                return null;
            }
        }

        /// <summary>
        /// 退避データが存在するか確認する。
        /// </summary>
        public bool Exists(string key)
        {
            string path = GetBackupPath(key);

            try
            {
                return File.Exists(path);
            }
            catch (Exception ex)
            {
                MyLogger.Error($"Failed to check backup file. Key={key}, Path={path}, {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 退避データを削除する。
        /// </summary>
        public bool Delete(string key)
        {
            string path = GetBackupPath(key);

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
            catch (Exception ex)
            {
                MyLogger.Error($"Failed to delete backup file. Key={key}, Path={path}, {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 現在のモジュールに保存されている退避データのキー一覧を取得する。
        /// </summary>
        public List<string> GetBackupKeys()
        {
            var keys = new List<string>();
            string moduleId = GetModuleId();
            string prefix = $"{moduleId}_";
            const string suffix = "_backup.json";

            try
            {
                if (!Directory.Exists(baseDirectory))
                {
                    return keys;
                }

                // キーなしの退避ファイルは "{moduleId}_backup.json" となり、prefix+suffix のパターンにマッチしないため個別に判定する
                if (File.Exists(Path.Combine(baseDirectory, $"{moduleId}_backup.json")))
                {
                    keys.Add(string.Empty);
                }

                foreach (string path in Directory.GetFiles(baseDirectory, $"{prefix}*{suffix}"))
                {
                    string fileName = Path.GetFileName(path);
                    int keyLength = fileName.Length - prefix.Length - suffix.Length;
                    if (0 < keyLength)
                    {
                        keys.Add(fileName.Substring(prefix.Length, keyLength));
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.Error($"Failed to get backup keys. Directory={baseDirectory}, {ex.Message}", isUpload: true);
            }

            return keys;
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception)
            {
                // 一時ファイルの後始末に失敗しても退避処理の結果には影響させない
            }
        }

        private static string GetModuleId()
        {
            string moduleId = Environment.GetEnvironmentVariable(ModuleIdEnvName);
            if (string.IsNullOrWhiteSpace(moduleId))
            {
                moduleId = DefaultModuleId;
            }
            ValidateKey(moduleId, ModuleIdEnvName);
            return moduleId;
        }

        /// <summary>
        /// ファイル名の一部として安全に使用できるか検証する。
        /// </summary>
        private static void ValidateKey(string key, string paramName)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"{paramName} is null or empty.", paramName);
            }

            // 区切り文字はプラットフォームに依存せず拒否する（Linux では '\' が有効なファイル名文字のため）
            if (key.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
                || key.Contains("..")
                || key.IndexOf('/') >= 0
                || key.IndexOf('\\') >= 0)
            {
                throw new ArgumentException(
                    $"{paramName} contains invalid characters for a file name. value={key}", paramName);
            }
        }
    }
}
