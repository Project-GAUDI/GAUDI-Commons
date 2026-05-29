using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Application Main Interface class
    /// </summary>
    public interface IApplicationMain : IAsyncDisposable
    {
        /// <summary>
        /// アプリケーション初期化処理
        /// システム初期化前に呼び出される
        /// </summary>
        /// <returns>処理成功(true)、処理失敗(false)</returns>
        public Task<bool> InitializeAsync();

        /// <summary>
        /// アプリケーション起動処理
        /// システム初期化完了後に呼び出される
        /// </summary>
        /// <returns>処理成功(true)、処理失敗(false)</returns>
        public Task<bool> StartAsync();

        /// <summary>
        /// アプリケーション解放処理
        /// システム終了要求、または、再起動要求があった際に呼び出される
        /// </summary>
        /// <returns>処理成功(true)、処理失敗(false)</returns>
        public Task<bool> TerminateAsync();


        /// <summary>
        /// DesiredProperties 更新コールバック処理
        /// システム初期化完了後（StartAsync より前）と、DesiredProperties 更新通知受信時に呼び出される
        /// </summary>
        /// <param name="desiredProperties">Desired Properties</param>
        /// <returns>処理成功(true)、処理失敗(false)</returns>
        public Task<bool> OnDesiredPropertiesReceivedAsync(JObject desiredProperties);

    }
}