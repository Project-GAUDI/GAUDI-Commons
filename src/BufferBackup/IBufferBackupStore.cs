using System.Collections.Generic;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// メモリ上の未送信データをディスクへ退避・復元するストアのインターフェース。
    /// </summary>
    /// <remarks>
    /// モジュール終了時に <see cref="Save{TItem}"/> で退避し、起動時に <see cref="Restore{TItem}"/> で復元する用途を想定する。
    /// 入出力の失敗は例外をスローせず、ログ出力のうえ戻り値で通知する（ベストエフォート）。
    /// </remarks>
    public interface IBufferBackupStore
    {
        /// <summary>
        /// 指定したキーでデータをディスクへ退避する。
        /// 既に同じキーの退避データが存在する場合は上書きする。
        /// </summary>
        /// <param name="key">退避データを識別するキー</param>
        /// <param name="items">退避対象データ</param>
        /// <typeparam name="TItem">退避対象データの要素タイプ</typeparam>
        /// <returns>退避に成功した(true)／失敗した(false)</returns>
        /// <exception cref="System.ArgumentException">キーが空白以外で、ファイル名として使用できない場合</exception>
        bool Save<TItem>(string key, IEnumerable<TItem> items);

        /// <summary>
        /// 指定したキーの退避データをディスクから復元する。
        /// </summary>
        /// <param name="key">退避データを識別するキー</param>
        /// <param name="deleteAfterRestore">復元後に退避ファイルを削除するか（デフォルト：true）</param>
        /// <typeparam name="TItem">復元対象データの要素タイプ</typeparam>
        /// <returns>復元したデータ。退避データが存在しない場合、または復元に失敗した場合は null</returns>
        /// <exception cref="System.ArgumentException">キーが空白以外で、ファイル名として使用できない場合</exception>
        List<TItem> Restore<TItem>(string key, bool deleteAfterRestore = true);

        /// <summary>
        /// 指定したキーの退避データが存在するか確認する。
        /// </summary>
        /// <param name="key">退避データを識別するキー</param>
        /// <returns>存在する(true)／存在しない(false)</returns>
        /// <exception cref="System.ArgumentException">キーが空白以外で、ファイル名として使用できない場合</exception>
        bool Exists(string key);

        /// <summary>
        /// 指定したキーの退避データを削除する。
        /// </summary>
        /// <param name="key">退避データを識別するキー</param>
        /// <returns>削除した、または元から存在しなかった(true)／削除に失敗した(false)</returns>
        /// <exception cref="System.ArgumentException">キーが空白以外で、ファイル名として使用できない場合</exception>
        bool Delete(string key);

        /// <summary>
        /// 現在のモジュールに保存されている退避データのキー一覧を取得する。
        /// </summary>
        /// <returns>退避データを識別するキーの一覧。退避データが存在しない場合は空のリスト</returns>
        List<string> GetBackupKeys();

        /// <summary>
        /// 指定したキーに対応する退避ファイルのパスを取得する。
        /// </summary>
        /// <param name="key">退避データを識別するキー</param>
        /// <returns>退避ファイルのパス</returns>
        /// <exception cref="System.ArgumentException">キーが空白以外で、ファイル名として使用できない場合</exception>
        string GetBackupPath(string key);
    }
}
