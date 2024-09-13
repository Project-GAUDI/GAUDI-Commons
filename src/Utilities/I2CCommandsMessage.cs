using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TICO.GAUDI.Commons
{
    [DataContract]
    public class I2CCommandsMessage
    {
        [DataMember(Name = "CommandList")]
        public List<I2CCommand> CommandList;

        public static I2CCommandsMessage DeserializeJson(string message)
        {
            try
            {
                return DeserializeJson(Encoding.UTF8.GetBytes(message));

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static I2CCommandsMessage DeserializeJson(byte[] message)
        {
            var serializer = new DataContractJsonSerializer(typeof(I2CCommandsMessage));
            try
            {
                using (var ms = new MemoryStream(message))
                {
                    return (I2CCommandsMessage)serializer.ReadObject(ms);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string SerializeJson(I2CCommandsMessage message)
        {
            var byteAry = SerializeJsonBytes(message);
            if (byteAry != null)
            {
                return Encoding.UTF8.GetString(byteAry);
            }
            return null;
        }

        public static byte[] SerializeJsonBytes(I2CCommandsMessage message)
        {
            if (message == null)
            {
                return null;
            }

            var serializer = new DataContractJsonSerializer(typeof(I2CCommandsMessage));
            try
            {
                using (var ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, message);
                    return ms.ToArray();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    [DataContract]
    public class I2CCommand
    {
        [DataMember(Name = "Action")]
        public string Action { get; set; }

        [DataMember(Name = "Address")]
        public string Address { get; set; }

        [DataMember(Name = "Command")]
        public string Command { get; set; }

        [DataMember(Name = "Data")]
        public string Data { get; set; }

        [DataMember(Name = "Filter")]
        public string Filter { get; set; }

        [DataMember(Name = "Length")]
        public int Length { get; set; }

        [DataMember(Name = "Interval")]
        public int Interval { get; set; }
    }
}
