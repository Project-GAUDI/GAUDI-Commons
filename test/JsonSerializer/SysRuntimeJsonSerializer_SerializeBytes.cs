using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SysRuntimeJsonSerializerTraceCollection))]
public sealed class SysRuntimeJsonSerializer_SerializeBytes
{
    [Fact(DisplayName = "No548_正常系：正常動作の確認_targetにTextが\"ABC\"のオブジェクトを指定して、メソッドを呼び出す")]
    public void Case548()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new SampleData { Text = "ABC" };

        var result = serializer.SerializeBytes(target);

        Assert.Equal(Encoding.UTF8.GetBytes("{\"Text\":\"ABC\"}"), result);
    }

    [Fact(DisplayName = "No549_正常系：正常動作の確認_targetにnullを指定して、メソッドを呼び出す")]
    public void Case549()
    {
        var serializer = new SysRuntimeJsonSerializer();

        var result = serializer.SerializeBytes<SampleData>(null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No550_異常系：異常動作の確認_シリアライズ時に例外を発生させるオブジェクトをtargetに指定して、メソッドを呼び出す")]
    public void Case550()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new SelfReferencing();
        target.Self = target;

        var result = serializer.SerializeBytes(target);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No562_正常系：正常動作の確認_targetにTextが\"ABC\"のオブジェクト、settingsにnullを指定して、メソッドを呼び出す")]
    public void Case562()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new SampleData { Text = "ABC" };

        var result = serializer.SerializeBytes(target, null!);

        Assert.Equal(Encoding.UTF8.GetBytes("{\"Text\":\"ABC\"}"), result);
    }

    [Fact(DisplayName = "No563_正常系：正常動作の確認_targetに複数のプロパティを持つオブジェクト、settingsのFormattingにDefaultを指定して、メソッドを呼び出す")]
    public void Case563()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new MultiPropertyData { Text = "ABC", Number = 123 };
        var settings = new JsonSerializerSettings { Formatting = JsonFormatting.None };

        var result = serializer.SerializeBytes(target, settings);

        Assert.Equal(serializer.SerializeBytes(target), result);
        var json = Encoding.UTF8.GetString(result);
        Assert.DoesNotContain("\n", json);
    }

    [Fact(DisplayName = "No564_正常系：正常動作の確認_targetに複数のプロパティを持つオブジェクト、settingsのFormattingにIndentedを指定して、メソッドを呼び出す")]
    public void Case564()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new MultiPropertyData { Text = "ABC", Number = 123 };
        var settings = new JsonSerializerSettings { Formatting = JsonFormatting.Indented };

        var result = serializer.SerializeBytes(target, settings);

        var expected = Encoding.UTF8.GetBytes(SysRuntimeJsonSerializerTestHelper.BuildIndentedJson(target));
        Assert.Equal(expected, result);
        Assert.Contains("\n", Encoding.UTF8.GetString(result));
    }

    [Fact(DisplayName = "No565_正常系：正常動作の確認_TraceListenerを登録し、全項目がデフォルト値のsettingsを指定して、メソッドを呼び出す")]
    public void Case565()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new SampleData { Text = "ABC" };
        var settings = new JsonSerializerSettings();

        using var trace = new SysRuntimeJsonSerializerTestHelper.TraceCapture();
        var result = serializer.SerializeBytes(target, settings);

        Assert.NotNull(result);
        Assert.Empty(trace.Messages);
    }

    [Fact(DisplayName = "No566_正常系：正常動作の確認_TraceListenerを登録し、すべての非対応設定にデフォルト値以外を指定して、メソッドを呼び出す")]
    public void Case566()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new SampleData { Text = "ABC" };
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = JsonNullValueHandling.Ignore,
            DefaultValueHandling = JsonDefaultValueHandling.Ignore,
            ReferenceLoopHandling = JsonReferenceLoopHandling.Ignore,
            DateParseHandling = JsonDateParseHandling.None,
            DateFormatHandling = JsonDateFormatHandling.MicrosoftDateFormat,
            StringEscapeHandling = JsonStringEscapeHandling.EscapeHtml,
            ExcludeEmptyCollections = true,
            TypeNameHandling = JsonTypeNameHandling.All,
        };

        using var trace = new SysRuntimeJsonSerializerTestHelper.TraceCapture();
        var result = serializer.SerializeBytes(target, settings);

        Assert.NotNull(result);
        var message = string.Join(" ", trace.Messages);
        Assert.Contains("NullValueHandling=Ignore", message);
        Assert.Contains("DefaultValueHandling=Ignore", message);
        Assert.Contains("ReferenceLoopHandling=Ignore", message);
        Assert.Contains("DateParseHandling=None", message);
        Assert.Contains("DateFormatHandling=MicrosoftDateFormat", message);
        Assert.Contains("StringEscapeHandling=EscapeHtml", message);
        Assert.Contains("ExcludeEmptyCollections=True", message);
        Assert.Contains("TypeNameHandling=All", message);
    }

    [Fact(DisplayName = "No567_正常系：正常動作の確認_targetにnull、settingsに任意の設定を指定して、メソッドを呼び出す")]
    public void Case567()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var settings = new JsonSerializerSettings { Formatting = JsonFormatting.Indented };

        var result = serializer.SerializeBytes<SampleData>(null!, settings);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No568_異常系：異常動作の確認_シリアライズ時に例外を発生させるオブジェクトをtargetに指定して、メソッドを呼び出す")]
    public void Case568()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new SelfReferencing();
        target.Self = target;
        var settings = new JsonSerializerSettings();

        var result = serializer.SerializeBytes(target, settings);

        Assert.Null(result);
    }

    public sealed class MultiPropertyData
    {
        public string Text { get; set; } = null!;
        public int Number { get; set; }
    }

    public sealed class SampleData
    {
        public string Text { get; set; } = null!;
    }

    public sealed class SelfReferencing
    {
        public SelfReferencing? Self { get; set; }
    }
}
