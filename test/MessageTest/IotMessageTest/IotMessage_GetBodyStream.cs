namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_GetBodyStream : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No013_フィールド\"bodyStream\"がnull、かつ\"message\"がnull以外、かつ、\"message\"から取得したストリームがシーク可能")]
    public void Message_Case013()
    {
        var bytes = new byte[50];
        new Random().NextBytes(bytes);

        using var obj = new IotMessage(bytes);
        var message = GetMessageProperty(obj)!;
        var streamOnMessage = message.BodyStream;
        streamOnMessage.Position = 10;

        var result = obj.GetBodyStream();
        var bodyStream = GetBodyStreamProperty(obj);

        Assert.Equal(streamOnMessage, result);
        Assert.Equal(streamOnMessage, bodyStream);
        Assert.Equal(0, result.Position);
    }

    [Fact(DisplayName = "Message No014_フィールド\"bodyStream\"がnull、かつ\"message\"がnull以外、かつ、\"message\"から取得したストリームがシーク不可")]
    public void Message_Case014()
    {
        using var cannotSeekStream = Console.OpenStandardOutput();

        using var obj = new IotMessage(cannotSeekStream);
        SetBodyStreamProperty(obj, null);

        var result = obj.GetBodyStream();
        var bodyStream = GetBodyStreamProperty(obj);
        var originStream = GetMessageProperty(obj)!.BodyStream;

        Assert.Equal(originStream, result);
        Assert.Equal(originStream, bodyStream);
    }

    [Fact(DisplayName = "Message No015_フィールド\"bodyStream\"がnull以外、かつ、シーク可能")]
    public void Message_Case015()
    {
        var bytes = new byte[50];
        new Random().NextBytes(bytes);
        using var canSeekStream = new MemoryStream();
        canSeekStream.Position = 20;

        using var obj = new IotMessage(canSeekStream);
        SetBodyStreamProperty(obj, null);

        var result = obj.GetBodyStream();
        var bodyStream = GetBodyStreamProperty(obj);

        Assert.Equal(bodyStream, result);
        Assert.Equal(0, bodyStream!.Position);
    }

    [Fact(DisplayName = "Message No016_フィールド\"bodyStream\"がnull以外、かつ、シーク不可")]
    public void Message_Case016()
    {
        using var cannotSeekStream = Console.OpenStandardOutput();

        using var obj = new IotMessage(cannotSeekStream);

        var result = obj.GetBodyStream();
        var bodyStream = GetBodyStreamProperty(obj);

        Assert.Equal(bodyStream, result);
    }
}
