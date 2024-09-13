using System;

namespace TICO.GAUDI.Commons.Test
{
    public class StubLoggerFactory
    {
        public static ILogger GetLogger(Type type)
        {
            return StubLogger.GetLogger(type);
        }
    }
}