using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class MyExtensions_IsReconnectableReason
{
    [Fact(DisplayName = "No753_正常系：正常動作の確認_thisReasonにBad_Credentialを指定して、メソッドを呼び出す")]
    public void Case753()
    {
        Assert.False(IotConnectionStatusChangeReason.Bad_Credential.IsReconnectableReason());
    }

    [Fact(DisplayName = "No754_正常系：正常動作の確認_thisReasonにClient_Closeを指定して、メソッドを呼び出す")]
    public void Case754()
    {
        Assert.False(IotConnectionStatusChangeReason.Client_Close.IsReconnectableReason());
    }

    [Fact(DisplayName = "No755_正常系：正常動作の確認_thisReasonにCommunication_Errorを指定して、メソッドを呼び出す")]
    public void Case755()
    {
        Assert.True(IotConnectionStatusChangeReason.Communication_Error.IsReconnectableReason());
    }

    [Fact(DisplayName = "No756_正常系：正常動作の確認_thisReasonにConnection_Okを指定して、メソッドを呼び出す")]
    public void Case756()
    {
        Assert.False(IotConnectionStatusChangeReason.Connection_Ok.IsReconnectableReason());
    }

    [Fact(DisplayName = "No757_正常系：正常動作の確認_thisReasonにDevice_Disabledを指定して、メソッドを呼び出す")]
    public void Case757()
    {
        Assert.False(IotConnectionStatusChangeReason.Device_Disabled.IsReconnectableReason());
    }

    [Fact(DisplayName = "No758_正常系：正常動作の確認_thisReasonにExpired_SAS_Tokenを指定して、メソッドを呼び出す")]
    public void Case758()
    {
        Assert.False(IotConnectionStatusChangeReason.Expired_SAS_Token.IsReconnectableReason());
    }

    [Fact(DisplayName = "No759_正常系：正常動作の確認_thisReasonにNo_Networkを指定して、メソッドを呼び出す")]
    public void Case759()
    {
        Assert.False(IotConnectionStatusChangeReason.No_Network.IsReconnectableReason());
    }

    [Fact(DisplayName = "No760_正常系：正常動作の確認_thisReasonにRetry_Expiredを指定して、メソッドを呼び出す")]
    public void Case760()
    {
        Assert.True(IotConnectionStatusChangeReason.Retry_Expired.IsReconnectableReason());
    }

    [Fact(DisplayName = "No761_正常系：正常動作の確認_thisReasonに列挙体未定義の値をキャストして指定し、メソッドを呼び出す")]
    public void Case761()
    {
        Assert.False(((IotConnectionStatusChangeReason)99).IsReconnectableReason());
    }
}
