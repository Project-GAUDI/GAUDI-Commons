using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace TICO.GAUDI.Commons.Test
{
 
    /// <summary>
    /// Application Engine IApplicationEngineInternal Implementation class
    /// </summary>
    public partial class StubApplicationEngine : IApplicationEngine
    {
        /// <summary>
        /// エンジンの初期化(internal)
        /// </summary>
        public async Task<bool> Init()
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: Init");

            if (null == applicationMain)
            {
                MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Exit Method: Init caused by applicationMain not set.");
                throw new Exception("Error: ApplicationMain not set.");
            }

            // ステートを初期化状態にする
            var stateChangeResult = Enqueue(ApplicationState.Initialize);
            if (ApplicationStateChangeResult.Ignored == stateChangeResult)
            {
                throw new Exception("Error: Failed to change the ApplicationState to 'Initialize'.");
            }

            // アプリケーションの初期化（Desired取得前）
            var status = await applicationMain.InitializeAsync();
            MyLogger.WriteLog(ILogger.LogLevel.INFO, $"Application initialized.");

            // 通信の初期化
            try
            {
                MyModuleClient = new StubModuleClient();
            }
            catch (Exception ex)
            {
                MyLogger.WriteLog(ILogger.LogLevel.WARN, $"Creating Module Client failed. {ex.Message}");
                status = false;
            }
            MyLogger.WriteLog(ILogger.LogLevel.INFO, $"ModuelClient created.");

            // edgeHubへの接続
            while (status)
            {
                try
                {
                    await MyModuleClient.OpenAsync().ConfigureAwait(false);
                    break;
                }
                catch (Exception ex)
                {
                    MyLogger.WriteLog(ILogger.LogLevel.WARN, $"Open a connection to the Edge runtime was failed. {ex.Message}");
                    await Task.Delay(1000);
                }
            }
            MyLogger.WriteLog(ILogger.LogLevel.INFO, $"Connection to edgeHub established.");

            // Logger設定
            if (true == status)
            {
                // Loggerへモジュールクライアントを設定
                MyLogger.SetModuleClient(MyModuleClient);

                // 環境変数からログレベルを設定
                string logEnv = Environment.GetEnvironmentVariable("LogLevel");
                try
                {
                    if (logEnv != null) MyLogger.SetOutputLogLevel(logEnv);
                    MyLogger.WriteLog(ILogger.LogLevel.INFO, $"Output log level is: {MyLogger.OutputLogLevel.ToString()}");
                }
                catch (ArgumentException)
                {
                    MyLogger.WriteLog(ILogger.LogLevel.WARN, $"Environment LogLevel does not expected string. Default value ({MyLogger.OutputLogLevel.ToString()}) assigned.");
                    status = false;
                }
            }
            MyLogger.WriteLog(ILogger.LogLevel.INFO, $"Logger initialized.");

            // desiredプロパティの取得
            while (status)
            {
                try
                {
                    var twin = await MyModuleClient.GetTwinAsync().ConfigureAwait(false);
                    var desiredProperties = twin.Properties.Desired;
                    MyLogger.WriteLog(ILogger.LogLevel.INFO, $"Module Twin received.");

                    await DesiredPropertiesUpdate(desiredProperties, null);
                    MyLogger.WriteLog(ILogger.LogLevel.INFO, $"DesiredProperties update method called.");

                    // プロパティ更新時のコールバックを登録
                    await MyModuleClient.SetDesiredPropertyUpdateCallbackAsync(ReceiveTwinAsync, null).ConfigureAwait(false);
                    MyLogger.WriteLog(ILogger.LogLevel.INFO, $"DesiredProperties update callback was set.");

                    break;
                }
                catch (Exception ex)
                {
                    MyLogger.WriteLog(ILogger.LogLevel.WARN, $"Receiving module twin was failed. {ex.Message}");
                    await Task.Delay(1000);
                }
            }

            // ダイレクトメソッドコールバックの設定
            if (true == status)
            {
                await AddDirectMethodHandlerAsync( "SetLogLevel",  DirectMethodCalled, null );
                await AddDirectMethodHandlerAsync( "GetLogLevel",  DirectMethodCalled, null );
                MyLogger.WriteLog(ILogger.LogLevel.INFO, $"DirectMethod handler was set.");
            }

            // 通信切断時コールバックの設定
            if (true == status)
            {
                await MyModuleClient.SetConnectionStatusChangedHandlerAsync(OnConnectionStatusChangedAsync);
                MyLogger.WriteLog(ILogger.LogLevel.INFO, $"Connection status change handler was setled.");
            }

            // アプリケーションの始動（Desired取得後）
            if (true == status)
            {
                status = await applicationMain.StartAsync();
                MyLogger.WriteLog(ILogger.LogLevel.INFO, $"Application start result: {status}");
            }

            // 初期化状態を抜ける
            stateChangeResult = Dequeue();
            if (ApplicationStateChangeResult.Ignored == stateChangeResult)
            {
                throw new Exception("Error: Failed to exit the ApplicationState from 'Initialize'.");
            }

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: Init");
            return status;
        }

        /// <summary>
        /// エンジンの開放処理(internal)
        /// </summary>
        public async Task<bool> Term()
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: Term");
            var status = true;

            // Terminateへ遷移
            var stateChangeResult = Enqueue(ApplicationState.Terminate);
            if (ApplicationStateChangeResult.Ignored == stateChangeResult)
            {
                MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Exit Method: Restart caused by failed to change the ApplicationState to 'Terminate'.");
                throw new Exception("Error: Failed to change the ApplicationState to 'Terminate'.");
            }

            if (null != applicationMain)
            {
                // アプリケーションの終了処理（開放はしない）
                await applicationMain.TerminateAsync();
                MyLogger.WriteLog(ILogger.LogLevel.INFO, $"Application Terminated.");
            }

            // 取得済みのModuleClientを解放する
            if (MyModuleClient != null)
            {
                messageInputEventData.Keys.Select(async (key) =>
                {
                    await MyModuleClient.SetInputMessageHandlerAsync(key, null, null);
                    return key;
                });
                methodRequestEventData.Keys.Select(async (key) =>
                {
                    await MyModuleClient.SetMethodHandlerAsync(key, null, null);
                    return key;
                });
                messageInputEventData.Clear();
                methodRequestEventData.Clear();
                await MyModuleClient.SetDesiredPropertyUpdateCallbackAsync(null, null);
                await MyModuleClient.CloseAsync();
                MyModuleClient.Dispose();
                MyModuleClient = null;
            }

            Dequeue();

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: Term");
            return status;
        }

        /// <summary>
        /// エンジンの再起動(internal)
        /// </summary>
        public async Task<bool> Restart()
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: Restart");

            var status = true;

            if (null == applicationMain)
            {
                MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Exit Method: Restart caused by applicationMain not set.");
                throw new Exception("Error: ApplicationMain not set.");
            }

            // エンジンの終了処理
            var result = await Term();
            // （終了処理ステータスは無視）

            // エンジンの再初期化
            result = await Init();
            if (false == result)
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Exit Method: Restart caused by failed to re-initialize ApplicationEngine.", true);
                throw new Exception("Error: Failed to change the ApplicationState to 'Ready'.");
            }

            // Readyに入る
            var stateChangeResult = Enqueue(ApplicationState.Ready);
            if (ApplicationStateChangeResult.Ignored == stateChangeResult)
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Exit Method: Restart caused by failed to change the ApplicationState to 'Ready'.", true);
                throw new Exception("Error: Failed to change the ApplicationState to 'Ready'.");
            }
            Dequeue();

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: Restart");
            return status;
        }

        /// <summary>
        /// 指定状態への遷移キューイング(internal)
        /// </summary>
        /// <param name="appState">遷移先状態</param>
        /// <returns>状態遷移結果</returns>
        public ApplicationStateChangeResult Enqueue(ApplicationState appState)
        {
            // MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: Enqueue");

            var currentState = stateController.CurrentState;
            var retStateChangeResult = stateController.ChangeState(appState);
            if (ApplicationStateChangeResult.Ignored == retStateChangeResult)
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"State change failed. {currentState} -> {appState}");
            }
            else
            {
                MyLogger.WriteLog(ILogger.LogLevel.DEBUG, $"State changed. {currentState} -> {appState}");
            }

            // MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: Enqueue");
            return retStateChangeResult;
        }

        /// <summary>
        /// 指定状態への遷移キュー解放(internal)
        /// </summary>
        /// <returns>状態遷移結果</returns>
        public ApplicationStateChangeResult Dequeue()
        {
            // MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: Dequeue");

            var currentState = stateController.CurrentState;
            var retStateChangeResult = ApplicationStateChangeResult.Ignored;

            // 今のところ固有の実装なし
            switch (currentState)
            {
                case ApplicationState.Start:
                    retStateChangeResult = ApplicationStateChangeResult.Success;
                    break;
                case ApplicationState.Initialize:
                    retStateChangeResult = ApplicationStateChangeResult.Success;
                    break;
                case ApplicationState.Ready:
                    retStateChangeResult = ApplicationStateChangeResult.Success;
                    break;
                case ApplicationState.Running:
                    retStateChangeResult = ApplicationStateChangeResult.Success;
                    break;
                case ApplicationState.Terminate:
                    retStateChangeResult = ApplicationStateChangeResult.Success;
                    break;
                case ApplicationState.End:
                    retStateChangeResult = ApplicationStateChangeResult.Success;
                    break;
                default:
                    break;
            }

            // MyLogger.WriteLog(ILogger.LogLevel.DEBUG, $"Return result : {retStateChangeResult}");
            // MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: Dequeue");
            return retStateChangeResult;
        }

        /// <summary>
        /// メッセージ受信時コールバック(internal)
        /// </summary>
        /// <param name="message">受信メッセージ</param>
        /// <param name="userContext">拡張データ</param>
        /// <returns>メッセージ処理結果</returns>
        public async Task<MessageResponse> ReceiveMessageAsync(IotMessage message,
                                                                object userContext)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: ReceiveMessageAsync");

            MessageResponse retResp = MessageResponse.None;

            // ステートを実行中状態にする
            ApplicationStateChangeResult stateChangeResult = Enqueue(ApplicationState.Running);
            if (ApplicationStateChangeResult.Ignored == stateChangeResult)
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Failed to change the ApplicationState to 'Running'.", true);
                retResp = MessageResponse.Abandoned;
                return retResp;
            }
            Dequeue();

            try
            {

                var eventData = messageInputEventData[(string)userContext];

                if (null != eventData)
                {
                    bool result = await eventData.MsgHandler(eventData.InputName, message, eventData.UserContext);

                    retResp = result.ToMessageResponse();
                }
                else
                {
                    MyLogger.WriteLog(ILogger.LogLevel.WARN, $"Warning : {(string)userContext} input Handler not found.");
                    retResp = MessageResponse.Abandoned;
                }
            }
            catch (Exception ex)
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"ReceiveMessageAsync failed. {ex}", true);
                retResp = MessageResponse.Abandoned;
            }

            // ステートをReady状態に戻す
            stateChangeResult = Enqueue(ApplicationState.Ready);
            if (ApplicationStateChangeResult.Ignored == stateChangeResult)
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Failed to change the ApplicationState to 'Ready'.", true);
                retResp = MessageResponse.Abandoned;
                return retResp;
            }
            Dequeue();

            MyLogger.WriteLog(ILogger.LogLevel.DEBUG, $"Return MessageResponse : {retResp}");
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: ReceiveMessageAsync");
            return retResp;
        }

        /// <summary>
        /// Twin受信時コールバック(internal)
        /// </summary>
        /// <param name="twin">受信Twin</param>
        /// <param name="userContext">拡張データ</param>
        public async Task ReceiveTwinAsync(TwinCollection desiredProperties,
                                            object userContext)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: ReceiveTwinAsync");

            try
            {
                await Restart();
            }
            catch (Exception ex)
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Restart() failed. {ex}");
            }

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: ReceiveTwinAsync");
            return;
        }

        /// <summary>
        /// ダイレクトメソッド受信時コールバック(internal)
        /// </summary>
        /// <param name="request">ダイレクトメソッドリクエストデータ</param>
        /// <param name="userContext">拡張データ</param>
        /// <returns>ダイレクトメソッド応答</returns>
        public async Task<MethodResponse> ReceiveMethodAsync(MethodRequest request,
                                                                    object userContext)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: ReceiveMethodAsync");

            DirectMethodResponse resp = null;
            string methodName = (string)userContext;

            // ステートを実行中状態にする
            ApplicationStateChangeResult stateChangeResult = Enqueue(ApplicationState.Running);
            if (ApplicationStateChangeResult.Ignored == stateChangeResult)
            {
                string key = "Error";
                string msg = $"Calling {methodName} method failed.(Failed to change the ApplicationState to 'Running')";
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, msg, true);
                resp = new DirectMethodResponse(-1, key, msg);

                return resp.ToMethodResponse();
            }
            Dequeue();

            try
            {
                var eventData = methodRequestEventData[methodName];

                if (null != eventData)
                {
                    resp = await eventData.MethodHandler(eventData.MethodName, request.ToDirectMethodRequest(), eventData.UserContext);
                }
                else
                {
                    string key = "Warning";
                    string msg = $"{methodName} method Handler not found.";
                    MyLogger.WriteLog(ILogger.LogLevel.WARN, msg);
                    resp = new DirectMethodResponse(-1, key, msg);
                }
            }
            catch (Exception ex)
            {
                string key = "Error";
                string msg = $"Calling {methodName} method failed.({ex})";
                resp = new DirectMethodResponse(-1, key, msg);

                MyLogger.WriteLog(ILogger.LogLevel.ERROR, msg, true);
            }

            // ステートをReady状態に戻す
            stateChangeResult = Enqueue(ApplicationState.Ready);
            if (ApplicationStateChangeResult.Ignored == stateChangeResult)
            {
                string key = "Error2";
                string msg = $"Calling {methodName} method failed.(Failed to change the ApplicationState to 'Running')";
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, msg, true);
                // 既存出力がある場合の為、追加する形で設定
                if (null == resp)
                {
                    resp = new DirectMethodResponse();
                }
                resp.Status = -1;
                resp.Results.Add(key, msg);
            }
            Dequeue();

            MethodResponse retResp = null;
            if (null != resp)
            {
                retResp = resp.ToMethodResponse();
            }

            MyLogger.WriteLog(ILogger.LogLevel.DEBUG, $"retResp : Status={retResp.Status}, Result={retResp.ResultAsJson}");
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: ReceiveMethodAsync");
            return retResp;
        }

        /// <summary>
        /// 通信ステータス更新時のコールバック(internal)
        /// </summary>
        /// <param name="status">ステータス</param>
        /// <param name="reason">更新理由</param>
        public async Task OnConnectionStatusChangedAsync(IotConnectionStatus status,
                                                            IotConnectionStatusChangeReason reason)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: OnConnectionStatusChanged");

            bool toReconnect = false;

            MyLogger.WriteLog(ILogger.LogLevel.DEBUG, $"Client connection status changed to ${status.ToString()}. reason:{reason.ToString()}");
            switch (status)
            {
                case IotConnectionStatus.Connected:
                    // 接続済み・使用可能
                    break;
                case IotConnectionStatus.Disabled:
                    // クローズ済み・使用不可
                    break;
                case IotConnectionStatus.Disconnected:
                    // 切断済み・切断理由によっては、再接続
                    toReconnect = reason.IsReconnectableReason();
                    break;
                case IotConnectionStatus.Disconnected_Retrying:
                    // 切断発生・再接続中・更新不可
                    break;
                default:
                    break;
            }

            // 再接続可能な場合、リスタートをかける
            if (toReconnect)
            {
                try
                {
                    await Restart();
                }
                catch (Exception ex)
                {
                    MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Restart() failed. {ex}");
                }
            }

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: OnConnectionStatusChanged");
            return;
        }

    }
}