using Xunit;
using Xunit.Abstractions;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;


namespace TICO.GAUDI.Commons.Test.UtilitiesTest;

[Collection(nameof(Util_GetTransportSettings))]
public class Util_GetTransportSettings
{
    private readonly ITestOutputHelper _output;

    public Util_GetTransportSettings(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "Util No002_AMQPプロトコルが選択された場合のチェック")]
    public void Util_Case002()
    {
        // Act
        var settings = TransportProtocol.Amqp.GetTransportSettings();

        // Assert
        Assert.NotNull(settings);
        Assert.Single(settings);
        Assert.IsType<AmqpTransportSettings>(settings[0]);

        var amqpSettings = settings[0] as AmqpTransportSettings;
        Assert.NotNull(amqpSettings); // オブジェクトが null でないことを確認
        Assert.Equal(TransportType.Amqp_Tcp_Only, amqpSettings!.GetTransportType()); // TransportType が Amqp_Tcp_Only であることを確認
    }

    [Fact(DisplayName = "Util No003_MQTTプロトコルが選択された場合のチェック")]
    public void Util_Case003()
    {
        // Act
        var settings = TransportProtocol.Mqtt.GetTransportSettings();

        // Assert
        Assert.NotNull(settings);
        Assert.Single(settings);
        Assert.IsType<MqttTransportSettings>(settings[0]);

        var mqttSettings = settings[0] as MqttTransportSettings;
        Assert.NotNull(mqttSettings); // オブジェクトが null でないことを確認
        Assert.Equal(TransportType.Mqtt_Tcp_Only, mqttSettings!.GetTransportType()); // TransportType が Mqtt_Tcp_Only であることを確認
    }

    [Fact(DisplayName = "Util No004_未サポートのプロトコルが選択された場合のチェック")]
    public void Util_Case004()
    {
        // Arrange
        var unsupportedProtocol = (TransportProtocol)999; // 未サポートのプロトコル値

        // Act
        var settings = unsupportedProtocol.GetTransportSettings();

        // Assert
        Assert.Null(settings); // 未サポートのプロトコルの場合は null が返されることを確認
    }
}
