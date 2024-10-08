using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;


namespace TICO.GAUDI.Commons
{

    /// <summary>
    /// Application Engine Interface class
    /// </summary>
    public interface IApplicationEngineInternal : IApplicationEngine
    {

        /// <summary>
        /// エンジンの初期化(internal)
        /// </summary>
        public Task<bool> Init();

        /// <summary>
        /// エンジンの待機(internal)
        /// </summary>
        public Task<bool> Ready();

        /// <summary>
        /// エンジンの開放(internal)
        /// </summary>
        /// <param name="forced">
        /// 強制終了フラグ
        /// true：ステート遷移成否によらず、実行。
        /// end：ステート遷移失敗時はエラー。
        /// </param>
        public Task<bool> Term(bool forced=false);

        /// <summary>
        /// エンジンの停止(internal)
        /// </summary>
        /// <param name="forced">
        /// 強制終了フラグ
        /// true：ステート遷移成否によらず、実行。
        /// end：ステート遷移失敗時はエラー。
        /// </param>
        public Task<bool> End(bool forced=false);

        /// <summary>
        /// エンジンの再起動(internal)
        /// </summary>
        public Task<bool> Restart();

        /// <summary>
        /// エンジンの終了(internal)
        /// </summary>
        /// <param name="forced">
        /// 強制終了フラグ
        /// true：ステート遷移成否によらず、実行。
        /// end：ステート遷移失敗時はエラー。
        /// </param>
        public Task<bool> Terminate(bool forced=false);

        /// <summary>
        /// 指定状態への遷移キューイング(internal)
        /// </summary>
        /// <param name="appState">遷移先状態</param>
        /// <returns>状態遷移結果</returns>
        public ApplicationStateChangeResult Enqueue(ApplicationState appState);

        /// <summary>
        /// 指定状態への遷移キュー解放(internal)
        /// </summary>
        /// <returns>状態遷移結果</returns>
        public ApplicationStateChangeResult Dequeue();

        /// <summary>
        /// メッセージ受信時コールバック(internal)
        /// </summary>
        /// <param name="message">受信メッセージ</param>
        /// <param name="userContext">拡張データ</param>
        /// <returns>メッセージ処理結果</returns>
        public Task<MessageResponse> ReceiveMessageAsync(IotMessage message,
                                                            object userContext);

        /// <summary>
        /// Twin受信時コールバック(internal)
        /// </summary>
        /// <param name="twin">受信Twin</param>
        /// <param name="userContext">拡張データ</param>
        public Task ReceiveTwinAsync(TwinCollection twin,
                                        object userContext);

        /// <summary>
        /// ダイレクトメソッド受信時コールバック(internal)
        /// </summary>
        /// <param name="request">ダイレクトメソッドリクエストデータ</param>
        /// <param name="userContext">拡張データ</param>
        /// <returns>ダイレクトメソッド応答</returns>
        public Task<MethodResponse> ReceiveMethodAsync(MethodRequest request,
                                                        object userContext);

        /// <summary>
        /// 通信ステータス更新時のコールバック(internal)
        /// </summary>
        /// <param name="status">ステータス</param>
        /// <param name="reason">更新理由</param>
        public Task OnConnectionStatusChangedAsync(IotConnectionStatus status,
                                                    IotConnectionStatusChangeReason reason);


    }
}