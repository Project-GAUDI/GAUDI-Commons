using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class Util_GetTransportSettings
{
    [Fact(DisplayName = "No601_正常系：正常動作の確認_transportProtocolにTransportProtocol.Amqpを指定して、メソッドを呼び出す")]
    public void Case601()
    {
        var result = TransportProtocol.Amqp.GetTransportSettings();

        var setting = Assert.Single(result);
        var amqpSetting = Assert.IsType<AmqpTransportSettings>(setting);
        Assert.Equal(TransportType.Amqp_Tcp_Only, amqpSetting.GetTransportType());
    }

    [Fact(DisplayName = "No602_正常系：正常動作の確認_transportProtocolにTransportProtocol.Mqttを指定して、メソッドを呼び出す")]
    public void Case602()
    {
        var result = TransportProtocol.Mqtt.GetTransportSettings();

        var setting = Assert.Single(result);
        var mqttSetting = Assert.IsType<MqttTransportSettings>(setting);
        Assert.Equal(TransportType.Mqtt_Tcp_Only, mqttSetting.GetTransportType());
    }

    [Fact(DisplayName = "No603_異常系：異常動作の確認_transportProtocolに未定義の列挙値を指定して、メソッドを呼び出す")]
    public void Case603()
    {
        var result = ((TransportProtocol)999).GetTransportSettings();

        Assert.Null(result);
    }
}
