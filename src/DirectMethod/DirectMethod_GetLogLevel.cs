using System;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Direct Method Calling class
    /// </summary>
    internal class DirectMethod_GetLogLevel : IDirectMethodRunner
    {
        protected class RequestData
        {

        }

        protected RequestData requestData { get; set; }
        protected DirectMethodResponse responseData { get; set; } = new DirectMethodResponse(-1);


        public void Dispose()
        {

        }

        /// <summary>
        /// リクエスト(JSON形式)の解析
        /// </summary>
        /// <param name="requestJSON">リクエスト文字列(JSON形式)</param>
        /// <returns>処理結果：true=成功、false=失敗</returns>
        public async Task<bool> ParseRequest(string requestJSON)
        {
            await Task.CompletedTask;
            return true;
        }

        /// <summary>
        ///　ダイレクトメソッドの実行 
        /// </summary>
        /// <returns></returns>        
        public async Task<bool> Run()
        {
            bool retStatus = false;

            try
            {
                ILogger logger = LoggerFactory.GetLogger(typeof(DirectMethod_GetLogLevel));

                retStatus = true;
                responseData.Status = 0;
                responseData.Results.Add("CurrentLogLevel", $"{logger.OutputLogLevel.ToString().ToLower()}");
            }
            catch (Exception ex)
            {
                retStatus = false;
                responseData.Status = -1;
                responseData.Results.Add("Error", $"GetLogLevel failed.({ex})");
            }

            await Task.CompletedTask;

            return retStatus;
        }

        /// <summary>
        ///ダイレクトメソッドの実行結果の取得
        /// </summary>
        /// <returns>実行結果</returns>        
        public DirectMethodResponse GetResult()
        {
            return responseData;
        }
    }
}