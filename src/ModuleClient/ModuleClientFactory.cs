using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// モジュールクライアントファクトリークラス
    /// </summary>
    public class ModuleClientFactory
    {
        /// <summary>
        /// ENV:TransportProtocolの値に基づいてトランスポート設定を選定し、IModuleClientクラスのインスタンスを生成する。
        /// ENV:TransportProtocolの値が不正な場合、ArgumentException をスローする。
        /// </summary>
        /// <returns>IModuleClientクラスのインスタンス</returns>
        public static async Task<IModuleClient> CreateAsync()
        {
            return await CreateIotHubModuleClientAsync();
        }

        private static async Task<IModuleClient> CreateIotHubModuleClientAsync()
        {
            ITransportSettings[] settings;

            try
            {
                TransportProtocol transportProtocol = Util.GetEnvironmentVariable("TransportProtocol", false, TransportProtocol.Amqp);
                settings = transportProtocol.GetTransportSettings();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return await IotHubModuleClient.CreateAsync(settings);
        }
    }
}