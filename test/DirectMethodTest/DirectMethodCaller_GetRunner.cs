using System;
using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethodCaller_GetRunner
{
    private readonly ITestOutputHelper _output;

    public DirectMethodCaller_GetRunner(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case034_引数 methodName にnullを指定する")]
    public void DirectMethod_Case034()
    {
        DirectMethodCaller runner = new();

        var getRunnerMethod = runner.GetType().GetMethod("GetRunner", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var ret = getRunnerMethod!.Invoke(runner, [null]) as IDirectMethodRunner;
        Assert.Null(ret);
    }

    [Fact(DisplayName = "DirectMethod Case035_引数 methodName にnullを指定する")]
    public void DirectMethod_Case035()
    {
        DirectMethodCaller runner = new();

        var getRunnerMethod = runner.GetType().GetMethod("GetRunner", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var ret = getRunnerMethod!.Invoke(runner, [""]) as IDirectMethodRunner;
        Assert.Null(ret);
    }

    [Fact(DisplayName = "DirectMethod Case036_引数 methodName にnullを指定する")]
    public void DirectMethod_Case036()
    {
        DirectMethodCaller runner = new();

        var getRunnerMethod = runner.GetType().GetMethod("GetRunner", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.Throws<Exception>(() =>
        {
            try
            {
                getRunnerMethod!.Invoke(runner, ["Test_Method"]);
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException!;
            }
        });
    }

    [Fact(DisplayName = "DirectMethod Case037_引数 methodName に prefixとしてDirectMethod_ を付与した際に存在するクラス名を含む 文字列を指定する")]
    public void DirectMethod_Case037()
    {
        DirectMethodCaller runner = new();

        var getRunnerMethod = runner.GetType().GetMethod("GetRunner", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var ret = getRunnerMethod!.Invoke(runner, ["SetLogLevel"]) as IDirectMethodRunner;
        Assert.NotNull(ret);
        Assert.IsType<DirectMethod_SetLogLevel>(ret);
    }

    [Fact(DisplayName = "DirectMethod Case045_引数 methodName に prefixとしてDirectMethod_ を付与した際に存在するクラス名を含む 文字列を指定する")]
    public void DirectMethod_Case045()
    {
        DirectMethodCaller runner = new();

        var getRunnerMethod = runner.GetType().GetMethod("GetRunner", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var ret = getRunnerMethod!.Invoke(runner, ["GetLogLevel"]) as IDirectMethodRunner;
        Assert.NotNull(ret);
        Assert.IsType<DirectMethod_GetLogLevel>(ret);
    }
}