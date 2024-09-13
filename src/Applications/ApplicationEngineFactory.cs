
namespace TICO.GAUDI.Commons
{

    /// <summary>
    /// Application Engine Factory class
    /// </summary>
    public class ApplicationEngineFactory
    {
        /// <summary>
        /// エンジンインスタンス。(Singleton)					
        /// </summary>
        /// <param name=""></param>
        private static ApplicationEngine applicationEngine = new ApplicationEngine();

        /// <summary>
        /// エンジンインスタンス。(Singleton)					
        /// </summary>
        /// <param name=""></param>
        public static IApplicationEngine GetEngine()
        {
            return applicationEngine;
        }
    }
}