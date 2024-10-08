using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;

namespace TICO.GAUDI.Commons
{
    internal class MessageEventData
    {
        public string InputName { get; set; } = "";
        public object UserContext { get; set; } = null;
        public MessageEventHandler MsgHandler = null;

        public MessageEventData(string inputName, object userContext, MessageEventHandler msgHandler)
        {
            this.InputName = inputName;
            this.UserContext = userContext;
            this.MsgHandler = msgHandler;
        }
    }

    internal class MethodEventData
    {
        public string MethodName { get; set; } = "";
        public object UserContext { get; set; } = null;
        public DirectMethodHandler MethodHandler { get; set; } = null;

        public MethodEventData(string methodName, object userContext, DirectMethodHandler methodHandler)
        {
            this.MethodName = methodName;
            this.UserContext = userContext;
            this.MethodHandler = methodHandler;
        }
    }

    /// <summary>
    /// 内部使用用途の拡張メソッド実装クラス
    /// </summary>
    internal static class MyExtensions
    {

        /// <summary>
        /// MessageResponse -> bool結果置換
        /// </summary>
        /// <param name="thisMethodRequest"></param>
        /// <returns>DirectMethodRequestインスタンス</returns>
        public static MessageResponse ToMessageResponse(this bool thisResult)
        {
            MessageResponse retResp = MessageResponse.None;

            switch (thisResult)
            {
                case true:
                    retResp = MessageResponse.Completed;
                    break;
                case false:
                    retResp = MessageResponse.None;
                    break;
                default:
                    retResp = MessageResponse.Abandoned;
                    break;
            }
            return retResp;
        }

        /// <summary>
        /// MethodRequest -> DirectMethodRequest 変換
        /// </summary>
        /// <param name="thisMethodRequest"></param>
        /// <returns>DirectMethodRequestインスタンス</returns>
        public static DirectMethodRequest ToDirectMethodRequest(this MethodRequest thisMethodRequest)
        {
            return new DirectMethodRequest(thisMethodRequest.Name, thisMethodRequest.DataAsJson);
        }

        /// <summary>
        /// string -> Byte[] 変換
        /// </summary>
        /// <param name="thisString"></param>
        /// <returns></returns>
        public static Byte[] ToBytes(this string thisString)
        {
            Byte[] retBytes = null;

            if (null != thisString)
            {
                retBytes = Encoding.UTF8.GetBytes(thisString);
            }

            return retBytes;
        }

        /// <summary>
        /// DirectMethodResponse -> MethodResponse 変換
        /// </summary>
        /// <param name="thisMethodResponse"></param>
        /// <returns>MethodResponseインスタンス</returns>
        public static MethodResponse ToMethodResponse(this DirectMethodResponse thisMethodResponse)
        {
            IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
            string resultJson = serializer.Serialize<Dictionary<string, object>>(thisMethodResponse.Results);
            return new MethodResponse(resultJson.ToBytes(), thisMethodResponse.Status);
        }

        public static bool IsReconnectableReason(this IotConnectionStatusChangeReason thisReason)
        {
            bool retIsReconnectable = false;

            switch (thisReason)
            {
                case IotConnectionStatusChangeReason.Bad_Credential:       // 認証失敗
                    retIsReconnectable = false;     // 認証が通る状態ができるまで再接続は多分無理なので
                    break;
                case IotConnectionStatusChangeReason.Client_Close:         // 正常クローズされた 
                    retIsReconnectable = false;     // 意図的に閉じられたので再接続不要
                    break;
                case IotConnectionStatusChangeReason.Communication_Error:  // 通信失敗 
                    retIsReconnectable = true;      // 再接続必要
                    break;
                case IotConnectionStatusChangeReason.Connection_Ok:        // （対象外）接続成功
                    retIsReconnectable = false;     // 対象外
                    break;
                case IotConnectionStatusChangeReason.Device_Disabled:      // デバイス/モジュールの削除・無効化 
                    retIsReconnectable = false;     // 無効化されたので再接続不要
                    break;
                case IotConnectionStatusChangeReason.Expired_SAS_Token:    // （未使用）SASトークン期限切れ 
                    retIsReconnectable = false;     // 対象外
                    break;
                case IotConnectionStatusChangeReason.No_Network:           // （未使用）ネットワーク損失
                    retIsReconnectable = false;     // 対象外
                    break;
                case IotConnectionStatusChangeReason.Retry_Expired:        // 再接続期限切れ
                    retIsReconnectable = true;      // 再接続必要
                    break;
                default:
                    retIsReconnectable = false;
                    break;

            }

            return retIsReconnectable;
        }
    }

    /// <summary>
    /// Application Engine Implementation class
    /// </summary>
    internal partial class ApplicationEngine : IApplicationEngineInternal
    {
        /// <summary>
        /// ログ出力用クラス。					
        /// 初期化時に通信クラスを設定する。					
        /// </summary>
        private static ILogger MyLogger { get; } = LoggerFactory.GetLogger(typeof(ApplicationEngine));

        /// <summary>
        /// アプリケーションメインインスタンス
        /// </summary>
        protected IApplicationMain applicationMain = null;

        /// <summary>
        /// メッセージ受信時のイベントデータ					
        /// </summary>
        protected Dictionary<string, MessageEventData> messageInputEventData = new Dictionary<string, MessageEventData>();


        /// <summary>
        /// ダイレクトメソッド受信時のイベントデータ
        /// </summary>
        protected Dictionary<string, MethodEventData> methodRequestEventData = new Dictionary<string, MethodEventData>();

        /// <summary>
        /// モジュールクライアント
        /// </summary>
        protected IModuleClient MyModuleClient = null;

        /// <summary>
        /// ステート管理クラス
        /// </summary>
        protected StateController stateController = new StateController();

        /// <summary>
        /// エンジンキャンセル用トークン
        /// </summary>
        protected CancellationTokenSource engineCanceller = null;

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        protected static Task GetCancelWaitTask(CancellationToken cancellationToken)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: GetCancelWaitTask");

            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: GetCancelWaitTask");
            return tcs.Task;
        }

        /// <summary>
        /// プロパティ更新処理
        /// </summary>
        private async Task<bool> DesiredPropertiesUpdate(TwinCollection desiredProperties, object userContext)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: DesiredPropertiesUpdate");
            MyLogger.WriteLog(ILogger.LogLevel.INFO, "Updating desired properties.");
            bool retResult = false;

            try
            {
                string jsonDesiredProperties = desiredProperties.ToJson();
                IJsonSerializer jserializer = JsonSerializerFactory.GetJsonSerializer();
                JObject jsonRootObject = jserializer.Deserialize<JObject>(jsonDesiredProperties);
                retResult = await applicationMain.OnDesiredPropertiesReceivedAsync(jsonRootObject);
            }
            catch (Exception ex)
            {
                retResult = true;
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"OnDesiredPropertiesUpdate failed. {ex}", true);
            }

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: DesiredPropertiesUpdate");
            return retResult;
        }

        private async Task<DirectMethodResponse> DirectMethodCalled(string methodName, DirectMethodRequest methodRequest, object userContext)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: DirectMethodCalled");
            MyLogger.WriteLog(ILogger.LogLevel.DEBUG, $"Request MethodName={methodRequest.MethodName}, RequestJson={methodRequest.RequestJson}");

            DirectMethodResponse retResp = await DirectMethodCaller.Run(methodRequest);

            if (MyLogger.IsLogLevelToOutput(ILogger.LogLevel.DEBUG))
            {
                IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();
                string resultJson = serializer.Serialize<Dictionary<string, object>>(retResp.Results);

                MyLogger.WriteLog(ILogger.LogLevel.DEBUG, $"Response Status={retResp.Status}, Result={resultJson}");
            }
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: DirectMethodCalled");
            return retResp;
        }

    }
}