using System;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Newtonsoft.Json.Linq;

namespace TICO.GAUDI.Commons
{
    public enum TransportProtocol
    {
        Amqp,
        Mqtt,
        // Http1
    }

    public static class Util
    {
        public static string GetMessageId()
        {
            return $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}{Guid.NewGuid().ToString("N")}";
        }

        public static ITransportSettings[] GetTransportSettings(this TransportProtocol transportProtocol)
        {
            ITransportSettings[] settings;
            switch(transportProtocol)
            {
                case TransportProtocol.Amqp:
                    settings = new ITransportSettings[]{ new AmqpTransportSettings(TransportType.Amqp_Tcp_Only) };
                    break;
                case TransportProtocol.Mqtt:
                    settings = new ITransportSettings[]{ new MqttTransportSettings(TransportType.Mqtt_Tcp_Only) };
                    break;
                // case TransportProtocol.Http1:
                //     settings = new ITransportSettings[]{ new Http1TransportSettings() };
                //     break;
                default:
                    settings = null;
                    break;
            }

            return settings;
        }

        public static T GetRequiredValue<T>(JObject jobj, string key)
        {
            T ret;

            if (jobj.TryGetValue(key, out JToken value))
            {
                ret = value.Value<T>();
            }
            else
            {
                throw new Exception($"Property {key} dose not exist.");
            }

            return ret;
        }

        public static byte HexStringToByte(string val)
        {
            if (val.Length != 2)
            {
                throw new Exception($"Convert to byte failed. val:{val}");
            }

            return Convert.ToByte(val, 16);
        }

        public static byte[] HexStringToBytes(string val)
        {
            var hs = val.Split('-');
            var ret = new byte[hs.Length];

            for (int i = 0; i < hs.Length; i++)
            {
                ret[i] = HexStringToByte(hs[i]);
            }

            return ret;
        }
    }
}