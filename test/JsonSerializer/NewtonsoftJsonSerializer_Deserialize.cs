using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class NewtonsoftJsonSerializer_Deserialize
{
    [Fact(DisplayName = "No521_正常系：正常動作の確認_jsonStringに{\"Text\":\"ABC\",\"Number\":123}を指定し、対応するクラスをTargetTypeとしてメソッドを呼び出す")]
    public void Case521()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var result = serializer.Deserialize<SampleData>("{\"Text\":\"ABC\",\"Number\":123}");

        Assert.NotNull(result);
        Assert.Equal("ABC", result.Text);
        Assert.Equal(123, result.Number);
    }

    [Fact(DisplayName = "No522_正常系：正常動作の確認_jsonStringにnullを指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case522()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var result = serializer.Deserialize<SampleData>((string)null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No523_異常系：異常動作の確認_jsonStringに不正なJSON文字列を指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case523()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var result = serializer.Deserialize<SampleData>("{ invalid json");

        Assert.Null(result);
    }

    [Fact(DisplayName = "No524_正常系：正常動作の確認_jsonBytesに{\"Text\":\"ABC\",\"Number\":123}をUTF-8で符号化したバイト配列を指定し、対応するクラスをTargetTypeとしてメソッドを呼び出す")]
    public void Case524()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var jsonBytes = Encoding.UTF8.GetBytes("{\"Text\":\"ABC\",\"Number\":123}");

        var result = serializer.Deserialize<SampleData>(jsonBytes);

        Assert.NotNull(result);
        Assert.Equal("ABC", result.Text);
        Assert.Equal(123, result.Number);
    }

    [Fact(DisplayName = "No525_正常系：正常動作の確認_jsonBytesにnullを指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case525()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var result = serializer.Deserialize<SampleData>((byte[])null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No526_異常系：異常動作の確認_jsonBytesに不正なJSON文字列をUTF-8で符号化したバイト配列を指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case526()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var jsonBytes = Encoding.UTF8.GetBytes("{ invalid json");

        var result = serializer.Deserialize<SampleData>(jsonBytes);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No537_正常系：正常動作の確認_jsonStringに{\"Text\":\"ABC\",\"Number\":123}、settingsにnullを指定し、対応するクラスをTargetTypeとしてメソッドを呼び出す")]
    public void Case537()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var result = serializer.Deserialize<SampleData>("{\"Text\":\"ABC\",\"Number\":123}", null!);

        Assert.NotNull(result);
        Assert.Equal("ABC", result.Text);
        Assert.Equal(123, result.Number);
    }

    [Fact(DisplayName = "No538_正常系：正常動作の確認_jsonStringに日付形式の文字列を含むJSONを指定し、settingsのDateParseHandlingにNoneを指定して、メソッドを呼び出す")]
    public void Case538()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var settings = new JsonSerializerSettings { DateParseHandling = JsonDateParseHandling.None };

        var result = serializer.Deserialize<ObjectHolder>("{\"Value\":\"2024-01-01T00:00:00\"}", settings);

        Assert.NotNull(result);
        Assert.IsType<string>(result.Value);
        Assert.Equal("2024-01-01T00:00:00", result.Value);
    }

    [Fact(DisplayName = "No539_正常系：正常動作の確認_jsonStringにnull、settingsに任意の設定を指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case539()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var settings = new JsonSerializerSettings { DateParseHandling = JsonDateParseHandling.None };

        var result = serializer.Deserialize<SampleData>((string)null!, settings);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No540_異常系：異常動作の確認_jsonStringに不正なJSON文字列、settingsに任意の設定を指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case540()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var settings = new JsonSerializerSettings { DateParseHandling = JsonDateParseHandling.None };

        var result = serializer.Deserialize<SampleData>("{ invalid json", settings);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No541_正常系：正常動作の確認_jsonBytesに{\"Text\":\"ABC\",\"Number\":123}をUTF-8で符号化したバイト配列、settingsにnullを指定し、対応するクラスをTargetTypeとしてメソッドを呼び出す")]
    public void Case541()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var jsonBytes = Encoding.UTF8.GetBytes("{\"Text\":\"ABC\",\"Number\":123}");

        var result = serializer.Deserialize<SampleData>(jsonBytes, null!);

        Assert.NotNull(result);
        Assert.Equal("ABC", result.Text);
        Assert.Equal(123, result.Number);
    }

    [Fact(DisplayName = "No542_正常系：正常動作の確認_jsonBytesに日付形式の文字列を含むJSONをUTF-8で符号化したバイト配列を指定し、settingsのDateParseHandlingにNoneを指定して、メソッドを呼び出す")]
    public void Case542()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var jsonBytes = Encoding.UTF8.GetBytes("{\"Value\":\"2024-01-01T00:00:00\"}");
        var settings = new JsonSerializerSettings { DateParseHandling = JsonDateParseHandling.None };

        var result = serializer.Deserialize<ObjectHolder>(jsonBytes, settings);

        Assert.NotNull(result);
        Assert.IsType<string>(result.Value);
        Assert.Equal("2024-01-01T00:00:00", result.Value);
    }

    [Fact(DisplayName = "No543_正常系：正常動作の確認_jsonBytesにnull、settingsに任意の設定を指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case543()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var settings = new JsonSerializerSettings { DateParseHandling = JsonDateParseHandling.None };

        var result = serializer.Deserialize<SampleData>((byte[])null!, settings);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No544_異常系：異常動作の確認_jsonBytesに不正なJSON文字列をUTF-8で符号化したバイト配列、settingsに任意の設定を指定し、参照型をTargetTypeとしてメソッドを呼び出す")]
    public void Case544()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var jsonBytes = Encoding.UTF8.GetBytes("{ invalid json");
        var settings = new JsonSerializerSettings { DateParseHandling = JsonDateParseHandling.None };

        var result = serializer.Deserialize<SampleData>(jsonBytes, settings);

        Assert.Null(result);
    }

    private sealed class SampleData
    {
        public string Text { get; set; } = null!;
        public int Number { get; set; }
    }

    private sealed class ObjectHolder
    {
        public object Value { get; set; } = null!;
    }
}
