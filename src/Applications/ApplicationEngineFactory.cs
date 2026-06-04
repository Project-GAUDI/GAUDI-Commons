
namespace TICO.GAUDI.Commons
{

    /// <summary>
    /// Application Engine Factory class
    /// </summary>
    public class ApplicationEngineFactory
    {
        /// <summary>
        /// エンジンインスタンス(Singleton)					
        /// </summary>
        private static ApplicationEngine applicationEngine = new ApplicationEngine();

        /// <summary>
        /// IApplicationEngine クラスのインスタンスを取得する。
        /// </summary>
        /// <returns>インスタンス</returns>
        public static IApplicationEngine GetEngine()
        {
            return applicationEngine;
        }
    }
}