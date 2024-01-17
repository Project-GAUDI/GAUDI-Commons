using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace TICO.GAUDI.Commons
{
    public class IotHubModuleClient : IModuleClient
    {
        private ModuleClient MyModuleClient { get; set; } = null;
        private TransportTopic defaultSendTopic;
        private TransportTopic defaultReceiveTopic;

        private IotHubModuleClient()
        {
        }

        public static async Task<IotHubModuleClient> CreateAsync(ITransportSettings[] settings = null, TransportTopic defaultSendTopic = TransportTopic.Iothub, TransportTopic defaultReceiveTopic = TransportTopic.Iothub, ClientOptions options = null)
        {
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

        public async Task SendEventAsync(string outputName, IotMessage message, TransportTopic transportTopic)
        {
            if (transportTopic == TransportTopic.Iothub)
            {
                if(string.IsNullOrEmpty(message.message.MessageId))
                {
                    message.message.MessageId = Guid.NewGuid().ToString();
                }

                await MyModuleClient.SendEventAsync(outputName, message.message).ConfigureAwait(false);
            }
            else if (transportTopic == TransportTopic.Mqtt)
            {
                throw new NotImplementedException($"MqttTopic is not yet implemented in IotHubModuleClient");
            }
        }

        public async Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext)
        {
            await SetDesiredPropertyUpdateCallbackAsync(callback, userContext, defaultReceiveTopic).ConfigureAwait(false);
        }

        public async Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext, TransportTopic transportTopic)
        {
            if (transportTopic == TransportTopic.Iothub)
            {
                await MyModuleClient.SetDesiredPropertyUpdateCallbackAsync(callback, userContext).ConfigureAwait(false);
            }
            else if (transportTopic == TransportTopic.Mqtt)
            {
                throw new NotImplementedException($"MqttTopic is not yet implemented in IotHubModuleClient");
            }
        }

        public async Task SetInputMessageHandlerAsync(string inputName, IotMessageHandler iotHandler, object userContext)
        {
            await SetInputMessageHandlerAsync(inputName, iotHandler, userContext, defaultReceiveTopic).ConfigureAwait(false);
        }

        public async Task SetInputMessageHandlerAsync(string inputName, IotMessageHandler iotHandler, object userContext, TransportTopic transportTopic)
        {
            if (transportTopic == TransportTopic.Iothub)
            {
                MessageHandler handler = async (msg, obj)=>{return await iotHandler(new IotMessage(msg), obj);};
                await MyModuleClient.SetInputMessageHandlerAsync(inputName, handler, userContext).ConfigureAwait(false);
            }
            else if (transportTopic == TransportTopic.Mqtt)
            {
                throw new NotImplementedException($"MqttTopic is not yet implemented in IotHubModuleClient");
            }
        }

        public async Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext)
        {
            await SetMethodHandlerAsync(methodName, methodHandler, userContext, defaultReceiveTopic).ConfigureAwait(false);
        }

        public async Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext, TransportTopic transportTopic)
        {
            if (transportTopic == TransportTopic.Iothub)
            {
                await MyModuleClient.SetMethodHandlerAsync(methodName, methodHandler, userContext).ConfigureAwait(false);
            }
            else if (transportTopic == TransportTopic.Mqtt)
            {
                throw new NotImplementedException($"MqttTopic is not yet implemented in IotHubModuleClient");
            }
        }

        public async Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties)
        {
            await UpdateReportedPropertiesAsync(reportedProperties, defaultSendTopic).ConfigureAwait(false);
        }

        public async Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties, TransportTopic transportTopic)
        {
            if (transportTopic == TransportTopic.Iothub)
            {
                await MyModuleClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
            }
            else if (transportTopic == TransportTopic.Mqtt)
            {
                throw new NotImplementedException($"MqttTopic is not yet implemented in IotHubModuleClient");
            }
        }
    }
}
