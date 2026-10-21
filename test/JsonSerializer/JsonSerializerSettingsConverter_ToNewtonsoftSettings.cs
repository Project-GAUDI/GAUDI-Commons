using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class JsonSerializerSettingsConverter_ToNewtonsoftSettings
{
    [Fact(DisplayName = "No510_正常系：正常動作の確認_sourceにそれぞれ有効な値を設定し、メソッドを呼び出す")]
    public void Case510()
    {
        var source = new JsonSerializerSettings
        {
            NullValueHandling = JsonNullValueHandling.Ignore,
            DefaultValueHandling = JsonDefaultValueHandling.IgnoreAndPopulate,
            Formatting = JsonFormatting.Indented,
            ReferenceLoopHandling = JsonReferenceLoopHandling.Ignore,
            DateParseHandling = JsonDateParseHandling.DateTimeOffset,
            DateFormatHandling = JsonDateFormatHandling.MicrosoftDateFormat,
            StringEscapeHandling = JsonStringEscapeHandling.EscapeHtml,
            TypeNameHandling = JsonTypeNameHandling.All,
        };

        var result = source.ToNewtonsoftSettings();

        Assert.Equal(Newtonsoft.Json.NullValueHandling.Ignore, result.NullValueHandling);
        Assert.Equal(Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate, result.DefaultValueHandling);
        Assert.Equal(Newtonsoft.Json.Formatting.Indented, result.Formatting);
        Assert.Equal(Newtonsoft.Json.ReferenceLoopHandling.Ignore, result.ReferenceLoopHandling);
        Assert.Equal(Newtonsoft.Json.DateParseHandling.DateTimeOffset, result.DateParseHandling);
        Assert.Equal(Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat, result.DateFormatHandling);
        Assert.Equal(Newtonsoft.Json.StringEscapeHandling.EscapeHtml, result.StringEscapeHandling);
        Assert.Equal(Newtonsoft.Json.TypeNameHandling.All, result.TypeNameHandling);
    }

    [Fact(DisplayName = "No511_正常系：正常動作の確認_sourceのExcludeEmptyCollectionsにfalseを設定して、メソッドを呼び出す")]
    public void Case511()
    {
        var source = new JsonSerializerSettings
        {
            ExcludeEmptyCollections = false,
        };

        var result = source.ToNewtonsoftSettings();

        Assert.Null(result.ContractResolver);
    }

    [Fact(DisplayName = "No512_正常系：正常動作の確認_sourceのExcludeEmptyCollectionsにtrueを設定して、メソッドを呼び出す")]
    public void Case512()
    {
        var source = new JsonSerializerSettings
        {
            ExcludeEmptyCollections = true,
        };

        var result = source.ToNewtonsoftSettings();

        Assert.IsType<ExcludeEmptyContractResolver>(result.ContractResolver);
    }

    [Fact(DisplayName = "No513_異常系：異常動作の確認_sourceにnullを指定して、メソッドを呼び出す")]
    public void Case513()
    {
        JsonSerializerSettings? source = null;

        Assert.Throws<NullReferenceException>(() => source!.ToNewtonsoftSettings());
    }
}
