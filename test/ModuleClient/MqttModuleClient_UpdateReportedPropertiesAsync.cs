using Microsoft.Azure.Devices.Shared;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class MqttModuleClient_UpdateReportedPropertiesAsync
{
    [Fact(DisplayName = "No499_正常系：正常動作の確認_reportedPropertiesを指定して、メソッドを呼び出す")]
    public async Task Case499()
    {
        var client = NewMqttModuleClient();
        var reportedProperties = new TwinCollection();

        await Assert.ThrowsAsync<NotImplementedException>(
            () => client.UpdateReportedPropertiesAsync(reportedProperties));
    }

    [Fact(DisplayName = "No500_正常系：正常動作の確認_reportedProperties、transportTopicを指定して、メソッドを呼び出す")]
    public async Task Case500()
    {
        var client = NewMqttModuleClient();
        var reportedProperties = new TwinCollection();

        await Assert.ThrowsAsync<NotImplementedException>(
            () => client.UpdateReportedPropertiesAsync(reportedProperties, TransportTopic.Iothub));
    }

    private static MqttModuleClient NewMqttModuleClient()
    {
        return new MqttModuleClient("sas-token", "localhost", "device1");
    }
}
