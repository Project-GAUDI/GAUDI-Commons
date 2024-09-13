using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons.Test
{
    /// <summary>
    /// Application Engine IApplicationEngine Implementation class
    /// </summary>
    public partial class StubApplicationEngine : IApplicationEngine
    {

        public void Dispose()
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: Dispose");

            if (null != MyModuleClient)
            {
                MyModuleClient.CloseAsync();
                MyModuleClient.Dispose();
                MyModuleClient = null;
            }

            // Program.cs側で管理する為、Disposeはしない
            applicationMain = null;

            messageInputEventData.Clear();
            methodRequestEventData.Clear();

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: Dispose");
        }

        /// <summary>
        /// アプリケーションインスタンスを注入。
        /// </summary>
        /// <param name="applicationMain">アプリケーションメインインスタンス</param>
        public void SetApplication(IApplicationMain applicationMain)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: SetApplication");
            this.applicationMain = applicationMain;
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: SetApplication");
        }

        /// <summary>
        /// アプリケーションエンジン起動
        /// </summary>
        public async Task RunAsync()
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: RunAsync");

            // アプリケーションインスタンス設定確認
            if (null == applicationMain)
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Exit Method: RunAsync caused by changing ApplicationState failed.", true);
                throw new Exception("Error: ApplicationMain not set.");
            }

            // エンジンの初期化
            var status = false;

            try
            {
                status = await Init();
            }
            catch (Exception ex)
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Exit Method: RunAsync caused by Init() failed.{ex}", true);
                status = false;
            }

            // アプリケーションループに入る
            if (true == status)
            {
                // ステートを待ち状態にする
                var stateChangeResult = Enqueue(ApplicationState.Ready);
                if (ApplicationStateChangeResult.Ignored == stateChangeResult)
                {
                    MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Exit Method: RunAsync caused by failed to change the ApplicationState to 'Ready'.", true);
                    throw new Exception("Error: Failed to change the ApplicationState to 'Ready'.");
                }
                Dequeue();

                // Wait until the app unloads or is cancelled
                var cts = new CancellationTokenSource();
                AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
                Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
                GetCancelWaitTask(cts.Token).Wait();
            }

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: RunAsync");
            return;
        }

        /// <summary>
        /// アプリケーション実行権獲得
        /// </summary>
        /// <returns>状態遷移結果</returns>
        public async Task<ApplicationStateChangeResult> SetApplicationRunningAsync()
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: SetApplicationRunningAsync");

            // ステートを実行状態にする
            var stateChangeResult = Enqueue(ApplicationState.Running);
            if (ApplicationStateChangeResult.Success == stateChangeResult)
            {
                Dequeue();
            }
            else
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Failed to change the ApplicationState to 'Running'.", true);
            }

            await Task.CompletedTask;
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: SetApplicationRunningAsync");
            return stateChangeResult;
        }

        /// <summary>
        /// アプリケーション実行権解放
        /// </summary>
        /// <returns>状態遷移結果</returns>
        public async Task<ApplicationStateChangeResult> UnsetApplicationRunningAsync()
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: UnsetApplicationRunningAsync");

            // ステートを待機状態にする
            var stateChangeResult = Enqueue(ApplicationState.Ready);
            if (ApplicationStateChangeResult.Success == stateChangeResult)
            {
                Dequeue();
            }
            else
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Failed to change the ApplicationState to 'Ready'.", true);
            }

            await Task.CompletedTask;
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: UnsetApplicationRunningAsync");
            return stateChangeResult;
        }

        /// <summary>
        /// アプリケーション再起動（致命的障害時用）
        /// </summary>
        /// <returns>状態遷移結果</returns>
        public async Task<ApplicationStateChangeResult> SetApplicationRestartAsync()
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: SetApplicationRestartAsync");

            var stateChangeResult = ApplicationStateChangeResult.Ignored;

            try
            {
                var result = await Restart().ConfigureAwait(false);

                if (true == result)
                {
                    stateChangeResult = ApplicationStateChangeResult.Success;
                }
            }
            catch (Exception ex)
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Error in Restart.({ex})", true);
                stateChangeResult = ApplicationStateChangeResult.Ignored;
            }

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: SetApplicationRestartAsync");
            return stateChangeResult;
        }

        /// <summary>
        /// 終了中
        /// </summary>
        /// <returns></returns>
        public bool IsTerminating()
        {
            return stateController.IsTerminating;
        }
                /// <summary>
        /// メッセージ入力時のイベント処理追加
        /// </summary>
        /// <param name="inputName">入力名</param>
        /// <param name="msgHandler">イベントコールバック</param>
        /// <param name="userContext">拡張データ(省略可能)</param>
        public async Task AddMessageInputHandlerAsync(string inputName,
                                                        MessageEventHandler msgHandler,
                                                        object userContext = null)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: AddMessageInputHandlerAsync");

            // イベントハンドラデータを記録
            var msgEventData = new MessageEventData(inputName, userContext, msgHandler);
            messageInputEventData.Add(inputName, msgEventData);

            await MyModuleClient.SetInputMessageHandlerAsync(inputName, ReceiveMessageAsync, inputName);

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: AddMessageInputHandlerAsync");
        }

        /// <summary>
        /// メッセージ送信
        /// </summary>
        /// <param name="outputName">出力名</param>
        /// <param name="sendingMsg">送信メッセージ</param>
        public async Task SendMessageAsync(string outputName,
                                            IotMessage sendingMsg)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: SendMessageAsync");

            Action sender = null;
            var status = MyModuleClient.ConnectionStatus;
            switch (status)
            {
                case IotConnectionStatus.Connected:
                    // 接続済み・使用可能
                    sender = async () =>
                    {
                        await MyModuleClient.SendEventAsync(outputName, sendingMsg);
                    };
                    break;
                case IotConnectionStatus.Disabled:
                    // クローズ済み・使用不可
                    sender = null;
                    break;
                case IotConnectionStatus.Disconnected:
                    // 切断済み・再接続するとしても、Clientの再構成するので、使用不可
                    sender = null;
                    break;
                case IotConnectionStatus.Disconnected_Retrying:
                    // 切断発生・再接続中・更新不可
                    sender = async () =>
                    {
                        MyLogger.WriteLog(ILogger.LogLevel.INFO, $"Waiting for reconnecting...");
                        while (IotConnectionStatus.Disconnected_Retrying == MyModuleClient.ConnectionStatus)
                        {
                            await Task.Delay(1000);
                        }
                        await MyModuleClient.SendEventAsync(outputName, sendingMsg);
                    };
                    break;
                default:
                    break;
            }

            if (null == sender)
            {
                MyLogger.WriteLog(ILogger.LogLevel.ERROR, $"Exit Method: SendMessageAsync caused by ModuleClient disconnected.(status={status.ToString()})", true);
                throw new Exception("Message send error.(ModuleClient disconnected)");
            }
            else
            {
                await Task.Run(sender);
            }

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: SendMessageAsync");
            return;
        }

        /// <summary>
        /// ダイレクトメソッド受信時のイベントコールバック追加
        /// </summary>
        /// <param name="methodName">呼び出しダイレクトメソッド名</param>
        /// <param name="methodHandler">イベントコールバック</param>
        /// <param name="userContext">拡張データ</param>
        public async Task AddDirectMethodHandlerAsync(string methodName,
                                                        DirectMethodHandler methodHandler,
                                                        object userContext = null)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: AddDirectMethodHandlerAsync");

            // ダイレクトメソッドハンドラデータを記録
            var methodRequestData = new MethodEventData(methodName, userContext, methodHandler);
            methodRequestEventData.Add(methodName, methodRequestData);

            await MyModuleClient.SetMethodHandlerAsync(methodName, ReceiveMethodAsync, methodName);

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: AddDirectMethodHandlerAsync");
        }

    }
}