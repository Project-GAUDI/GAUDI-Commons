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

    public enum TransportTopic
    {
        /// <summary>IoThubトピック</summary>
        Iothub = 0,
        /// <summary>一般的なMQTTトピック</summary>
        Mqtt = 1
    }

    public interface IModuleClient : IDisposable
    {
        Task OpenAsync();

        Task CloseAsync();

        Task<Twin> GetTwinAsync();

        Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext);

        Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext, TransportTopic transportTopic);

        Task SetInputMessageHandlerAsync(string inputName, IotMessageHandler handler, object userContext);

        Task SetInputMessageHandlerAsync(string inputName, IotMessageHandler handler, object userContext, TransportTopic transportTopic);

        Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext);

        Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext, TransportTopic transportTopic);

        Task SendEventAsync(string outputName, IotMessage message);

        Task SendEventAsync(string outputName, IotMessage message, TransportTopic transportTopic);

        Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties);

        Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties, TransportTopic transportTopic);
    }
}