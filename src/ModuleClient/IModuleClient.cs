using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;


namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Iotメッセージハンドラの定義
    /// </summary>
    public delegate Task<MessageResponse> IotMessageHandler(IotMessage message, object userContext);
    public delegate Task IotConnectionStatusChangeHandler(IotConnectionStatus status, IotConnectionStatusChangeReason reason);

    public enum TransportTopic
    {
        /// <summary>IoThubトピック</summary>
        Iothub = 0,
        /// <summary>一般的なMQTTトピック</summary>
        Mqtt = 1
    }
    public enum IotConnectionStatus
    {
        /// <summary>切断</summary>
        Disconnected = 0,
        /// <summary>接続</summary>
        Connected = 1,
        /// <summary>再接続を試みている</summary>
        Disconnected_Retrying = 2,
        /// <summary>接続が閉じられた</summary>
        Disabled = 3
    }
    public enum IotConnectionStatusChangeReason
    {

        Connection_Ok = 0,

        Expired_SAS_Token = 1,

        Device_Disabled = 2,

        Bad_Credential = 3,

        Retry_Expired = 4,

        No_Network = 5,

        Communication_Error = 6,

        Client_Close = 7,
    }

    public interface IModuleClient : IDisposable
    {
        IotConnectionStatus ConnectionStatus{ get; }

        Task OpenAsync();

        Task CloseAsync();

        Task<Twin> GetTwinAsync();

        Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext);

        Task SetInputMessageHandlerAsync(string inputName, IotMessageHandler handler, object userContext);

        Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext);

        Task SendEventAsync(string outputName, IotMessage message);

        Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties);

        Task SetConnectionStatusChangedHandlerAsync(IotConnectionStatusChangeHandler handler);
    }
}