using System.Text;
using AzureMessage = Microsoft.Azure.Devices.Client.Message;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_Constructor
{
    [Fact(DisplayName = "No316_正常系：正常動作の確認_引数なしでコンストラクタを呼び出す")]
    public void Case316()
    {
        var message = new IotMessage();

        Assert.NotNull(message);
        Assert.Equal("application/json", message.GetContentType());
        Assert.Equal("utf-8", message.GetContentEncoding());
        Assert.Equal(0, message.GetBodyLength());
    }

    [Fact(DisplayName = "No317_正常系：正常動作の確認_byteArrayに任意のバイト配列を指定して、コンストラクタを呼び出す")]
    public void Case317()
    {
        var bytes = new byte[] { 0x01, 0x02, 0x03 };

        var message = new IotMessage(bytes);

        Assert.NotNull(message);
        Assert.Equal(bytes, message.GetBytes());
        Assert.Equal("application/json", message.GetContentType());
        Assert.Equal("utf-8", message.GetContentEncoding());
    }

    [Fact(DisplayName = "No318_異常系：異常動作の確認_byteArrayにnullを指定して、コンストラクタを呼び出す")]
    public void Case318()
    {
        Assert.Throws<ArgumentNullException>(() => new IotMessage((byte[])null!));
    }

    [Fact(DisplayName = "No319_正常系：正常動作の確認_messageStringに任意の文字列を指定して、コンストラクタを呼び出す")]
    public void Case319()
    {
        const string body = "message";

        var message = new IotMessage(body);

        Assert.NotNull(message);
        Assert.Equal(body, message.GetBodyString());
    }

    [Fact(DisplayName = "No320_異常系：異常動作の確認_messageStringにnullを指定して、コンストラクタを呼び出す")]
    public void Case320()
    {
        Assert.Throws<ArgumentNullException>(() => new IotMessage((string)null!));
    }

    [Fact(DisplayName = "No321_正常系：正常動作の確認_streamに読み取り可能なStreamを指定して、コンストラクタを呼び出す")]
    public void Case321()
    {
        var bytes = Encoding.UTF8.GetBytes("message");
        using var stream = new MemoryStream(bytes);

        var message = new IotMessage(stream);
        var bodyStream = message.GetBodyStream();

        Assert.NotNull(message);
        Assert.Equal(0, bodyStream.Position);
        Assert.Equal(bytes, ReadAllBytes(bodyStream));
    }

    [Fact(DisplayName = "No322_異常系：異常動作の確認_streamにnullを指定して、コンストラクタを呼び出す")]
    public void Case322()
    {
        var message = new IotMessage((Stream)null!);

        Assert.NotNull(message);
        Assert.Same(Stream.Null, message.GetBodyStream());
    }

    [Fact(DisplayName = "No323_正常系：正常動作の確認_orgMessageに有効なMessageインスタンスを指定して、コンストラクタを呼び出す")]
    public void Case323()
    {
        var orgMessage = new AzureMessage(Encoding.UTF8.GetBytes("message"));

        var message = new IotMessage(orgMessage);

        Assert.NotNull(message);
        Assert.Same(orgMessage, message.GetMessage());
    }

    [Fact(DisplayName = "No324_異常系：異常動作の確認_orgMessageにnullを指定して、コンストラクタを呼び出す")]
    public void Case324()
    {
        var message = new IotMessage((AzureMessage)null!);

        Assert.NotNull(message);
        Assert.Null(message.GetMessage());
        Assert.Null(message.GetContentType());
    }

    [Fact(DisplayName = "No325_正常系：正常動作の確認_orgMessageにプロパティ等を設定済みの有効なIotMessageインスタンスを指定して、コンストラクタを呼び出す")]
    public void Case325()
    {
        var bytes = Encoding.UTF8.GetBytes("message");
        var orgMessage = new IotMessage(bytes);
        orgMessage.SetProperty("key", "value");
        orgMessage.SetMessageId("message-id");
        orgMessage.SetContentType("text/plain");
        orgMessage.SetContentEncoding("utf-16");

        var message = new IotMessage(orgMessage);

        Assert.NotNull(message);
        Assert.Equal(orgMessage.GetProperties(), message.GetProperties());
        Assert.Equal(orgMessage.GetMessageId(), message.GetMessageId());
        Assert.Equal(orgMessage.GetContentType(), message.GetContentType());
        Assert.Equal(orgMessage.GetContentEncoding(), message.GetContentEncoding());
        Assert.Equal(ReadAllBytes(orgMessage.GetBodyStream()), ReadAllBytes(message.GetBodyStream()));
    }

    [Fact(DisplayName = "No326_異常系：異常動作の確認_orgMessageにnullを指定して、コンストラクタを呼び出す")]
    public void Case326()
    {
        Assert.Throws<NullReferenceException>(() => new IotMessage((IotMessage)null!));
    }

    private static byte[] ReadAllBytes(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}