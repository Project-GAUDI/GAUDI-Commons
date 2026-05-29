using TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest.Stub;
using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest;

[Collection(nameof(MqttModuleClientTest))]
public class MqttModuleClient_Constructor : MqttModuleClientTesterBase
{
    [Fact(DisplayName = "MqttModuleClient No001_デバイスIDのみが引数で指定される")]
    public void MqttModuleClient_Case001()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_GATEWAYHOSTNAME, null);

        Assert.ThrowsAny<Exception>(() => new MqttModuleClient(_sas, device: _deviceId));
    }

    [Fact(DisplayName = "MqttModuleClient No002_ホスト名のみが引数で指定される")]
    public void MqttModuleClient_Case002()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, null);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, null);

        var client = new MqttModuleClient(_sas, hostName: _gatewayHostName);

        Assert.NotNull(client);
        Assert.IsType<MqttModuleClient>(client);

        MqttClientProxy.ReviewConstructorArguments(_gatewayHostName);
    }

    [Fact(DisplayName = "MqttModuleClient No003_ホスト名が引数で指定され、デバイスIDが環境変数で指定される")]
    public void MqttModuleClient_Case003()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, null);

        var client = new MqttModuleClient(_sas, hostName: _gatewayHostName);

        Assert.NotNull(client);
        Assert.IsType<MqttModuleClient>(client);

        MqttClientProxy.ReviewConstructorArguments(_gatewayHostName);

        Assert.Equal(_deviceId, GetDeviceId(client));
    }

    [Fact(DisplayName = "MqttModuleClient No004_ホスト名が環境変数で指定され、デバイスIDが引数で指定される")]
    public void MqttModuleClient_Case004()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_GATEWAYHOSTNAME, _gatewayHostName);

        var client = new MqttModuleClient(_sas, device: _deviceId);

        Assert.NotNull(client);
        Assert.IsType<MqttModuleClient>(client);

        MqttClientProxy.ReviewConstructorArguments(_gatewayHostName);

        Assert.Equal(_deviceId, GetDeviceId(client));
    }

    [Fact(DisplayName = "MqttModuleClient No005_ホスト名が引数で指定され、デバイスIDとモジュールIDが環境変数で指定される")]
    public void MqttModuleClient_Case005()
    {
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_DEVICEID, _deviceId);
        Environment.SetEnvironmentVariable(ENV_IOTEDGE_MODULEID, _moduleId);

        var client = new MqttModuleClient(_sas, hostName: _gatewayHostName);

        Assert.NotNull(client);
        Assert.IsType<MqttModuleClient>(client);

        MqttClientProxy.ReviewConstructorArguments(_gatewayHostName);

        Assert.Equal(_deviceId, GetDeviceId(client));
        Assert.Equal(_moduleId, GetModuleId(client));
    }

    [Fact(DisplayName = "MqttModuleClient No006_ホスト名とデバイスIDが引数で指定される")]
    public void MqttModuleClient_Case006()
    {
        var client = new MqttModuleClient(_sas, hostName: _gatewayHostName, device: _deviceId);

        Assert.NotNull(client);
        Assert.IsType<MqttModuleClient>(client);

        MqttClientProxy.ReviewConstructorArguments(_gatewayHostName);

        Assert.Equal(_deviceId, GetDeviceId(client));
    }
}
