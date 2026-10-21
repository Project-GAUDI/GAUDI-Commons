using Newtonsoft.Json.Linq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class Util_GetDesiredProperty
{
    [Fact(DisplayName = "No618_正常系：正常動作の確認_propertiesに{\"Value\":123}、keyに\"Value\"、isRequiredにtrueを指定し、Tをintとしてメソッドを呼び出す")]
    public void Case618()
    {
        var properties = JObject.Parse("{\"Value\":123}");

        var result = Util.GetDesiredProperty<int>(properties, "Value", true);

        Assert.Equal(123, result);
    }

    [Fact(DisplayName = "No619_正常系：正常動作の確認_propertiesに空のJObject、keyに\"Value\"、isRequiredにfalse、defaultValueに10を指定して、メソッドを呼び出す")]
    public void Case619()
    {
        var properties = new JObject();

        var result = Util.GetDesiredProperty<int>(properties, "Value", false, 10);

        Assert.Equal(10, result);
    }

    [Fact(DisplayName = "No620_異常系：異常動作の確認_propertiesに空のJObject、keyに\"Value\"、isRequiredにtrueを指定して、メソッドを呼び出す")]
    public void Case620()
    {
        var properties = new JObject();

        Assert.Throws<ArgumentException>(() => Util.GetDesiredProperty<int>(properties, "Value", true));
    }

    [Fact(DisplayName = "No621_異常系：異常動作の確認_propertiesに{\"Value\":null}、keyに\"Value\"、isRequiredにtrueを指定して、メソッドを呼び出す")]
    public void Case621()
    {
        var properties = JObject.Parse("{\"Value\":null}");

        Assert.Throws<ArgumentException>(() => Util.GetDesiredProperty<string>(properties, "Value", true));
    }

    [Fact(DisplayName = "No622_異常系：異常動作の確認_propertiesに{\"Value\":\"   \"}、keyに\"Value\"、isRequiredにtrueを指定して、メソッドを呼び出す")]
    public void Case622()
    {
        var properties = JObject.Parse("{\"Value\":\"   \"}");

        Assert.Throws<ArgumentException>(() => Util.GetDesiredProperty<string>(properties, "Value", true));
    }

    [Fact(DisplayName = "No623_異常系：異常動作の確認_propertiesに{\"Value\":\"ABC\"}、keyに\"Value\"を指定し、Tをintとしてメソッドを呼び出す")]
    public void Case623()
    {
        var properties = JObject.Parse("{\"Value\":\"ABC\"}");

        Assert.Throws<ArgumentException>(() => Util.GetDesiredProperty<int>(properties, "Value", true));
    }

    [Fact(DisplayName = "No624_正常系：正常動作の確認_propertiesに{\"Protocol\":\"MQTT\"}、keyに\"Protocol\"を指定し、TをTransportProtocolとしてメソッドを呼び出す")]
    public void Case624()
    {
        var properties = JObject.Parse("{\"Protocol\":\"MQTT\"}");

        var result = Util.GetDesiredProperty<TransportProtocol>(properties, "Protocol", true);

        Assert.Equal(TransportProtocol.Mqtt, result);
    }

    [Fact(DisplayName = "No625_正常系：正常動作の確認_propertiesに{\"Protocol\":\"mqtt\"}、keyに\"Protocol\"を指定し、TをTransportProtocolとしてメソッドを呼び出す")]
    public void Case625()
    {
        var properties = JObject.Parse("{\"Protocol\":\"mqtt\"}");

        var result = Util.GetDesiredProperty<TransportProtocol>(properties, "Protocol", true);

        Assert.Equal(TransportProtocol.Mqtt, result);
    }

    [Fact(DisplayName = "No626_異常系：異常動作の確認_propertiesに{\"Protocol\":999}、keyに\"Protocol\"を指定し、TをTransportProtocolとしてメソッドを呼び出す")]
    public void Case626()
    {
        var properties = JObject.Parse("{\"Protocol\":999}");

        Assert.Throws<ArgumentException>(() => Util.GetDesiredProperty<TransportProtocol>(properties, "Protocol", true));
    }

    [Fact(DisplayName = "No627_正常系：正常動作の確認_propertiesに{\"Value\":1}、keyに\"Value\"、conditionにvalue=>value>0を指定して、メソッドを呼び出す")]
    public void Case627()
    {
        var properties = JObject.Parse("{\"Value\":1}");

        var result = Util.GetDesiredProperty<int>(properties, "Value", true, condition: value => value > 0);

        Assert.Equal(1, result);
    }

    [Fact(DisplayName = "No628_異常系：異常動作の確認_propertiesに{\"Value\":0}、keyに\"Value\"、conditionにvalue=>value>0を指定して、メソッドを呼び出す")]
    public void Case628()
    {
        var properties = JObject.Parse("{\"Value\":0}");

        Assert.Throws<ArgumentException>(() => Util.GetDesiredProperty<int>(properties, "Value", true, condition: value => value > 0));
    }

    [Fact(DisplayName = "No629_異常系：異常動作の確認_propertiesにnull、keyに任意の文字列を指定して、メソッドを呼び出す")]
    public void Case629()
    {
        Assert.Throws<NullReferenceException>(() => Util.GetDesiredProperty<int>(null!, "Value", true));
    }
}
