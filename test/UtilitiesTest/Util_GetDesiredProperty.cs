using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json.Linq;

namespace TICO.GAUDI.Commons.Test.UtilitiesTest;

[Collection(nameof(Util_GetDesiredProperty))]
public class Util_GetDesiredProperty
{
    private readonly ITestOutputHelper _output;

    public Util_GetDesiredProperty(ITestOutputHelper output)
    {
        _output = output;
    }

    enum LogLevel
    {
        TRACE = 0,
        DEBUG = 1,
        INFO = 2,
        WARN = 3,
        ERROR = 4
    }

    [Fact(DisplayName = "Util No015_DesiredProperty：正常値を取得(int)")]
    public void Util_Case015()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": 123}");
        string key = "key1";
        bool isRequired = true;

        // Act
        int result = Util.GetDesiredProperty<int>(jobj, key, isRequired);

        // Assert
        Assert.Equal(123, result);
    }

    [Fact(DisplayName = "Util No016_DesiredProperty：正常値を取得(string)")]
    public void Util_Case016()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": \"abc\"}");
        string key = "key1";
        bool isRequired = true;

        // Act
        string result = Util.GetDesiredProperty<string>(jobj, key, isRequired);

        // Assert
        Assert.Equal("abc", result);
    }
    
    [Fact(DisplayName = "Util No017_DesiredProperty：正常値を取得(enum)※小文字")]
    public void Util_Case017()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": \"warn\"}");
        string key = "key1";
        bool isRequired = true;

        // Act
        LogLevel result = Util.GetDesiredProperty<LogLevel>(jobj, key, isRequired);

        // Assert
        Assert.Equal(LogLevel.WARN, result);
    }

    [Fact(DisplayName = "Util No018_DesiredProperty：正常値を取得(enum)※大文字")]
    public void Util_Case018()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": \"WARN\"}");
        string key = "key1";
        bool isRequired = true;

        // Act
        LogLevel result = Util.GetDesiredProperty<LogLevel>(jobj, key, isRequired);

        // Assert
        Assert.Equal(LogLevel.WARN, result);
    }

    [Fact(DisplayName = "Util No019_DesiredProperty：正常値を取得(enum)※数値")]
    public void Util_Case019()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": 4}");
        string key = "key1";
        bool isRequired = true;

        // Act
        LogLevel result = Util.GetDesiredProperty<LogLevel>(jobj, key, isRequired);

        // Assert
        Assert.Equal(LogLevel.ERROR, result);
    }

    [Fact(DisplayName = "Util No020_DesiredProperty：正常値を取得(bool)")]
    public void Util_Case020()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": true}");
        string key = "key1";
        bool isRequired = true;

        // Act
        bool result = Util.GetDesiredProperty<bool>(jobj, key, isRequired);

        // Assert
        Assert.True(result);
    }

    [Fact(DisplayName = "Util No021_DesiredProperty：正常値を取得(float)")]
    public void Util_Case021()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": 1.23}");
        string key = "key1";
        bool isRequired = true;

        // Act
        float result = Util.GetDesiredProperty<float>(jobj, key, isRequired);

        // Assert
        Assert.Equal(1.23f, result);
    }

    [Fact(DisplayName = "Util No022_DesiredProperty：正常値を取得(JObject)")]
    public void Util_Case022()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": {\"subkey1\": 123, \"subkey2\": \"test\"}}");
        string key = "key1";
        bool isRequired = true;

        // Act
        var result = Util.GetDesiredProperty<JObject>(jobj, key, isRequired);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<JObject>(result);
        Assert.Equal(123, ((JObject)result)["subkey1"]);
        Assert.Equal("test", ((JObject)result)["subkey2"]);
    }

    [Fact(DisplayName = "Util No023_DesiredProperty：正常値を取得(array)")]
    public void Util_Case023()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": [1, 2, 3]}");
        string key = "key1";
        bool isRequired = true;

        // Act
        var result = Util.GetDesiredProperty<JArray>(jobj, key, isRequired);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<JArray>(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0]);
        Assert.Equal(2, result[1]);
        Assert.Equal(3, result[2]);
    }

    [Fact(DisplayName = "Util No024_DesiredProperty：type違反（int）")]
    public void Util_Case024()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": \"abc\"}");
        string key = "key1";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<int>(jobj, key, isRequired));
    }

    [Fact(DisplayName = "Util No025_DesiredProperty：type違反（enum）文字")]
    public void Util_Case025()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": \"err\"}");
        string key = "key1";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<LogLevel>(jobj, key, isRequired));
    }

    [Fact(DisplayName = "Util No026_DesiredProperty：type違反（enum）数値")]
    public void Util_Case026()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": 6}");
        string key = "key1";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<LogLevel>(jobj, key, isRequired));
    }

    [Fact(DisplayName = "Util No027_DesiredProperty：type違反（bool）")]
    public void Util_Case027()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": \"test\"}");
        string key = "key1";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<bool>(jobj, key, isRequired));
    }

    [Fact(DisplayName = "Util No028_DesiredProperty：type違反（float）")]
    public void Util_Case028()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": \"test\"}");
        string key = "key1";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<float>(jobj, key, isRequired));
    }

    [Fact(DisplayName = "Util No029_DesiredProperty：type違反（array）")]
    public void Util_Case029()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": \"test\"}");
        string key = "key1";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<JArray>(jobj, key, isRequired));
    }

    [Fact(DisplayName = "Util No057_DesiredProperty：type違反（JObject）")]
    public void Util_Case057()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": \"test\"}");
        string key = "key1";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<JObject>(jobj, key, isRequired));
    }

    [Fact(DisplayName = "Util No030_DesiredProperty：condition違反（int）")]
    public void Util_Case030()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": 123}");
        string key = "key1";
        bool isRequired = true; 

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<int>(jobj, key, isRequired, condition: x => x >= 1 && x < 100));
    }
    
    [Fact(DisplayName = "Util No031_DesiredProperty：condition違反（float）")]
    public void Util_Case031()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": 1.23}");
        string key = "key1";
        bool isRequired = true;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<float>(jobj, key, isRequired, condition: x => x >= 0.0 && x < 1.0));
    }

    [Fact(DisplayName = "Util No032_DesiredProperty：condition違反なし（int）")]
    public void Util_Case032()
    {
        // Arrange
        var jobj = JObject.Parse("{\"key1\": -1}");
        string key = "key1";
        bool isRequired = true;

        // Act
        int result = Util.GetDesiredProperty<int>(jobj, key, isRequired, condition: x => x >= -1);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact(DisplayName = "Util No033_DesiredProperty：必須項目でキーが存在しない（Exception）")]
    public void Util_Case033()
    {
            // Arrange
            var jobj = JObject.Parse("{\"key1\": 123}");
            string key = "key2";
            bool isRequired = true;

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<int>(jobj, key, isRequired));
    }
    
    [Fact(DisplayName = "Util No034_DesiredProperty：必須でない項目でキーが存在しない（デフォルト値を取得）")]
    public void Util_Case034()
    {
            // Arrange
            var jobj = JObject.Parse("{\"key1\": 123}");
            string key = "key2";
            bool isRequired = false;
            int defaultValue = 456;

            // Act
            int result = Util.GetDesiredProperty<int>(jobj, key, isRequired, defaultValue);

            // Assert
            Assert.Equal(defaultValue, result);
    }
    
    [Fact(DisplayName = "Util No055_DesiredProperty：必須項目でキーはあるが値がnull（Exception）")]
    public void Util_Case055()
    {
            // Arrange
            var jobj = JObject.Parse("{\"key1\": null}");
            string key = "key1";
            bool isRequired = true;

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<string>(jobj, key, isRequired));
    }
    
    [Fact(DisplayName = "Util No056_DesiredProperty：必須項目でキーはあるが値が空白（Exception）")]
    public void Util_Case056()
    {
            // Arrange
            var jobj = JObject.Parse("{\"key1\": \"\"}");
            string key = "key1";
            bool isRequired = true;

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => Util.GetDesiredProperty<string>(jobj, key, isRequired));
    }
}
