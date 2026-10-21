namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// IBufferBackupStore インスタンスを取得するためのファクトリークラス。
    /// </summary>
    public class BufferBackupStoreFactory
    {
        /// <summary>
        /// 退避先ディレクトリ
        /// </summary>
        protected const string BUFFERBACKUP_DIR = "/app/backupdata";

        /// <summary>
        /// IBufferBackupStore クラスのインスタンスを取得する。
        /// 退避先ディレクトリは "/app/backupdata" を使用する。
        /// </summary>
        /// <param name="preserveDerivedTypes">派生型を保持して復元するか（デフォルト：false）</param>
        /// <returns>IBufferBackupStore インスタンス</returns>
        public static IBufferBackupStore GetStore(bool preserveDerivedTypes = false)
        {
            return GetStore(BUFFERBACKUP_DIR, preserveDerivedTypes);
        }

        /// <summary>
        /// 退避先ディレクトリを指定して IBufferBackupStore クラスのインスタンスを取得する。
        /// </summary>
        /// <param name="baseDirectory">退避先ディレクトリ</param>
        /// <param name="preserveDerivedTypes">派生型を保持して復元するか（デフォルト：false）</param>
        /// <returns>IBufferBackupStore インスタンス</returns>
        public static IBufferBackupStore GetStore(string baseDirectory, bool preserveDerivedTypes = false)
        {
            return new BufferBackupStore(baseDirectory, preserveDerivedTypes);
        }
    }
}
