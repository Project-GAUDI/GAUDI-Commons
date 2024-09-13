using System;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Direct Method Calling class
    /// </summary>
    internal class DirectMethod_SetLogLevel : IDirectMethodRunner
    {
        protected class RequestData
        {
            public int EnableSec { get; set; } = ILogger.SecondsDefinition_OUTOFRANGE;//未設定
            public string LogLevel = "";
            
            public bool Validate()
            {
                bool result = false;

                if ("" == LogLevel)
                {
                    if (ILogger.SecondsDefinition_CANCEL == EnableSec)
                    {
                        // キャンセル時は省略可
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if (ILogger.SecondsDefinition_UNLIMITED <= EnableSec)
                {
                    result = true;
                }

                return result;
            }
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
            bool retStatus = false;

            try
            {
                IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();

                requestData = serializer.Deserialize<RequestData>(requestJSON);

                if (null != requestData && true == requestData.Validate())
                {
                    retStatus = true;
                }
                else
                {
                    retStatus = false;
                    responseData.Status = -1;
                    responseData.Results.Add("Error", $"Bad request.({requestJSON})");
                }
            }
            catch (Exception ex)
            {
                retStatus = false;
                responseData.Status = -1;
                responseData.Results.Add("Error", $"Parsing request failed.({ex})");
            }

            await Task.CompletedTask;

            return retStatus;
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
                ILogger logger = LoggerFactory.GetLogger(typeof(DirectMethod_SetLogLevel));

                logger.SetMandatoryLogLevel(requestData.LogLevel, requestData.EnableSec);

                retStatus = true;
                responseData.Status = 0;
                responseData.Results.Add("CurrentLogLevel", $"{logger.OutputLogLevel.ToString().ToLower()}");
            }
            catch (Exception ex)
            {
                retStatus = false;
                responseData.Status = -1;
                responseData.Results.Add("Error", $"SetLogLevel failed.({ex})");
            }

            await Task.CompletedTask;

            return retStatus;
        }

        /// <summary>
        ///　ダイレクトメソッドの実行結果の取得
        /// </summary>
        /// <returns>実行結果</returns>        
        public DirectMethodResponse GetResult()
        {
            return responseData;
        }
    }
}
