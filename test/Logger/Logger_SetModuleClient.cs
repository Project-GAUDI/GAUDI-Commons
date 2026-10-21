using System.Reflection;
using System.Text;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

[Collection(nameof(SharedLoggerState))]
public sealed class Logger_SetModuleClient
{
    [Fact(DisplayName = "No134_正常系：正常動作の確認_moduleClientにIModuleClientインスタンスを指定する")]
    public void Case134()
    {
        var logger = NewLogger();
        var client = new Mock<IModuleClient>();
        client.SetupGet(value => value.ConnectionStatus).Returns(IotConnectionStatus.Connected);
        logger.SetModuleClient(client.Object);
        Assert.Same(client.Object, logger.MyClient);
    }
    [Fact(DisplayName = "No135_正常系：正常動作の確認_moduleClientにnullを指定する")]
    public void Case135()
    {
        var logger = NewLogger();
        logger.SetModuleClient(null!);
        Assert.Null(logger.MyClient);
    }

    private static ILogger NewLogger()
    {
        var type = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
        type.GetField("outputLogLevel", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, ILogger.LogLevel.INFO);
        var logger = (ILogger)Activator.CreateInstance(type, new object[] { "SpecClass" })!;
        logger.SetOutputTextWriter(new StringWriter(new StringBuilder())); return logger;
    }
}