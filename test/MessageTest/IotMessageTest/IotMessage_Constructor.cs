using System.Text;

namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_Constructor : IotMessageTesterBase
{
    [Fact(DisplayName = "Message No001_引数なし")]
    public void Message_Case001()
    {
        using var obj = new IotMessage();

        var message = GetMessageProperty(obj);
        Assert.IsType<Message>(message);

        using var ms = new MemoryStream();
        message!.BodyStream.CopyTo(ms);
        Assert.Empty(ms.ToArray());

        Assert.Equal(CONTENT_TYPE_APPLICATION_JSON, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_UTF8, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No002_引数が\"byteArray\"")]
    public void Message_Case002()
    {
        var bytes = new byte[50];
        new Random().NextBytes(bytes);

        using var obj = new IotMessage(bytes);

        var message = GetMessageProperty(obj);
        Assert.IsType<Message>(message);

        using var ms = new MemoryStream();
        message!.BodyStream.CopyTo(ms);
        Assert.Equal(bytes, ms.ToArray());

        var byteData = GetByteDataProperty(obj);
        Assert.Equal(bytes, byteData);

        Assert.Equal(CONTENT_TYPE_APPLICATION_JSON, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_UTF8, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No003_引数が\"messageString\"")]
    public void Message_Case003()
    {
        var uuid = Guid.NewGuid().ToString();
        var expectedBytes = Encoding.UTF8.GetBytes(uuid);

        using var obj = new IotMessage(uuid);

        var message = GetMessageProperty(obj);
        Assert.IsType<Message>(message);

        using var ms = new MemoryStream();
        message!.BodyStream.CopyTo(ms);
        Assert.Equal(expectedBytes, ms.ToArray());

        var byteData = GetByteDataProperty(obj);
        Assert.Equal(expectedBytes, byteData);

        Assert.Equal(CONTENT_TYPE_APPLICATION_JSON, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_UTF8, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No004_引数が\"stream\"")]
    public void Message_Case004()
    {
        var uuid = Guid.NewGuid().ToString();
        var expectedBytes = Encoding.UTF8.GetBytes(uuid);
        using var inputStream = new MemoryStream(expectedBytes);

        using var obj = new IotMessage(inputStream);

        var message = GetMessageProperty(obj);
        Assert.IsType<Message>(message);

        using var ms = new MemoryStream();
        message!.BodyStream.CopyTo(ms);
        Assert.Equal(expectedBytes, ms.ToArray());

        using var actualStream = GetBodyStreamProperty(obj);
        Assert.Equal(inputStream, actualStream);

        Assert.Equal(CONTENT_TYPE_APPLICATION_JSON, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_UTF8, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No005_引数が\"orgMessage(Message)\"")]
    public void Message_Case005()
    {
        var uuid = Guid.NewGuid().ToString();
        var original = new Message(Encoding.UTF8.GetBytes(uuid))
        {
            ContentType = CONTENT_TYPE_APPLICATION_XML,
            ContentEncoding = CONTENT_ENCODING_SHIFT_JIS
        };

        using var obj = new IotMessage(original);

        var message = GetMessageProperty(obj);
        Assert.IsType<Message>(message);

        Assert.Equal(original, message);
        Assert.Equal(CONTENT_TYPE_APPLICATION_XML, message!.ContentType);
        Assert.Equal(CONTENT_ENCODING_SHIFT_JIS, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No006_引数が\"orgMessage(IotMessage)\"")]
    public void Message_Case006()
    {
        var uuid = Guid.NewGuid().ToString();
        var expectedBytes = Encoding.UTF8.GetBytes(uuid);
        using var inputStream = new MemoryStream(expectedBytes);

        using var original = new IotMessage(inputStream);
        var originalMessage = GetMessageProperty(original);
        originalMessage!.ContentType = CONTENT_TYPE_APPLICATION_XML;
        originalMessage.ContentEncoding = CONTENT_ENCODING_SHIFT_JIS;

        using var cloned = new IotMessage(original);
        var clonedMessage = GetMessageProperty(cloned);
        Assert.IsType<Message>(clonedMessage);
        Assert.Equal(originalMessage!.BodyStream, clonedMessage!.BodyStream);

        var clonedBodyStream = GetBodyStreamProperty(cloned);
        Assert.Equal(originalMessage.BodyStream, clonedBodyStream);

        Assert.Equal(originalMessage.MessageId, clonedMessage.MessageId);
        Assert.Equal(CONTENT_TYPE_APPLICATION_XML, clonedMessage.ContentType);
        Assert.Equal(CONTENT_ENCODING_SHIFT_JIS, clonedMessage.ContentEncoding);
    }

}
