using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;


namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Iotメッセージハンドラの定義
    /// </summary>
    /// <param name="message">Iotメッセージ</param>
    /// <param name="userContext">ユーザーコンテキスト</param>
    /// <returns>メッセージレスポンス</returns>
    public delegate Task<MessageResponse> IotMessageHandler(IotMessage message, object userContext);

    /// <summary>
    /// Iot接続状態変更ハンドラの定義
    /// </summary>
    /// <param name="status">接続状態</param>
    /// <param name="reason">変更理由</param>
    /// <returns>タスク</returns>
    public delegate Task IotConnectionStatusChangeHandler(IotConnectionStatus status, IotConnectionStatusChangeReason reason);

    /// <summary>
    /// トランスポートトピック種別
    /// </summary>
    public enum TransportTopic
    {
        /// <summary>
        /// IoThubトピック
        /// </summary>
        Iothub = 0,
        /// <summary>
        /// 一般的なMQTTトピック（未使用）
        /// </summary>
        Mqtt = 1
    }

    /// <summary>
    /// Iot接続状態
    /// </summary>
    public enum IotConnectionStatus
    {
        /// <summary>
        /// 切断
        /// </summary>
        Disconnected = 0,
        /// <summary>
        /// 接続
        /// </summary>
        Connected = 1,
        /// <summary>
        /// 再接続を試みている
        /// </summary>
        Disconnected_Retrying = 2,
        /// <summary>
        /// 接続が閉じられた
        /// </summary>
        Disabled = 3
    }
    
    /// <summary>
    /// Iot接続状態変更理由
    /// </summary>
    public enum IotConnectionStatusChangeReason
    {
        /// <summary>
        /// 正常
        /// </summary>
        Connection_Ok = 0,
        /// <summary>
        /// SASトークン期限切れ
        /// </summary>
        Expired_SAS_Token = 1,
        /// <summary>
        /// デバイス無効
        /// </summary>
        Device_Disabled = 2,
        /// <summary>
        /// 認証情報不正
        /// </summary>
        Bad_Credential = 3,
        /// <summary>
        /// リトライ期限切れ
        /// </summary>
        Retry_Expired = 4,
        /// <summary>
        /// ネットワークなし
        /// </summary>
        No_Network = 5,
        /// <summary>
        /// 通信エラー
        /// </summary>
        Communication_Error = 6,
        /// <summary>
        /// クライアントクローズ
        /// </summary>
        Client_Close = 7,
    }

    /// <summary>
    /// モジュールクライアントインターフェース
    /// </summary>
    public interface IModuleClient : IDisposable
    {
        /// <summary>
        /// 接続状態
        /// </summary>
        IotConnectionStatus ConnectionStatus{ get; }

        /// <summary>
        /// 接続開始処理
        /// </summary>
        /// <returns>タスク</returns>
        Task OpenAsync();

        /// <summary>
        /// 切断処理
        /// </summary>
        /// <returns>タスク</returns>
        Task CloseAsync();

        /// <summary>
        /// Twin情報取得処理
        /// </summary>
        /// <returns>Twin 情報</returns>
        Task<Twin> GetTwinAsync();

        /// <summary>
        /// Desiredプロパティ更新時のコールバックを設定する。
        /// </summary>
        /// <param name="callback">Desiredプロパティ更新コールバック</param>
        /// <param name="userContext">ユーザーコンテキスト</param>
        /// <returns>タスク</returns>
        Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext);

        /// <summary>
        /// 入力メッセージを処理するためのコールバックを設定する。
        /// </summary>
        /// <param name="inputName">インプット名</param>
        /// <param name="handler">IoTメッセージハンドラ</param>
        /// <param name="userContext">ユーザーコンテキスト</param>
        /// <returns>タスク</returns>
        Task SetInputMessageHandlerAsync(string inputName, IotMessageHandler handler, object userContext);

        /// <summary>
        /// ダイレクトメソッドを処理するためのコールバックを設定する。
        /// </summary>
        /// <param name="methodName">メソッド名</param>
        /// <param name="methodHandler">メソッドコールバック</param>
        /// <param name="userContext">ユーザーコンテキスト</param>
        /// <returns>タスク</returns>
        Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext);

        /// <summary>
        /// 指定したアプトプット名とIoTメッセージを使って、IoTハブにイベントを送信する。
        /// IoTメッセージにメッセージIDが設定されていない場合、生成して設定する。
        /// </summary>
        /// <param name="outputName">アウトプット名</param>
        /// <param name="message">IoTメッセージ</param>
        /// <returns>タスク</returns>
        Task SendEventAsync(string outputName, IotMessage message);

        /// <summary>
        /// Reportedプロパティを更新する。
        /// </summary>
        /// <param name="reportedProperties">Reportedプロパティ</param>
        /// <returns>タスク</returns>
        Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties);

        /// <summary>
        /// 接続状態の変更通知を処理するためのコールバックを設定する。
        /// 受信した接続状態は、プロパティ：ConnectionStatusに保存する。
        /// </summary>
        /// <param name="handler">Iot接続状態変更ハンドラ</param>
        /// <returns>タスク</returns>
        Task SetConnectionStatusChangedHandlerAsync(IotConnectionStatusChangeHandler handler);
    }
}