namespace TICO.GAUDI.Commons.Test.MessageTest.IotMessageTest;

public abstract class IotMessageTesterBase
{
    private const string MESSAGE_PROPERTY_NAME = "message";

    private const string BYTE_DATA_PROPERTY_NAME = "byteData";

    private const string BODY_STREAM_PROPERTY_NAME = "bodyStream";

    protected const string CONTENT_TYPE_APPLICATION_JSON = "application/json";

    protected const string CONTENT_TYPE_APPLICATION_XML = "application/xml";

    protected const string CONTENT_ENCODING_UTF8 = "utf-8";

    protected const string CONTENT_ENCODING_SHIFT_JIS = "shift-jis";

    private static object? GetProperty(IotMessage obj, string prop, BindingFlags bindingFlags)
    {
        var type = typeof(IotMessage);
        var pi = type.GetProperty(prop, bindingFlags);
        return pi?.GetValue(obj);
    }

    private static void SetProperty(IotMessage obj, string prop, object? value, BindingFlags bindingFlags)
    {
        var type = typeof(IotMessage);
        var pi = type.GetProperty(prop, bindingFlags);
        pi?.SetValue(obj, value);
    }

    protected static Message? GetMessageProperty(IotMessage obj)
    {
        var value = GetProperty(obj, MESSAGE_PROPERTY_NAME, BindingFlags.Instance | BindingFlags.NonPublic);
        return value as Message;
    }

    protected static void SetMessageProperty(IotMessage obj, Message? message)
    {
        SetProperty(obj, MESSAGE_PROPERTY_NAME, message, BindingFlags.Instance | BindingFlags.NonPublic);
    }

    protected static byte[]? GetByteDataProperty(IotMessage obj)
    {
        var value = GetProperty(obj, BYTE_DATA_PROPERTY_NAME, BindingFlags.Instance | BindingFlags.NonPublic);
        return value as byte[];
    }

    protected static Stream? GetBodyStreamProperty(IotMessage obj)
    {
        var value = GetProperty(obj, BODY_STREAM_PROPERTY_NAME, BindingFlags.Instance | BindingFlags.NonPublic);
        return value as Stream;
    }

    protected static void SetBodyStreamProperty(IotMessage obj, Stream? stream)
    {
        SetProperty(obj, BODY_STREAM_PROPERTY_NAME, stream, BindingFlags.Instance | BindingFlags.NonPublic);
    }
}
