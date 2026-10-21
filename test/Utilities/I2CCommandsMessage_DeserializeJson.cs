using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class I2CCommandsMessage_DeserializeJson
{
    private const string ValidJson = "{\"CommandList\":[{\"Action\":\"read\",\"Address\":\"0x10\",\"Command\":\"cmd1\",\"Data\":\"data1\",\"Filter\":\"filter1\",\"Interval\":100,\"Length\":4}]}";

    [Fact(DisplayName = "No587_正常系：正常動作の確認_messageにCommandListの各要素のAction、Address、Command、Data、Filter、Length、Intervalを設定した正常なJSON文字列を指定して、メソッドを呼び出す")]
    public void Case587()
    {
        var result = I2CCommandsMessage.DeserializeJson(ValidJson);

        Assert.NotNull(result);
        var command = Assert.Single(result.CommandList);
        Assert.Equal("read", command.Action);
        Assert.Equal("0x10", command.Address);
        Assert.Equal("cmd1", command.Command);
        Assert.Equal("data1", command.Data);
        Assert.Equal("filter1", command.Filter);
        Assert.Equal(4, command.Length);
        Assert.Equal(100, command.Interval);
    }

    [Fact(DisplayName = "No588_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case588()
    {
        var result = I2CCommandsMessage.DeserializeJson((string)null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No589_異常系：異常動作の確認_messageに不正なJSON文字列を指定して、メソッドを呼び出す")]
    public void Case589()
    {
        var result = I2CCommandsMessage.DeserializeJson("{ invalid json");

        Assert.Null(result);
    }

    [Fact(DisplayName = "No590_正常系：正常動作の確認_messageにCommandListの各要素のAction、Address、Command、Data、Filter、Length、Intervalを設定したJSON文字列をUTF-8で符号化したバイト配列を指定して、メソッドを呼び出す")]
    public void Case590()
    {
        var bytes = Encoding.UTF8.GetBytes(ValidJson);

        var result = I2CCommandsMessage.DeserializeJson(bytes);

        Assert.NotNull(result);
        var command = Assert.Single(result.CommandList);
        Assert.Equal("read", command.Action);
        Assert.Equal("0x10", command.Address);
        Assert.Equal("cmd1", command.Command);
        Assert.Equal("data1", command.Data);
        Assert.Equal("filter1", command.Filter);
        Assert.Equal(4, command.Length);
        Assert.Equal(100, command.Interval);
    }

    [Fact(DisplayName = "No591_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case591()
    {
        var result = I2CCommandsMessage.DeserializeJson((byte[])null!);

        Assert.Null(result);
    }

    [Fact(DisplayName = "No592_異常系：異常動作の確認_messageに不正なJSON文字列をUTF-8で符号化したバイト配列を指定して、メソッドを呼び出す")]
    public void Case592()
    {
        var bytes = Encoding.UTF8.GetBytes("{ invalid json");

        var result = I2CCommandsMessage.DeserializeJson(bytes);

        Assert.Null(result);
    }
}
