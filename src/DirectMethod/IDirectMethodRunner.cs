using System;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Direct Method Runner class
    /// </summary>
    public interface IDirectMethodRunner : IDisposable
    {
        /// <summary>
        /// リクエスト文字列（JSON 形式）を解析して、処理結果を返す。
        /// </summary>
        /// <param name="requestJSON">リクエスト文字列（JSON形式）</param>
        /// <returns>処理成功(true)、処理失敗(false)</returns>
        public Task<bool> ParseRequest(string requestJSON);

        /// <summary>
        ///　ダイレクトメソッドを実行する。 
        /// </summary>
        /// <returns>処理成功(true)、処理失敗(false)</returns>
        public Task<bool> Run();

        /// <summary>
        /// ダイレクトメソッドの実行結果を取得する。
        /// </summary>
        /// <returns>実行結果</returns>        
        public DirectMethodResponse GetResult();
    }
}