using System;

namespace TICO.GAUDI.Commons.Test
{
    public class StubLoggerFactory
    {
        private static bool isLoggerInitialized = false;

        public static ILogger GetLogger(Type type)
        {
            var logger = StubLogger.GetLogger(type);

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