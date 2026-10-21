using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SysRuntimeJsonSerializerTraceCollection))]
public sealed class SysRuntimeJsonSerializer_Serialize
{
    [Fact(DisplayName = "No545_正常系：正常動作の確認_targetにTextが\"ABC\"のオブジェクトを指定して、メソッドを呼び出す")]
    public void Case545()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new SampleData { Text = "ABC" };

        var result = serializer.Serialize(target);

        Assert.Equal("{\"Text\":\"ABC\"}", result);
    }

    [Fact(DisplayName = "No546_正常系：正常動作の確認_targetにnullを指定して、メソッドを呼び出す")]
    public void Case546()
    {
        var serializer = new SysRuntimeJsonSerializer();

        var result = serializer.Serialize<SampleData>(null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No547_異常系：異常動作の確認_シリアライズ時に例外を発生させるオブジェクトをtargetに指定して、メソッドを呼び出す")]
    public void Case547()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new SelfReferencing();
        target.Self = target;

        var result = serializer.Serialize(target);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No557_正常系：正常動作の確認_targetにTextが\"ABC\"のオブジェクト、settingsにnullを指定して、メソッドを呼び出す")]
    public void Case557()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new SampleData { Text = "ABC" };

        var result = serializer.Serialize(target, null!);

        Assert.Equal("{\"Text\":\"ABC\"}", result);
    }

    [Fact(DisplayName = "No558_正常系：正常動作の確認_targetに複数のプロパティを持つオブジェクト、settingsのFormattingにIndentedを指定して、メソッドを呼び出す")]
    public void Case558()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new MultiPropertyData { Text = "ABC", Number = 123 };
        var settings = new JsonSerializerSettings { Formatting = JsonFormatting.Indented };

        var result = serializer.Serialize(target, settings);

        var expected = SysRuntimeJsonSerializerTestHelper.BuildIndentedJson(target);
        Assert.Equal(expected, result);
        Assert.Contains("\n", result);
    }

    [Fact(DisplayName = "No559_正常系：正常動作の確認_TraceListenerを登録し、settingsのFormattingのみにIndentedを指定して、メソッドを呼び出す")]
    public void Case559()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new MultiPropertyData { Text = "ABC", Number = 123 };
        var settings = new JsonSerializerSettings { Formatting = JsonFormatting.Indented };

        using var trace = new SysRuntimeJsonSerializerTestHelper.TraceCapture();
        var result = serializer.Serialize(target, settings);

        Assert.NotNull(result);
        Assert.Empty(trace.Messages);
    }

    [Fact(DisplayName = "No560_正常系：正常動作の確認_targetにnull、settingsに任意の設定を指定して、メソッドを呼び出す")]
    public void Case560()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var settings = new JsonSerializerSettings { Formatting = JsonFormatting.Indented };

        var result = serializer.Serialize<SampleData>(null!, settings);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No561_異常系：異常動作の確認_シリアライズ時に例外を発生させるオブジェクトをtargetに指定して、メソッドを呼び出す")]
    public void Case561()
    {
        var serializer = new SysRuntimeJsonSerializer();
        var target = new SelfReferencing();
        target.Self = target;
        var settings = new JsonSerializerSettings();

        var result = serializer.Serialize(target, settings);

        Assert.Null(result);
    }

    public sealed class SampleData
    {
        public string Text { get; set; } = null!;
    }

    public sealed class MultiPropertyData
    {
        public string Text { get; set; } = null!;
        public int Number { get; set; }
    }

    public sealed class SelfReferencing
    {
        public SelfReferencing? Self { get; set; }
    }
}
