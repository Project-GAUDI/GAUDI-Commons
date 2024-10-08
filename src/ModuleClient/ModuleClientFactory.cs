using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace TICO.GAUDI.Commons
{
    public class ModuleClientFactory
    {
        public static async Task<IModuleClient> CreateAsync()
        {
            return await CreateIotHubModuleClientAsync();
        }

        private static async Task<IModuleClient> CreateIotHubModuleClientAsync()
        {
            ITransportSettings[] settings;
            string protocolEnv = Environment.GetEnvironmentVariable("TransportProtocol");
            if (Enum.TryParse(protocolEnv, true, out TransportProtocol transportProtocol))
            {
                settings = transportProtocol.GetTransportSettings();
            }
            else
            {
                // デフォルトのトランスポートプロトコルを使用
                settings = TransportProtocol.Amqp.GetTransportSettings();
            }

            return await IotHubModuleClient.CreateAsync(settings);
        }
    }
}