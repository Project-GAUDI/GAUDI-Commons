using System.Reflection;
using AzureMessage = Microsoft.Azure.Devices.Client.Message;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_GetConnectionModuleId
{
    [Fact(DisplayName = "No380_正常系：正常動作の確認_messageのConnectionModuleIdに値が設定されている状態で、メソッドを呼び出す")]
    public void Case380()
    {
        var azureMessage = new AzureMessage();
        SetInternalProperty(azureMessage, "ConnectionModuleId", "module-id");
        var message = new IotMessage(azureMessage);

        var result = message.GetConnectionModuleId();

        Assert.Equal("module-id", result);
    }

    [Fact(DisplayName = "No381_異常系：異常動作の確認_message=nullの状態で、メソッドを呼び出す")]
    public void Case381()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.GetConnectionModuleId();

        Assert.Null(result);
    }

    private static void SetInternalProperty(AzureMessage message, string propertyName, string value)
    {
        typeof(AzureMessage).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(message, value);
    }
}