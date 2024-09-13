using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Application Main Interface class
    /// </summary>
    public interface IApplicationMain : IDisposable
    {
        /// <summary>
        /// アプリケーション初期化					
        /// システム初期化前に呼び出される
        /// </summary>
        /// <returns></returns>
        public Task<bool> InitializeAsync();

        /// <summary>
        /// アプリケーション起動処理
        /// システム初期化完了後に呼び出される
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public Task<bool> StartAsync();

        /// <summary>
        /// アプリケーション解放。					
        /// </summary>
        /// <returns></returns>
        public Task<bool> TerminateAsync();


        /// <summary>
        /// DesiredPropertis更新コールバック。					
        /// </summary>
        /// <param name="desiredProperties"></param>
        /// <returns></returns>
        public Task<bool> OnDesiredPropertiesReceivedAsync(JObject desiredProperties);

    }
}