using System.Collections.Generic;
using System.IO;

namespace TICO.GAUDI.Commons.Test
{
    public interface IStubLoggerResults
    {
        public List<string> GetLogs();

        public List<IotMessage> GetMessages();
    }
}