using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class I2CCommandsMessage_SerializeJson
{
    [Fact(DisplayName = "No593_正常系：正常動作の確認_messageのCommandListに、Action、Address、Command、Data、Filter、Length、Intervalを設定したI2CCommandを追加して、メソッドを呼び出す")]
    public void Case593()
    {
        var message = new I2CCommandsMessage
        {
            CommandList = new List<I2CCommand>
            {
                new I2CCommand
                {
                    Action = "read",
                    Address = "0x10",
                    Command = "cmd1",
                    Data = "data1",
                    Filter = "filter1",
                    Length = 4,
                    Interval = 100,
                },
            },
        };

        var result = I2CCommandsMessage.SerializeJson(message);

        Assert.Equal(
            "{\"CommandList\":[{\"Action\":\"read\",\"Address\":\"0x10\",\"Command\":\"cmd1\",\"Data\":\"data1\",\"Filter\":\"filter1\",\"Interval\":100,\"Length\":4}]}",
            result);
    }

    [Fact(DisplayName = "No594_正常系：正常動作の確認_CommandListがnullのI2CCommandsMessageをmessageに指定して、メソッドを呼び出す")]
    public void Case594()
    {
        var message = new I2CCommandsMessage { CommandList = null };

        var result = I2CCommandsMessage.SerializeJson(message);

        Assert.Equal("{\"CommandList\":null}", result);
    }

    [Fact(DisplayName = "No595_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case595()
    {
        var result = I2CCommandsMessage.SerializeJson(null!);

        Assert.Null(result);
    }
}
