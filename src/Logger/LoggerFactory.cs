using System;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// ロガーファクトリークラス
    /// </summary>
    public class LoggerFactory
    {
        private static bool isLoggerInitialized = false;

        /// <summary>
        /// 指定した Type に対応した ILogger クラスのインスタンスを取得する。
        /// </summary>
        /// <param name="type">ログ出力時に name を使用</param>
        /// <returns>ILoggerインスタンス</returns>
        public static ILogger GetLogger(Type type)
        {
            var logger = Logger.GetLogger(type);

            if (!isLoggerInitialized)
            {
                InitializeLogger(logger);
                isLoggerInitialized = true;
            }
                            
            return logger;
        }
        
        private static void InitializeLogger(ILogger logger)
        {
            // 環境変数より、LogLevel を取得してセット
            var value = Environment.GetEnvironmentVariable("LogLevel");

            if (value == null)
            {
                logger.SetOutputLogLevel("Info");
                logger.WriteLog(ILogger.LogLevel.INFO, $"Environment variable 'LogLevel' is not specified. Using default value: INFO.");
            }
            else if (!Enum.TryParse(typeof(ILogger.LogLevel), value, true, out _))
            {
                logger.SetOutputLogLevel("Info");
                logger.WriteLog(ILogger.LogLevel.WARN, $"Environment variable 'LogLevel' is invalid. Using default value: INFO.");
            }
            else
            {
                logger.SetOutputLogLevel(value);
                logger.WriteLog(ILogger.LogLevel.INFO, $"Environment variable 'LogLevel' retrieved successfully: {value}");
            }
        }
    }
}