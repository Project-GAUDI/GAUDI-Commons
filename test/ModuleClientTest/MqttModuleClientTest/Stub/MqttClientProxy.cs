using Xunit;

namespace TICO.GAUDI.Commons.Test.ModuleClientTest.MqttModuleClientTest.Stub;

/// <summary>
/// MqttClient に対する入出力値検証用クラス
/// </summary>
public static class MqttClientProxy
{
    private static string? _hostName;

    private static List<SubscribeMethodArguments> _subscribeMethodCalledHistory = [];

    private static string? _clientId;

    private static string? _username;

    private static string? _password;

    private static bool _published;

    private static string? _publishedTopic;

    private static byte[]? _publishedMessage;

    public static IReadOnlyList<SubscribeMethodArguments> SubscribeMethodCalledHistoy => _subscribeMethodCalledHistory;

    public static bool WasPublished => _published;

    public static string PublishedTopic => _publishedTopic ?? throw new NullReferenceException();

    public static byte[] PublishedMessage => _publishedMessage ?? throw new NullReferenceException();

    public static void SaveConstructorArguments(string hostName)
    {
        // 別テストで初期化した値がクリアされていない場合に検知できるように検証する
        Assert.Null(_hostName);

        _hostName = hostName;
    }

    public static void ReviewConstructorArguments(string expected)
    {
        Assert.Equal(expected, _hostName);
    }

    public static void StackSubscribeCall(string[] topics, byte[] qosLevels)
    {
        _subscribeMethodCalledHistory.Add(new SubscribeMethodArguments(){ Topics = topics, QosLevels = qosLevels });
    }

    public static void ClearSubscribeCallHistory()
    {
        _subscribeMethodCalledHistory.Clear();
    }

    public static void SaveConnectArguments(string clientId, string username, string password)
    {
        // 別テストで初期化した値がクリアされていない場合に検知できるように検証する
        Assert.Null(_clientId);
        Assert.Null(_username);
        Assert.Null(_password);

        _clientId = clientId;
        _username = username;
        _password = password;
    }

    public static void ReviewConnectArguments(string clientId, string username, string password)
    {
        Assert.Equal(clientId, _clientId);
        Assert.Equal(username, _username);
        Assert.Equal(password, _password);
    }

    public static void SavePublishArguments(string topic, byte[] message)
    {
        // 別テストで初期化した値がクリアされていない場合に検知できるように検証する
        Assert.False(_published);

        _published = true;
        _publishedTopic = topic;
        _publishedMessage = message;
    }

    public static void Clear()
    {
        _hostName = null;

        _subscribeMethodCalledHistory = [];

        _clientId = null;
        _username = null;
        _password = null;

        _published = false;
        _publishedTopic = null;
        _publishedMessage = null;
    }
}
