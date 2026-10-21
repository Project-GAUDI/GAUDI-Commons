using System.Text;
using AzureMessage = Microsoft.Azure.Devices.Client.Message;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class IotMessage_GetBodyStream
{
    [Fact(DisplayName = "No334_正常系：正常動作の確認_bodyStreamが未設定、CanSeek=trueの状態で、メソッドを呼び出す")]
    public void Case334()
    {
        var bytes = Encoding.UTF8.GetBytes("message");
        var message = new IotMessage(new AzureMessage(bytes));

        var result = message.GetBodyStream();

        Assert.NotNull(result);
        Assert.True(result.CanSeek);
        Assert.Equal(0, result.Position);
        Assert.Equal(bytes, ReadAllBytes(result));
    }

    [Fact(DisplayName = "No335_正常系：正常動作の確認_bodyStreamが設定済みで、位置が先頭以外まで読み進められ、CanSeek=trueの状態で、メソッドを呼び出す")]
    public void Case335()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("message"));
        stream.ReadByte();
        var message = new IotMessage(stream);

        var result = message.GetBodyStream();

        Assert.Same(stream, result);
        Assert.Equal(0, result.Position);
    }

    [Fact(DisplayName = "No336_正常系：正常動作の確認_bodyStreamが設定済みで、位置が先頭以外まで読み進められ、CanSeek=falseの状態で、メソッドを呼び出す")]
    public void Case336()
    {
        var stream = new NonSeekableReadStream(Encoding.UTF8.GetBytes("message"));
        stream.ReadByte();
        var position = stream.Position;
        var message = new IotMessage(stream);

        var result = message.GetBodyStream();

        Assert.Same(stream, result);
        Assert.Equal(position, result.Position);
    }

    [Fact(DisplayName = "No337_異常系：異常動作の確認_message=null、bodyStream未設定の状態で、メソッドを呼び出す")]
    public void Case337()
    {
        var message = new IotMessage();
        message.Dispose();

        var result = message.GetBodyStream();

        Assert.Null(result);
    }

    private static byte[] ReadAllBytes(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    private sealed class NonSeekableReadStream : Stream
    {
        private readonly MemoryStream innerStream;

        public NonSeekableReadStream(byte[] bytes)
        {
            innerStream = new MemoryStream(bytes);
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => innerStream.Length;

        public override long Position
        {
            get => innerStream.Position;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}