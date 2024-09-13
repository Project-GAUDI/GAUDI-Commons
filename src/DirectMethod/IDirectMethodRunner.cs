using System;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Direct Method Interface class
    /// </summary>
    public interface IDirectMethodRunner : IDisposable
    {
        /// <summary>
        /// リクエスト(JSON形式)の解析
        /// </summary>
        /// <param name="requestJSON">リクエスト文字列(JSON形式)</param>
        /// <returns>処理結果：true=成功、false=失敗</returns>
        public Task<bool> ParseRequest(string requestJSON);

        /// <summary>
        ///　ダイレクトメソッドの実行 
        /// </summary>
        /// <returns>処理結果：true=成功、false=失敗</returns>
        public Task<bool> Run();

        /// <summary>
        ///　ダイレクトメソッドの実行結果の取得
        /// </summary>
        /// <returns>実行結果</returns>        
        public DirectMethodResponse GetResult();
    }
}