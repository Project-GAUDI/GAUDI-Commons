using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class NewtonsoftJsonSerializer_SerializeBytes
{
    [Fact(DisplayName = "No518_正常系：正常動作の確認_targetに文字列プロパティが\"ABC\"のオブジェクトを指定して、メソッドを呼び出す")]
    public void Case518()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new TextOnly { Text = "ABC" };

        var result = serializer.SerializeBytes(target);

        Assert.Equal(Encoding.UTF8.GetBytes("{\"Text\":\"ABC\"}"), result);
    }

    [Fact(DisplayName = "No519_正常系：正常動作の確認_targetにnullを指定して、メソッドを呼び出す")]
    public void Case519()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var result = serializer.SerializeBytes<TextOnly>(null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No520_異常系：異常動作の確認_自己参照するプロパティを持つオブジェクトをtargetに指定して、メソッドを呼び出す")]
    public void Case520()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new SelfReferencing();
        target.Self = target;

        var result = serializer.SerializeBytes(target);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No533_正常系：正常動作の確認_targetに複数のプロパティを持つオブジェクト、settingsのFormattingにIndentedを指定して、メソッドを呼び出す")]
    public void Case533()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new SampleData { Text = "ABC", Number = 123 };
        var settings = new JsonSerializerSettings { Formatting = JsonFormatting.Indented };

        var result = serializer.SerializeBytes(target, settings);

        var expectedJson = Newtonsoft.Json.JsonConvert.SerializeObject(target, new Newtonsoft.Json.JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.Default,
        });
        Assert.Equal(Encoding.UTF8.GetBytes(expectedJson), result);
    }

    [Fact(DisplayName = "No534_正常系：正常動作の確認_targetに文字列プロパティが\"ABC\"のオブジェクト、settingsにnullを指定して、メソッドを呼び出す")]
    public void Case534()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new TextOnly { Text = "ABC" };

        var result = serializer.SerializeBytes(target, null!);

        Assert.Equal(Encoding.UTF8.GetBytes("{\"Text\":\"ABC\"}"), result);
    }

    [Fact(DisplayName = "No535_正常系：正常動作の確認_targetにnull、settingsに任意の設定を指定して、メソッドを呼び出す")]
    public void Case535()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var settings = new JsonSerializerSettings { Formatting = JsonFormatting.Indented };

        var result = serializer.SerializeBytes<TextOnly>(null!, settings);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No536_異常系：異常動作の確認_自己参照するプロパティを持つオブジェクトをtargetに指定し、settingsのReferenceLoopHandlingにErrorを指定して、メソッドを呼び出す")]
    public void Case536()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var target = new SelfReferencing();
        target.Self = target;
        var settings = new JsonSerializerSettings { ReferenceLoopHandling = JsonReferenceLoopHandling.Error };

        var result = serializer.SerializeBytes(target, settings);

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

    private sealed class SelfReferencing
    {
        public SelfReferencing? Self { get; set; }
    }
}
