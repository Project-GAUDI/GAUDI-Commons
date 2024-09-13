using System;
using System.Collections.Generic;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Direct Method Response class
    /// </summary>
    public class DirectMethodResponse
    {
        public int Status { get; set; } = 0;
        public Dictionary<string, object> Results { get; set; } = new Dictionary<string, object>();

        public DirectMethodResponse(int status = 0, string resultKey = null, object resultValue = null)
        {
            Status = status;
            if (false == String.IsNullOrEmpty(resultKey) && null != resultValue)
            {
                Results.Add(resultKey, resultValue);
            }
        }
    }
}