namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public class IotMessage_setContentTypeAndEncording : IotMessageTesterBase
{
    private static void Invoke(IotMessage instance)
    {
        var type = typeof(IotMessage);
        var mi = type.GetMethod("setContentTypeAndEncoding", BindingFlags.Instance | BindingFlags.NonPublic);
        mi!.Invoke(instance, null);
    }

    [Fact(DisplayName = "Message No044_\"GetContentType\"の戻り値がnull、かつ、\"GetContentEncoding\"の戻り値がnull")]
    public void Message_Case044()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.ContentType = null;
        message.ContentEncoding = null;

        Invoke(obj);

        Assert.Equal(CONTENT_TYPE_APPLICATION_JSON, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_UTF8, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No045_\"GetContentType\"の戻り値がnull、かつ、\"GetContentEncoding\"の戻り値が空文字")]
    public void Message_Case045()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.ContentType = null;
        message.ContentEncoding = "";

        Invoke(obj);

        Assert.Equal(CONTENT_TYPE_APPLICATION_JSON, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_UTF8, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No046_\"GetContentType\"の戻り値がnull、かつ、\"GetContentEncoding\"の戻り値がnull/空文字以外")]
    public void Message_Case046()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.ContentType = null;
        message.ContentEncoding = CONTENT_ENCODING_SHIFT_JIS;

        Invoke(obj);

        Assert.Equal(CONTENT_TYPE_APPLICATION_JSON, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_SHIFT_JIS, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No047_\"GetContentType\"の戻り値が空文字、かつ、\"GetContentEncoding\"の戻り値がnull")]
    public void Message_Case047()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.ContentType = "";
        message.ContentEncoding = null;

        Invoke(obj);

        Assert.Equal(CONTENT_TYPE_APPLICATION_JSON, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_UTF8, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No048_\"GetContentType\"の戻り値が空文字、かつ、\"GetContentEncoding\"の戻り値が空文字")]
    public void Message_Case048()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.ContentType = "";
        message.ContentEncoding = "";

        Invoke(obj);

        Assert.Equal(CONTENT_TYPE_APPLICATION_JSON, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_UTF8, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No049_\"GetContentType\"の戻り値が空文字、かつ、\"GetContentEncoding\"の戻り値がnull/空文字以外")]
    public void Message_Case049()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.ContentType = "";
        message.ContentEncoding = CONTENT_ENCODING_SHIFT_JIS;

        Invoke(obj);

        Assert.Equal(CONTENT_TYPE_APPLICATION_JSON, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_SHIFT_JIS, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No050_\"GetContentType\"の戻り値がnull空文字以外、かつ、\"GetContentEncoding\"の戻り値がnull")]
    public void Message_Case050()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.ContentType = CONTENT_TYPE_APPLICATION_XML;
        message.ContentEncoding = null;

        Invoke(obj);

        Assert.Equal(CONTENT_TYPE_APPLICATION_XML, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_UTF8, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No051_\"GetContentType\"の戻り値がnull空文字以外、かつ、\"GetContentEncoding\"の戻り値が空文字")]
    public void Message_Case051()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.ContentType = CONTENT_TYPE_APPLICATION_XML;
        message.ContentEncoding = "";

        Invoke(obj);

        Assert.Equal(CONTENT_TYPE_APPLICATION_XML, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_UTF8, message.ContentEncoding);
    }

    [Fact(DisplayName = "Message No052_\"GetContentType\"の戻り値がnull空文字以外、かつ、\"GetContentEncoding\"の戻り値がnull/空文字以外")]
    public void Message_Case052()
    {
        using var obj = new IotMessage();
        var message = GetMessageProperty(obj);
        message!.ContentType = CONTENT_TYPE_APPLICATION_XML;
        message.ContentEncoding = CONTENT_ENCODING_SHIFT_JIS;

        Invoke(obj);

        Assert.Equal(CONTENT_TYPE_APPLICATION_XML, message.ContentType);
        Assert.Equal(CONTENT_ENCODING_SHIFT_JIS, message.ContentEncoding);
    }

    #region 単体テスト仕様書外の既存テスト
    [Fact]
    public void setContentTypeAndEncodingTest001()
    {
        IotMessage MyIotMessage = new IotMessage();

        Assert.Equal("application/json", MyIotMessage.GetContentType());
        Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());

    }
    [Fact]
    public void setContentTypeAndEncodingTest002()
    {
        Byte[] byteArray = new Byte[10];
        IotMessage MyIotMessage = new IotMessage(byteArray);

        Assert.Equal("application/json", MyIotMessage.GetContentType());
        Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());
    }
    [Fact]
    public void setContentTypeAndEncodingTest003()
    {
        string body = "test";
        IotMessage MyIotMessage = new IotMessage(body);

        Assert.Equal("application/json", MyIotMessage.GetContentType());
        Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());
    }

    [Fact]
    public void setContentTypeAndEncodingTest004()
    {
        Stream bodyStream = new MemoryStream();
        IotMessage MyIotMessage = new IotMessage(bodyStream);

        Assert.Equal("application/json", MyIotMessage.GetContentType());
        Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());
    }

    [Fact]
    public void setContentTypeAndEncodingTest005()
    {
        IotMessage orgMessage = new IotMessage();
        IotMessage MyIotMessage = new IotMessage(orgMessage);

        Assert.Equal("application/json", MyIotMessage.GetContentType());
        Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());
    }
    [Fact]
    public void setContentTypeAndEncodingTest006()
    {
        Message orgMessage = new Message();
        IotMessage MyIotMessage = new IotMessage(orgMessage);

        Assert.Equal("application/json", MyIotMessage.GetContentType());
        Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());
    }
    [Fact]
    public void setContentTypeAndEncodingTest007()
    {
        Message orgMessage = new Message();
        orgMessage.ContentType = "testContentType";
        orgMessage.ContentEncoding = "testContentEncoding";
        IotMessage MyIotMessage = new IotMessage(orgMessage);

        Assert.Equal(orgMessage.ContentType, MyIotMessage.GetContentType());
        Assert.Equal(orgMessage.ContentEncoding, MyIotMessage.GetContentEncoding());
    }
    #endregion
}
