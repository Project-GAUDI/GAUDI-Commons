using Microsoft.Azure.Devices.Client;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class MyExtensions_ToMessageResponse
{
    [Fact(DisplayName = "No742_正常系：正常動作の確認_thisResultにtrueを指定して、メソッドを呼び出す")]
    public void Case742()
    {
        var result = true.ToMessageResponse();

        Assert.Equal(MessageResponse.Completed, result);
    }

    [Fact(DisplayName = "No743_正常系：正常動作の確認_thisResultにfalseを指定して、メソッドを呼び出す")]
    public void Case743()
    {
        var result = false.ToMessageResponse();

        Assert.Equal(MessageResponse.None, result);
    }
}
