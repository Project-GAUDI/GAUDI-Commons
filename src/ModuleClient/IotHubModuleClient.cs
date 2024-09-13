using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;


namespace TICO.GAUDI.Commons
{
    internal class IotHubModuleClient : IModuleClient
    {
        private static ILogger MyLogger { get; } = LoggerFactory.GetLogger(typeof(IotHubModuleClient));
        private ModuleClient MyModuleClient { get; set; } = null;
        private TransportTopic defaultSendTopic;
        private TransportTopic defaultReceiveTopic;
        public IotConnectionStatus ConnectionStatus{get;internal set;} = IotConnectionStatus.Disconnected;

        private IotHubModuleClient(){}

        public static async Task<IotHubModuleClient> CreateAsync(ITransportSettings[] settings = null, TransportTopic defaultSendTopic = TransportTopic.Iothub, TransportTopic defaultReceiveTopic = TransportTopic.Iothub, ClientOptions options = null)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: CreateAsync");

            IotHubModuleClient moduleClient = new IotHubModuleClient
            {
                defaultSendTopic = defaultSendTopic,
                defaultReceiveTopic = defaultReceiveTopic
            };

            if (settings == null)
            {
                if (options == null)
                {
                    moduleClient.MyModuleClient = await ModuleClient.CreateFromEnvironmentAsync().ConfigureAwait(false);
                }
                else
                {
                    moduleClient.MyModuleClient = await ModuleClient.CreateFromEnvironmentAsync(options).ConfigureAwait(false);
                }
            }
            else
            {
                if (options == null)
                {
                    moduleClient.MyModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings).ConfigureAwait(false);
                }
                else
                {
                    moduleClient.MyModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings, options).ConfigureAwait(false);
                }
            }

            await moduleClient.SetConnectionStatusChangedHandlerAsync(moduleClient.DefaultConnectionStatusChangeHandler);

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: CreateAsync");
            return moduleClient;
        }

        public async Task CloseAsync()
        {
            await MyModuleClient.CloseAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            MyModuleClient.Dispose();
        }

        public async Task OpenAsync()
        {
            await MyModuleClient.OpenAsync().ConfigureAwait(false);
        }

        public async Task<Twin> GetTwinAsync()
        {
            var twin = await MyModuleClient.GetTwinAsync().ConfigureAwait(false);
            return twin;
        }

        public async Task SendEventAsync(string outputName, IotMessage message)
        {
            await SendEventAsync(outputName, message, defaultSendTopic).ConfigureAwait(false);
        }

        protected async Task SendEventAsync(string outputName, IotMessage message, TransportTopic transportTopic)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: SendEventAsync");

            if (transportTopic == TransportTopic.Iothub)
            {
                if (string.IsNullOrEmpty(message.GetMessageId()))
                {
                    message.SetMessageId(Util.GetMessageId());
                }

                await MyModuleClient.SendEventAsync(outputName, message.GetMessage()).ConfigureAwait(false);
            }
            else if (transportTopic == TransportTopic.Mqtt)
            {
                throw new NotImplementedException($"MqttTopic is not yet implemented in IotHubModuleClient");
            }
            
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: SendEventAsync");
        }

        public async Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext)
        {
            await SetDesiredPropertyUpdateCallbackAsync(callback, userContext, defaultReceiveTopic).ConfigureAwait(false);
        }

        protected async Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext, TransportTopic transportTopic)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: SetDesiredPropertyUpdateCallbackAsync");

            if (transportTopic == TransportTopic.Iothub)
            {
                await MyModuleClient.SetDesiredPropertyUpdateCallbackAsync(callback, userContext).ConfigureAwait(false);
            }
            else if (transportTopic == TransportTopic.Mqtt)
            {
                throw new NotImplementedException($"MqttTopic is not yet implemented in IotHubModuleClient");
            }

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: SetDesiredPropertyUpdateCallbackAsync");
        }

        public async Task SetInputMessageHandlerAsync(string inputName, IotMessageHandler iotHandler, object userContext)
        {
            await SetInputMessageHandlerAsync(inputName, iotHandler, userContext, defaultReceiveTopic).ConfigureAwait(false);
        }

        protected async Task SetInputMessageHandlerAsync(string inputName, IotMessageHandler iotHandler, object userContext, TransportTopic transportTopic)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: SetInputMessageHandlerAsync");

            if (transportTopic == TransportTopic.Iothub)
            {
                MessageHandler handler = async (msg, obj) => { return await iotHandler(new IotMessage(msg), obj); };
                await MyModuleClient.SetInputMessageHandlerAsync(inputName, handler, userContext).ConfigureAwait(false);
            }
            else if (transportTopic == TransportTopic.Mqtt)
            {
                throw new NotImplementedException($"MqttTopic is not yet implemented in IotHubModuleClient");
            }

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: SetInputMessageHandlerAsync");
        }

        public async Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext)
        {
            await SetMethodHandlerAsync(methodName, methodHandler, userContext, defaultReceiveTopic).ConfigureAwait(false);
        }

        protected async Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext, TransportTopic transportTopic)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: SetMethodHandlerAsync");

            if (transportTopic == TransportTopic.Iothub)
            {
                await MyModuleClient.SetMethodHandlerAsync(methodName, methodHandler, userContext).ConfigureAwait(false);
            }
            else if (transportTopic == TransportTopic.Mqtt)
            {
                throw new NotImplementedException($"MqttTopic is not yet implemented in IotHubModuleClient");
            }

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: SetMethodHandlerAsync");
        }

        public async Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties)
        {
            await UpdateReportedPropertiesAsync(reportedProperties, defaultSendTopic).ConfigureAwait(false);
        }

        protected async Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties, TransportTopic transportTopic)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: UpdateReportedPropertiesAsync");

            if (transportTopic == TransportTopic.Iothub)
            {
                await MyModuleClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
            }
            else if (transportTopic == TransportTopic.Mqtt)
            {
                throw new NotImplementedException($"MqttTopic is not yet implemented in IotHubModuleClient");
            }

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: UpdateReportedPropertiesAsync");
        }
        public async Task SetConnectionStatusChangedHandlerAsync(IotConnectionStatusChangeHandler handler)
        {
            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"Start Method: SetConnectionStatusChangedHandlerAsync");

            ConnectionStatusChangesHandler connectionStatusChangeHandler = (status, reason) =>
            {
                handler((IotConnectionStatus)status, (IotConnectionStatusChangeReason)reason);
                // 受信したステータスをプロパティに保存
                ConnectionStatus = (IotConnectionStatus)status;
            };
            MyModuleClient.SetConnectionStatusChangesHandler(connectionStatusChangeHandler);

            await Task.CompletedTask;

            MyLogger.WriteLog(ILogger.LogLevel.TRACE, $"End Method: SetConnectionStatusChangedHandlerAsync");
            return;
        }

        public async Task DefaultConnectionStatusChangeHandler(IotConnectionStatus status, IotConnectionStatusChangeReason reason)
        {
            //空実装
            await Task.CompletedTask;
            return;
        }
    }
}
