using System.Collections.Generic;
using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class MqttModuleClient_Dispose
{
    [Fact(DisplayName = "No454_正常系：正常動作の確認_メソッドを呼び出す")]
    public void Case454()
    {
        var client = new MqttModuleClient("sas-token", "localhost", "device1");
        var inputNames = (List<string>)typeof(MqttModuleClient).GetField("inputNames", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(client)!;
        inputNames.Add("input1");
        inputNames.Add("input2");

        client.Dispose();

        Assert.Empty(inputNames);
    }
}
