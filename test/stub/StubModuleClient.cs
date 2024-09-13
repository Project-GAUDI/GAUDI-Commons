using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    public class StubModuleClient : IModuleClient, IModuleClientTestHelper
    {
        public Dictionary<string,Tuple<IotMessageHandler,object>> _Handlers = new Dictionary<string, Tuple<IotMessageHandler,object>>();
        public IotConnectionStatus ConnectionStatus{get;internal set;} = IotConnectionStatus.Disconnected;
        
        public async Task CloseAsync()
        {
            await Task.CompletedTask;
        }

        public async Task<Twin> GetTwinAsync()
        {
            Twin retTwin = new Twin();

            if ( false == String.IsNullOrEmpty(desiredJSON) ) {
                retTwin.Properties.Desired = new TwinCollection(desiredJSON);
            }

            await Task.CompletedTask;

            return retTwin;
        }

        public async Task OpenAsync()
        {
            await Task.CompletedTask;
        }

        public async Task SendEventAsync(string outputName, IotMessage message)
        {
            if( string.IsNullOrEmpty(outputName) ){
                throw new Exception("outputName is null or Empty.");
            }
            
            if ( _Handlers.ContainsKey(outputName) ) {
                var handler = _Handlers[outputName];
                await handler.Item1(message, handler.Item2);
            }

            await Task.CompletedTask;
            return;
        }

        public async Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext)
        {
            await Task.CompletedTask;
            return;
        }


        public async Task SetInputMessageHandlerAsync(string inputName, IotMessageHandler handler, object userContext)
        {
            _Handlers[inputName] = new Tuple<IotMessageHandler, object>(handler, userContext);

            await Task.CompletedTask;
            return;
        }

        public async Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext)
        {
            await Task.CompletedTask;
            return;
        }


        public async Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties)
        {
            await Task.CompletedTask;
            return;
        }

        public async Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties, TransportTopic transportTopic)
        {
            await Task.CompletedTask;
            return;
        }

        public async Task SetConnectionStatusChangedHandlerAsync(IotConnectionStatusChangeHandler handler)
        {
            await Task.CompletedTask;
            return;
        }

        public void Dispose()
        {

        }

        /// ---------------------------------------------------------
        /// for IModuleClientTestHelper
        /// ---------------------------------------------------------

        /// <summary>
        /// desiredProperties保存領域
        /// staticメンバの為、並列テストには使用不可
        /// </summary>
        static string desiredJSON = "";
        
        /// <summary>
        /// GetTwinAsyncで返されるDesiredPropertiesを設定
        /// </summary>
        /// <param name="desired"></param>
        public void SetDesiredProperties(string desired)
        {
            desiredJSON = desired;
            return;
        }

    }

}