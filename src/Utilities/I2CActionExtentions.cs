using System;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// I2Cアクション種別
    /// </summary>
    public enum I2CAction
    {
        /// <summary>
        /// 読み取り
        /// </summary>
        Read,
        /// <summary>
        /// 書き込み
        /// </summary>
        Write,
        /// <summary>
        /// 待機
        /// </summary>
        Wait
    }

    /// <summary>
    /// I2Cアクション拡張メソッドクラス
    /// </summary>
    public static class I2CActionExtentions
    {
        /// <summary>
        /// I2Cアクション種別を文字列（小文字）へ変換する。
        /// 想定外のアクション種別が指定された場合、ArgumentExceptionをスローする。
        /// </summary>
        /// <param name="self">I2Cアクション種別</param>
        /// <returns>文字列（小文字）</returns>
        public static string ToString(this I2CAction self)
        {
            switch (self)
            {
                case I2CAction.Read:
                    return "read";
                case I2CAction.Wait:
                    return "wait";
                case I2CAction.Write:
                    return "write";
                default:
                    throw new ArgumentException($"Unexpected value {self}");
            }
        }

        /// <summary>
        /// 文字列をI2Cアクション種別へ変換する。
        /// 想定外の変換対象文字列が指定された場合、ArgumentExceptionをスローする。
        /// </summary>
        /// <param name="self">変換対象文字列</param>
        /// <returns>I2Cアクション種別</returns>
        public static I2CAction ToI2CAction(this string self)
        {
            I2CAction ret = I2CAction.Read;
            if (Enum.TryParse(self, true, out ret))
            {
                return ret;
            }
            else
            {
                throw new ArgumentException($"Unexpected value {self}");
            }
        }
    }
}
