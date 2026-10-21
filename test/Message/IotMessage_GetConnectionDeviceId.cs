using System.Reflection;
using AzureMessage = Microsoft.Azure.Devices.Client.Message;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_GetConnectionDeviceId
{
    [Fact(DisplayName = "No378_正常系：正常動作の確認_messageのConnectionDeviceIdに値が設定されている状態で、メソッドを呼び出す")]
    public void Case378()
    {
        var azureMessage = new AzureMessage();
        SetInternalProperty(azureMessage, "ConnectionDeviceId", "device-id");
        var message = new IotMessage(azureMessage);

        var result = message.GetConnectionDeviceId();

        Assert.Equal("device-id", result);
    }

    [Fact(DisplayName = "No379_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case379()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.GetConnectionDeviceId();

        Assert.Null(result);
    }

    private static void SetInternalProperty(AzureMessage message, string propertyName, string value)
    {
        typeof(AzureMessage).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(message, value);
    }
}