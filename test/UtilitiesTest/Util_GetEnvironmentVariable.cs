using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json.Linq;

namespace TICO.GAUDI.Commons.Test.UtilitiesTest;

[Collection(nameof(Util_GetEnvironmentVariable))]
public class Util_GetEnvironmentVariable
{
    private readonly ITestOutputHelper _output;

    public Util_GetEnvironmentVariable(ITestOutputHelper output)
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

    [Fact(DisplayName = "Util No035_環境変数：正常値を取得(int)")]
    public void Util_Case035()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "123");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act
        int result = Util.GetEnvironmentVariable<int>(name, isRequired);

        // Assert
        Assert.Equal(123, result);
    }

    [Fact(DisplayName = "Util No036_環境変数：正常値を取得(string)")]
    public void Util_Case036()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "abc");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act
        string result = Util.GetEnvironmentVariable<string>(name, isRequired);

        // Assert
        Assert.Equal("abc", result);
    }

    [Fact(DisplayName = "Util No037_環境変数：正常値を取得(enum)※小文字")]
    public void Util_Case037()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "debug");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act
        LogLevel result = Util.GetEnvironmentVariable<LogLevel>(name, isRequired);

        // Assert
        Assert.Equal(LogLevel.DEBUG, result);
    }

    [Fact(DisplayName = "Util No038_環境変数：正常値を取得(enum)※大文字")]
    public void Util_Case038()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "DEBUG");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act
        LogLevel result = Util.GetEnvironmentVariable<LogLevel>(name, isRequired);

        // Assert
        Assert.Equal(LogLevel.DEBUG, result);
    }
    
    [Fact(DisplayName = "Util No039_環境変数：正常値を取得(enum)※数値")]
    public void Util_Case039()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "3");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;     

        // Act
        LogLevel result = Util.GetEnvironmentVariable<LogLevel>(name, isRequired);

        // Assert
        Assert.Equal(LogLevel.WARN, result);
    }

    [Fact(DisplayName = "Util No040_環境変数：正常値を取得(bool)※大文字")]
    public void Util_Case040()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "true");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act
        bool result = Util.GetEnvironmentVariable<bool>(name, isRequired);

        // Assert
        Assert.True(result);
    }

    [Fact(DisplayName = "Util No041_環境変数：正常値を取得(bool)※小文字")]
    public void Util_Case041()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "FALSE");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act
        bool result = Util.GetEnvironmentVariable<bool>(name, isRequired);

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "Util No042_環境変数：正常値を取得(float)")]
    public void Util_Case042()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "1.23");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act
        float result = Util.GetEnvironmentVariable<float>(name, isRequired);

        // Assert
        Assert.Equal(1.23f, result);
    }

    [Fact(DisplayName = "Util No043_環境変数：type違反（int）")]
    public void Util_Case043()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "test");
        string name = "TEST_ENV_VALUE";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetEnvironmentVariable<int>(name, isRequired));
    }

    [Fact(DisplayName = "Util No044_環境変数：type違反（enum）文字")]
    public void Util_Case044()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "test");
        string name = "TEST_ENV_VALUE";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetEnvironmentVariable<LogLevel>(name, isRequired));
    }
    
    [Fact(DisplayName = "Util No045_環境変数：type違反（enum）数値")]
    public void Util_Case045()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "5");
        string name = "TEST_ENV_VALUE";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetEnvironmentVariable<LogLevel>(name, isRequired));
    }

    [Fact(DisplayName = "Util No046_環境変数：type違反（bool）")]
    public void Util_Case046()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "test");
        string name = "TEST_ENV_VALUE";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetEnvironmentVariable<bool>(name, isRequired));
    }

    [Fact(DisplayName = "Util No047_環境変数：type違反（float）")]
    public void Util_Case047()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "1.2.3");
        string name = "TEST_ENV_VALUE";
        bool isRequired = false;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetEnvironmentVariable<float>(name, isRequired));
    }

    [Fact(DisplayName = "Util No048_環境変数：condition違反（int）")]
    public void Util_Case048()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "123");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetEnvironmentVariable<int>(name, isRequired, condition: x => x > 1 && x < 100));
    }

    [Fact(DisplayName = "Util No049_環境変数：condition違反（float）")]
    public void Util_Case049()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "1.23");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetEnvironmentVariable<float>(name, isRequired, condition: x => x > 0.0 && x < 1.0));
    }

    [Fact(DisplayName = "Util No050_環境変数：condition違反なし（int）")]
    public void Util_Case050()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "-1");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act
        int result = Util.GetEnvironmentVariable<int>(name, isRequired, condition: x => x >= -1);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact(DisplayName = "Util No051_環境変数：必須項目でキーが存在しない（Exception）")]
    public void Util_Case051()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "123");
        string name = "TEST_ENV_VALUE1";
        bool isRequired = true;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetEnvironmentVariable<int>(name, isRequired));
    }

    [Fact(DisplayName = "Util No052_環境変数：必須でない項目でキーが存在しない（デフォルト値を取得）")]
    public void Util_Case052()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "123");
        string name = "TEST_ENV_VALUE1";
        bool isRequired = false;
        int defaultValue = 456;

        // Act
        int result = Util.GetEnvironmentVariable<int>(name, isRequired, defaultValue);
        
        // Assert
        Assert.Equal(defaultValue, result);
    }
    
    [Fact(DisplayName = "Util No053_環境変数：必須項目でキーはあるが値がnull（Exception）")]
    public void Util_Case053()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", null);
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetEnvironmentVariable<string>(name, isRequired));
    }
    
    [Fact(DisplayName = "Util No054_環境変数：必須項目でキーはあるが値が空白（Exception）")]
    public void Util_Case054()
    {
        // Arrange
        System.Environment.SetEnvironmentVariable("TEST_ENV_VALUE", "");
        string name = "TEST_ENV_VALUE";
        bool isRequired = true;

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Util.GetEnvironmentVariable<string>(name, isRequired));
    }
}
