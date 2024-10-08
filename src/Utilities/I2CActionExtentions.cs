using System;

namespace TICO.GAUDI.Commons
{
    public enum I2CAction
    {
        Read,
        Write,
        Wait
    }

    public static class I2CActionExtentions
    {
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
