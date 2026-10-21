using System.Text;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class I2CCommandsMessage_SerializeJsonBytes
{
    [Fact(DisplayName = "No596_正常系：正常動作の確認_messageのCommandListに、Action、Address、Command、Data、Filter、Length、Intervalを設定したI2CCommandを追加して、メソッドを呼び出す")]
    public void Case596()
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

        var result = I2CCommandsMessage.SerializeJsonBytes(message);

        var expected = Encoding.UTF8.GetBytes(
            "{\"CommandList\":[{\"Action\":\"read\",\"Address\":\"0x10\",\"Command\":\"cmd1\",\"Data\":\"data1\",\"Filter\":\"filter1\",\"Interval\":100,\"Length\":4}]}");
        Assert.Equal(expected, result);
    }

    [Fact(DisplayName = "No597_正常系：正常動作の確認_CommandListに空のリストを設定したI2CCommandsMessageをmessageに指定して、メソッドを呼び出す")]
    public void Case597()
    {
        var message = new I2CCommandsMessage { CommandList = new List<I2CCommand>() };

        var result = I2CCommandsMessage.SerializeJsonBytes(message);

        Assert.Equal(Encoding.UTF8.GetBytes("{\"CommandList\":[]}"), result);
    }

    [Fact(DisplayName = "No598_異常系：異常動作の確認_messageにnullを指定して、メソッドを呼び出す")]
    public void Case598()
    {
        var result = I2CCommandsMessage.SerializeJsonBytes(null!);

        Assert.Null(result);
    }
}
