
namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Direct Method Request class
    /// </summary>
    public class DirectMethodRequest
    {
        public string MethodName { get; set; } = "";
        public string RequestJson { get; set; } = "";

        public DirectMethodRequest()
        {
        }

        public DirectMethodRequest(string methodName, string requestJson)
        {
            MethodName = methodName;
            RequestJson = requestJson;
        }
    }
}