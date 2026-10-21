using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class NewtonsoftJsonSerializer_Serialize
{
    [Fact(DisplayName = "No514_正常系：正常動作の確認_targetに文字列プロパティが\"ABC\"、数値プロパティが123のオブジェクトを指定して、メソッドを呼び出す")]
    public void Case514()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new SampleData { Text = "ABC", Number = 123 };

        var result = serializer.Serialize(target);

        Assert.Equal("{\"Text\":\"ABC\",\"Number\":123}", result);
    }

    [Fact(DisplayName = "No515_正常系：正常動作の確認_targetにダブルクォート、バックスラッシュ、スラッシュを含む文字列を指定して、メソッドを呼び出す")]
    public void Case515()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = "He said \"Hi\" and used \\ backslash and / slash";

        var result = serializer.Serialize(target);

        Assert.NotNull(result);
        Assert.Contains("\\\"Hi\\\"", result);
        Assert.Contains("\\\\", result);
        Assert.Contains("/ slash", result);
        Assert.DoesNotContain("\\/", result);
    }

    [Fact(DisplayName = "No516_正常系：正常動作の確認_targetにnullを指定して、メソッドを呼び出す")]
    public void Case516()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var result = serializer.Serialize<SampleData>(null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No517_異常系：異常動作の確認_自己参照するプロパティを持つオブジェクトをtargetに指定して、メソッドを呼び出す")]
    public void Case517()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new SelfReferencing();
        target.Self = target;

        var result = serializer.Serialize(target);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No527_正常系：正常動作の確認_targetに文字列プロパティが\"ABC\"のオブジェクト、settingsにnullを指定して、メソッドを呼び出す")]
    public void Case527()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new TextOnly { Text = "ABC" };

        var result = serializer.Serialize(target, null!);

        Assert.Equal("{\"Text\":\"ABC\"}", result);
    }

    [Fact(DisplayName = "No528_正常系：正常動作の確認_targetに複数のプロパティを持つオブジェクト、settingsのFormattingにIndentedを指定して、メソッドを呼び出す")]
    public void Case528()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new SampleData { Text = "ABC", Number = 123 };
        var settings = new JsonSerializerSettings { Formatting = JsonFormatting.Indented };

        var result = serializer.Serialize(target, settings);

        var expected = Newtonsoft.Json.JsonConvert.SerializeObject(target, new Newtonsoft.Json.JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.Default,
        });
        Assert.Equal(expected, result);
        Assert.Contains("\n", result);
    }

    [Fact(DisplayName = "No529_正常系：正常動作の確認_targetに空文字列および空コレクションを持つオブジェクト、settingsのExcludeEmptyCollectionsにfalseを指定して、メソッドを呼び出す")]
    public void Case529()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new MixedData { EmptyText = "", EmptyList = new List<string>(), Text = "Value" };
        var settings = new JsonSerializerSettings { ExcludeEmptyCollections = false };

        var result = serializer.Serialize(target, settings);

        Assert.Contains("\"EmptyText\":\"\"", result);
        Assert.Contains("\"EmptyList\":[]", result);
        Assert.Contains("\"Text\":\"Value\"", result);
    }

    [Fact(DisplayName = "No530_正常系：正常動作の確認_targetにnull、空文字列、空コレクションおよび値を持つプロパティを含むオブジェクト、settingsのExcludeEmptyCollectionsにtrueを指定して、メソッドを呼び出す")]
    public void Case530()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new MixedData { NullText = null, EmptyText = "", EmptyList = new List<string>(), Text = "Value" };
        var settings = new JsonSerializerSettings { ExcludeEmptyCollections = true };

        var result = serializer.Serialize(target, settings);

        Assert.Equal("{\"Text\":\"Value\"}", result);
    }

    [Fact(DisplayName = "No531_正常系：正常動作の確認_targetにnull、settingsに任意の設定を指定して、メソッドを呼び出す")]
    public void Case531()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var settings = new JsonSerializerSettings { Formatting = JsonFormatting.Indented };

        var result = serializer.Serialize<SampleData>(null!, settings);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No532_異常系：異常動作の確認_自己参照するプロパティを持つオブジェクトをtargetに指定し、settingsのReferenceLoopHandlingにErrorを指定して、メソッドを呼び出す")]
    public void Case532()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new SelfReferencing();
        target.Self = target;
        var settings = new JsonSerializerSettings { ReferenceLoopHandling = JsonReferenceLoopHandling.Error };

        var result = serializer.Serialize(target, settings);

        Assert.Null(result);
    }

    private sealed class SampleData
    {
        public string Text { get; set; } = null!;
        public int Number { get; set; }
    }

    private sealed class TextOnly
    {
        public string Text { get; set; } = null!;
    }

    private sealed class MixedData
    {
        public string? NullText { get; set; }
        public string EmptyText { get; set; } = "";
        public List<string> EmptyList { get; set; } = new();
        public string Text { get; set; } = null!;
    }

    private sealed class SelfReferencing
    {
        public SelfReferencing? Self { get; set; }
    }
}
