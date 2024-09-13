using System;

namespace TICO.GAUDI.Commons
{
    public class LoggerFactory
    {
        public static ILogger GetLogger(Type type)
        {
            return Logger.GetLogger(type);
        }
    }
}