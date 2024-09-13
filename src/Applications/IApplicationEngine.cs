using System;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons
{
    public delegate Task<bool> MessageEventHandler(string inputName, IotMessage message, object userContext);

    public delegate Task<DirectMethodResponse> DirectMethodHandler(string methodName, DirectMethodRequest methodRequest, object userContext);

    /// <summary>
    /// Application Engine Interface class
    /// </summary>
    public interface IApplicationEngine : IDisposable
    {
        /// <summary>
        /// アプリケーションインスタンスを注入。
        /// </summary>
        /// <param name="applicationMain">アプリケーションメインインスタンス</param>
        public void SetApplication(IApplicationMain applicationMain);

        /// <summary>
        /// アプリケーションエンジン起動
        /// </summary>
        public Task RunAsync();

        /// <summary>
        /// アプリケーション実行権獲得
        /// </summary>
        /// <returns>状態遷移結果</returns>
        public Task<ApplicationStateChangeResult> SetApplicationRunningAsync();

        /// <summary>
        /// アプリケーション実行権解放
        /// </summary>
        /// <returns>状態遷移結果</returns>
        public Task<ApplicationStateChangeResult> UnsetApplicationRunningAsync();

        /// <summary>
        /// アプリケーション再起動（致命的障害時用）
        /// </summary>
        /// <returns>状態遷移結果</returns>
        public Task<ApplicationStateChangeResult> SetApplicationRestartAsync();

        /// <summary>
        /// アプリケーション終了中判定
        /// </summary>
        /// <returns>false:未起動・通常状態、true:終了中</returns>
        public bool IsTerminating();
        
        /// <summary>
        /// メッセージ入力時のイベント処理追加
        /// </summary>
        /// <param name="inputName">入力名</param>
        /// <param name="msgHandler">イベントコールバック</param>
        /// <param name="userContext">拡張データ(省略可能)</param>
        public Task AddMessageInputHandlerAsync(string inputName,
                                                    MessageEventHandler msgHandler,
                                                    object userContext = null);

        /// <summary>
        /// メッセージ送信
        /// </summary>
        /// <param name="outputName">出力名</param>
        /// <param name="sendingMsg">送信メッセージ</param>
        public Task SendMessageAsync(string outputName,
                                        IotMessage sendingMsg);

        /// <summary>
        /// ダイレクトメソッド受信時のイベントコールバック追加
        /// </summary>
        /// <param name="methodName">呼び出しダイレクトメソッド名</param>
        /// <param name="methodHandler">イベントコールバック</param>
        /// <param name="userContext">拡張データ</param>
        public Task AddDirectMethodHandlerAsync(string methodName,
                                                    DirectMethodHandler methodHandler,
                                                    object userContext = null);

    }
}