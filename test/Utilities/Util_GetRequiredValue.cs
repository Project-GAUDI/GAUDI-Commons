using Newtonsoft.Json.Linq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class Util_GetRequiredValue
{
    [Fact(DisplayName = "No604_正常系：正常動作の確認_jobjに{\"Value\":123}、keyに\"Value\"を指定し、Tをintとしてメソッドを呼び出す")]
    public void Case604()
    {
        var jobj = JObject.Parse("{\"Value\":123}");

        var result = Util.GetRequiredValue<int>(jobj, "Value");

        Assert.Equal(123, result);
    }

    [Fact(DisplayName = "No605_異常系：異常動作の確認_jobjに{\"Value\":123}、keyに\"Unknown\"を指定して、メソッドを呼び出す")]
    public void Case605()
    {
        var jobj = JObject.Parse("{\"Value\":123}");

        Assert.Throws<Exception>(() => Util.GetRequiredValue<int>(jobj, "Unknown"));
    }

    [Fact(DisplayName = "No606_異常系：異常動作の確認_jobjに{\"Value\":\"ABC\"}、keyに\"Value\"を指定し、Tをintとしてメソッドを呼び出す")]
    public void Case606()
    {
        var jobj = JObject.Parse("{\"Value\":\"ABC\"}");

        Assert.Throws<FormatException>(() => Util.GetRequiredValue<int>(jobj, "Value"));
    }

    [Fact(DisplayName = "No607_異常系：異常動作の確認_jobjにnull、keyに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case607()
    {
        Assert.Throws<NullReferenceException>(() => Util.GetRequiredValue<int>(null!, "Value"));
    }
}
