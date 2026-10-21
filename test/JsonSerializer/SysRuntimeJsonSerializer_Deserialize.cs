using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SysRuntimeJsonSerializerTraceCollection))]
public sealed class SysRuntimeJsonSerializer_Deserialize
{
    [Fact(DisplayName = "No551_正常系：正常動作の確認_jsonStringに{\"Text\":\"ABC\"}を指定し、対応するクラスをTargetTypeとしてメソッドを呼び出す")]
    public void Case551()
    {
        var serializer = new SysRuntimeJsonSerializer();

        var result = serializer.Deserialize<SampleData>("{\"Text\":\"ABC\"}");

        Assert.NotNull(result);
        Assert.Equal("ABC", result.Text);
    }

    [Fact(DisplayName = "No552_異常系：異常動作の確認_jsonStringにnullを指定して、メソッドを呼び出す")]
    public void Case552()
    {
        var serializer = new SysRuntimeJsonSerializer();

        Assert.Throws<ArgumentNullException>(() => serializer.Deserialize<SampleData>((string)null!));
    }

    [Fact(DisplayName = "No553_異常系：異常動作の確認_jsonStringに不正なJSON文字列を指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case553()
    {
        var serializer = new SysRuntimeJsonSerializer();

        var result = serializer.Deserialize<SampleData>("{ invalid json");

        Assert.Null(result);
    }

    [Fact(DisplayName = "No554_正常系：正常動作の確認_jsonBytesに{\"Text\":\"ABC\"}をUTF-8で符号化したバイト配列を指定し、対応するクラスをTargetTypeとしてメソッドを呼び出す")]
    public void Case554()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var jsonBytes = Encoding.UTF8.GetBytes("{\"Text\":\"ABC\"}");

        var result = serializer.Deserialize<SampleData>(jsonBytes);

        Assert.NotNull(result);
        Assert.Equal("ABC", result.Text);
    }

    [Fact(DisplayName = "No555_異常系：異常動作の確認_jsonBytesにnullを指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case555()
    {
        var serializer = new SysRuntimeJsonSerializer();

        var result = serializer.Deserialize<SampleData>((byte[])null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No556_異常系：異常動作の確認_jsonBytesに不正なJSON文字列をUTF-8で符号化したバイト配列を指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case556()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var jsonBytes = Encoding.UTF8.GetBytes("{ invalid json");

        var result = serializer.Deserialize<SampleData>(jsonBytes);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No569_正常系：正常動作の確認_jsonStringに{\"Text\":\"ABC\"}、settingsにnullを指定し、対応するクラスをTargetTypeとしてメソッドを呼び出す")]
    public void Case569()
    {
        var serializer = new SysRuntimeJsonSerializer();

        var result = serializer.Deserialize<SampleData>("{\"Text\":\"ABC\"}", null!);

        Assert.NotNull(result);
        Assert.Equal("ABC", result.Text);
    }

    [Fact(DisplayName = "No570_正常系：正常動作の確認_TraceListenerを登録し、jsonStringに{\"Text\":\"ABC\"}、settingsのNullValueHandlingにデフォルト値以外を指定して、メソッドを呼び出す")]
    public void Case570()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var settings = new JsonSerializerSettings { NullValueHandling = JsonNullValueHandling.Ignore };

        using var trace = new SysRuntimeJsonSerializerTestHelper.TraceCapture();
        var result = serializer.Deserialize<SampleData>("{\"Text\":\"ABC\"}", settings);

        Assert.NotNull(result);
        Assert.Equal("ABC", result.Text);
        var message = string.Join(" ", trace.Messages);
        Assert.Contains("NullValueHandling=Ignore", message);
    }

    [Fact(DisplayName = "No571_異常系：異常動作の確認_jsonStringにnull、settingsに任意の設定を指定して、メソッドを呼び出す")]
    public void Case571()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var settings = new JsonSerializerSettings { NullValueHandling = JsonNullValueHandling.Ignore };

        Assert.Throws<ArgumentNullException>(() => serializer.Deserialize<SampleData>((string)null!, settings));
    }

    [Fact(DisplayName = "No572_異常系：異常動作の確認_jsonStringに不正なJSON文字列、settingsに任意の設定を指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case572()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var settings = new JsonSerializerSettings { NullValueHandling = JsonNullValueHandling.Ignore };

        var result = serializer.Deserialize<SampleData>("{ invalid json", settings);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No573_正常系：正常動作の確認_jsonBytesに{\"Text\":\"ABC\"}をUTF-8で符号化したバイト配列、settingsにnullを指定し、対応するクラスをTargetTypeとしてメソッドを呼び出す")]
    public void Case573()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var jsonBytes = Encoding.UTF8.GetBytes("{\"Text\":\"ABC\"}");

        var result = serializer.Deserialize<SampleData>(jsonBytes, null!);

        Assert.NotNull(result);
        Assert.Equal("ABC", result.Text);
    }

    [Fact(DisplayName = "No574_正常系：正常動作の確認_TraceListenerを登録し、正常なJSONバイト配列と、NullValueHandlingにデフォルト値以外を指定したsettingsを使用して、メソッドを呼び出す")]
    public void Case574()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var jsonBytes = Encoding.UTF8.GetBytes("{\"Text\":\"ABC\"}");
        var settings = new JsonSerializerSettings { NullValueHandling = JsonNullValueHandling.Ignore };

        using var trace = new SysRuntimeJsonSerializerTestHelper.TraceCapture();
        var result = serializer.Deserialize<SampleData>(jsonBytes, settings);

        Assert.NotNull(result);
        Assert.Equal("ABC", result.Text);
        var message = string.Join(" ", trace.Messages);
        Assert.Contains("NullValueHandling=Ignore", message);
    }

    [Fact(DisplayName = "No575_異常系：異常動作の確認_jsonBytesにnull、settingsに任意の設定を指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case575()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var settings = new JsonSerializerSettings { NullValueHandling = JsonNullValueHandling.Ignore };

        var result = serializer.Deserialize<SampleData>((byte[])null!, settings);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No576_異常系：異常動作の確認_jsonBytesに不正なJSON文字列をUTF-8で符号化したバイト配列、settingsに任意の設定を指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case576()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var jsonBytes = Encoding.UTF8.GetBytes("{ invalid json");
        var settings = new JsonSerializerSettings { NullValueHandling = JsonNullValueHandling.Ignore };

        var result = serializer.Deserialize<SampleData>(jsonBytes, settings);

        Assert.Null(result);
    }

    public sealed class SampleData
    {
        public string Text { get; set; } = null!;
    }
}
