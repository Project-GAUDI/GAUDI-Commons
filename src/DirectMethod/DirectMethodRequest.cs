
namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Direct Method Request class
    /// </summary>
    public class DirectMethodRequest
    {
        /// <summary>
        /// メソッド名
        /// </summary>
        public string MethodName { get; set; } = "";
        /// <summary>
        /// リクエストJSON文字列
        /// </summary>
        public string RequestJson { get; set; } = "";

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public DirectMethodRequest()
        {
        }

        /// <summary>
        /// パラメータ指定コンストラクタ
        /// </summary>
        /// <param name="methodName">メソッド名</param>
        /// <param name="requestJson">リクエストJSON文字列</param>
        public DirectMethodRequest(string methodName, string requestJson)
        {
            MethodName = methodName;
            RequestJson = requestJson;
        }
    }
}