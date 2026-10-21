using System.Text;
using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class MyExtensions_ToDirectMethodRequest
{
    [Fact(DisplayName = "No744_正常系：正常動作の確認_thisMethodRequestに有効なName、DataAsJsonを持つMethodRequestを指定して、メソッドを呼び出す")]
    public void Case744()
    {
        var request = new MethodRequest("method1", Encoding.UTF8.GetBytes("{\"key\":\"value\"}"));

        var result = request.ToDirectMethodRequest();

        Assert.Equal("method1", result.MethodName);
        Assert.Equal("{\"key\":\"value\"}", result.RequestJson);
        Assert.Equal(request.DataAsJson, result.RequestJson);
    }

    [Fact(DisplayName = "No745_正常系：正常動作の確認_thisMethodRequestのデータ部が空（DataAsJsonが空文字）の状態で、メソッドを呼び出す")]
    public void Case745()
    {
        var request = new MethodRequest("method1", Array.Empty<byte>());

        var result = request.ToDirectMethodRequest();

        // SDKのMethodRequestはデータ部が空の場合、DataAsJsonとして空文字ではなくnullを返す
        Assert.Null(request.DataAsJson);
        Assert.Equal("method1", result.MethodName);
        Assert.Null(result.RequestJson);
    }

    [Fact(DisplayName = "No746_異常系：異常動作の確認_thisMethodRequestにnullを指定して、メソッドを呼び出す")]
    public void Case746()
    {
        MethodRequest? request = null;

        Assert.Throws<NullReferenceException>(() => request!.ToDirectMethodRequest());
    }
}
