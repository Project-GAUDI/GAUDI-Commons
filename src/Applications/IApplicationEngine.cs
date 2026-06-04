using System;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// メッセージ受信イベントハンドラー
    /// </summary>
    /// <param name="inputName">インプット名</param>
    /// <param name="message">イベントコールバック</param>
    /// <param name="userContext">拡張データ</param>
    /// <returns>処理成功(true)、処理失敗(false)</returns>
    public delegate Task<bool> MessageEventHandler(string inputName, IotMessage message, object userContext);

    /// <summary>
    /// ダイレクトメソッド受信イベントハンドラー
    /// </summary>
    /// <param name="methodName">メソッド名</param>
    /// <param name="methodRequest">リクエストデータ</param>
    /// <param name="userContext">拡張データ</param>
    /// <returns>実行結果</returns>
    public delegate Task<DirectMethodResponse> DirectMethodHandler(string methodName, DirectMethodRequest methodRequest, object userContext);

    /// <summary>
    /// Application Engine Interface class
    /// </summary>
    public interface IApplicationEngine : IAsyncDisposable
    {
        /// <summary>
        /// 引数で受け取った IApplicationMain インスタンスを内部に保持する。
        /// </summary>
        /// <param name="applicationMain">アプリケーションメインインスタンス</param>
        public void SetApplication(IApplicationMain applicationMain);

        /// <summary>
        /// アプリケーションエンジンを起動する。
        /// </summary>
        /// <returns>タスク</returns>
        public Task RunAsync();

        /// <summary>
        /// アプリケーション実行権を獲得する。
        /// </summary>
        /// <returns>アプリケーション状態変更結果</returns>
        public Task<ApplicationStateChangeResult> SetApplicationRunningAsync();

        /// <summary>
        /// アプリケーション実行権を解放する。
        /// </summary>
        /// <returns>アプリケーション状態変更結果</returns>
        public Task<ApplicationStateChangeResult> UnsetApplicationRunningAsync();

        /// <summary>
        /// アプリケーションを再起動する（致命的障害時用）。
        /// </summary>
        /// <returns>アプリケーション状態変更結果</returns>
        public Task<ApplicationStateChangeResult> SetApplicationRestartAsync();

        /// <summary>
        /// アプリケーションが終了中かどうか判定する。
        /// </summary>
        /// <returns>終了中(true)、未起動・通常状態(false)</returns>
        public bool IsTerminating();
        
        /// <summary>
        /// メッセージ受信時のイベントハンドラを追加する。
        /// </summary>
        /// <param name="inputName">インプット名</param>
        /// <param name="msgHandler">イベントコールバック</param>
        /// <param name="userContext">拡張データ（デフォルト：null）</param>
        /// <returns>タスク</returns>
        public Task AddMessageInputHandlerAsync(string inputName,
                                                    MessageEventHandler msgHandler,
                                                    object userContext = null);

        /// <summary>
        /// 指定したアウトプット名でメッセージを送信する。
        /// </summary>
        /// <param name="outputName">アウトプット名</param>
        /// <param name="sendingMsg">送信メッセージ</param>
        /// <returns>タスク</returns>
        public Task SendMessageAsync(string outputName,
                                        IotMessage sendingMsg);

        /// <summary>
        /// ダイレクトメソッド受信時のイベントハンドラを追加する。
        /// </summary>
        /// <param name="methodName">メソッド名</param>
        /// <param name="methodHandler">イベントコールバック</param>
        /// <param name="userContext">拡張データ（デフォルト：null）</param>
        /// <returns>タスク</returns>
        public Task AddDirectMethodHandlerAsync(string methodName,
                                                    DirectMethodHandler methodHandler,
                                                    object userContext = null);

    }
}