using System;
using System.Collections.Generic;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Direct Method Response class
    /// </summary>
    public class DirectMethodResponse
    {
        /// <summary>
        /// ステータスコード
        /// </summary>
        public int Status { get; set; } = 0;
        /// <summary>
        /// 実行結果ディクショナリ
        /// </summary>
        public Dictionary<string, object> Results { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="status">ステータスコード（デフォルト：0）</param>
        /// <param name="resultKey">結果キー（デフォルト：null）</param>
        /// <param name="resultValue">結果値（デフォルト：null）</param>
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