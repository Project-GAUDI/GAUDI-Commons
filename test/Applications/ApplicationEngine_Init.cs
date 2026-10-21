using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Moq;
using Newtonsoft.Json.Linq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// LoggerのModuleClient・出力先（静的状態）を操作するため、Logger系テストと直列化する
[Collection(nameof(SharedLoggerState))]
public sealed class ApplicationEngine_Init
{
    [Fact(DisplayName = "No684_異常系：異常動作の確認_applicationMainが未設定(null)の状態で、メソッドを呼び出す")]
    public async Task Case684()
    {
        var engine = new ApplicationEngine();

        var exception = await Assert.ThrowsAsync<Exception>(() => engine.Init());

        Assert.Equal("Error: ApplicationMain not set.", exception.Message);
    }

    [Fact(DisplayName = "No685_異常系：異常動作の確認_applicationMain設定済み、ステートがReady（Initializeへ遷移不可）の状態で、メソッドを呼び出す")]
    public async Task Case685()
    {
        var engine = new ApplicationEngine();
        var app = NewApplicationMain(new List<string>());
        engine.SetApplication(app.Object);
        SetCurrentState(engine, ApplicationState.Ready);

        var exception = await Assert.ThrowsAsync<Exception>(() => engine.Init());

        Assert.Equal("Error: Failed to change the ApplicationState to 'Initialize'.", exception.Message);
        app.Verify(a => a.InitializeAsync(), Times.Never);
    }

    [Fact(DisplayName = "No686_正常系：正常動作の確認_applicationMain設定済み、ステートがStart、全ての処理が成功する状態で、メソッドを呼び出す")]
    public async Task Case686()
    {
        using var log = new LogCapture();
        var calls = new List<string>();
        var client = NewModuleClient(calls);
        using var patch = new ModuleClientFactoryPatch(client.Object, calls);
        var engine = new ApplicationEngine();
        var app = NewApplicationMain(calls);
        engine.SetApplication(app.Object);

        var result = await engine.Init();

        Assert.True(result);
        Assert.Equal(
            new[]
            {
                "InitializeAsync",
                "CreateAsync",
                "OpenAsync",
                "SetModuleClient",
                "GetTwinAsync",
                "OnDesiredPropertiesReceivedAsync",
                "SetDesiredPropertyUpdateCallbackAsync",
                "SetMethodHandlerAsync:SetLogLevel",
                "SetMethodHandlerAsync:GetLogLevel",
                "SetConnectionStatusChangedHandlerAsync",
                "StartAsync",
            },
            calls);
        Assert.Equal(new[] { "SetLogLevel", "GetLogLevel" }, GetMethodRequestEventData(engine).Keys);
        Assert.Same(client.Object, LogCapture.GetLoggerModuleClient());
    }

    [Fact(DisplayName = "No687_異常系：異常動作の確認_ModuleClientFactory.CreateAsync()で例外が発生する状態で、メソッドを呼び出す")]
    public async Task Case687()
    {
        using var log = new LogCapture();
        var calls = new List<string>();
        using var patch = new ModuleClientFactoryPatch(null, calls);
        var engine = new ApplicationEngine();
        engine.SetApplication(NewApplicationMain(calls).Object);

        var result = await engine.Init();

        Assert.False(result);
        Assert.Equal(new[] { "InitializeAsync", "CreateAsync" }, calls);
        Assert.Null(LogCapture.GetLoggerModuleClient());
        Assert.Contains("Creating Module Client failed.", log.Output);
    }

    [Fact(DisplayName = "No688_正常系：正常動作の確認_MyModuleClient.OpenAsync()が1回目に例外を発生し、2回目以降で成功する状態で、メソッドを呼び出す")]
    public async Task Case688()
    {
        using var log = new LogCapture();
        var calls = new List<string>();
        var client = NewModuleClient(calls);
        var openTimes = SetupOpenAsyncFailures(client, 1);
        using var patch = new ModuleClientFactoryPatch(client.Object, calls);
        var engine = new ApplicationEngine();
        engine.SetApplication(NewApplicationMain(calls).Object);

        var result = await engine.Init();

        Assert.True(result);
        Assert.Equal(2, openTimes.Count);
        Assert.Contains("Open a connection to the Edge runtime was failed.", log.Output);
    }

    [Fact(DisplayName = "No689_正常系：正常動作の確認_MyModuleClient.OpenAsync()が3回連続で例外を送出し、4回目に成功するスタブを設定して、メソッドを呼び出す")]
    public async Task Case689()
    {
        using var log = new LogCapture();
        var calls = new List<string>();
        var client = NewModuleClient(calls);
        var openTimes = SetupOpenAsyncFailures(client, 3);
        using var patch = new ModuleClientFactoryPatch(client.Object, calls);
        var engine = new ApplicationEngine();
        engine.SetApplication(NewApplicationMain(calls).Object);

        var result = await engine.Init();

        Assert.True(result);
        Assert.Equal(4, openTimes.Count);
        for (var i = 1; i < openTimes.Count; i++)
        {
            Assert.True(openTimes[i] - openTimes[i - 1] >= TimeSpan.FromMilliseconds(900));
        }
        Assert.Equal(3, CountOccurrences(log.Output, "Open a connection to the Edge runtime was failed."));
    }

    [Fact(DisplayName = "No690_異常系：異常動作の確認_MyModuleClient.GetTwinAsync()で例外が発生する状態で、メソッドを呼び出す")]
    public async Task Case690()
    {
        using var log = new LogCapture();
        var calls = new List<string>();
        var client = NewModuleClient(calls);
        client.Setup(c => c.GetTwinAsync()).ThrowsAsync(new InvalidOperationException("twin error"));
        using var patch = new ModuleClientFactoryPatch(client.Object, calls);
        var engine = new ApplicationEngine();
        var app = NewApplicationMain(calls);
        engine.SetApplication(app.Object);

        var result = await engine.Init();

        Assert.False(result);
        app.Verify(a => a.OnDesiredPropertiesReceivedAsync(It.IsAny<JObject>()), Times.Never);
        app.Verify(a => a.StartAsync(), Times.Never);
        Assert.Contains("Receiving module twin was failed.", log.Output);
    }

    [Fact(DisplayName = "No691_異常系：異常動作の確認_DesiredPropertiesUpdate()で例外が発生する状態で、メソッドを呼び出す")]
    public async Task Case691()
    {
        using var log = new LogCapture();
        var calls = new List<string>();
        var client = NewModuleClient(calls);
        using var patch = new ModuleClientFactoryPatch(client.Object, calls);
        using var desiredPatch = new DesiredPropertiesUpdatePatch();
        var engine = new ApplicationEngine();
        var app = NewApplicationMain(calls);
        engine.SetApplication(app.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => engine.Init());

        app.Verify(a => a.StartAsync(), Times.Never);
    }

    [Fact(DisplayName = "No692_異常系：異常動作の確認_DesiredPropertiesUpdate()がfalseを返す状態で、メソッドを呼び出す")]
    public async Task Case692()
    {
        using var log = new LogCapture();
        var calls = new List<string>();
        var client = NewModuleClient(calls);
        using var patch = new ModuleClientFactoryPatch(client.Object, calls);
        var engine = new ApplicationEngine();
        var app = NewApplicationMain(calls);
        app.Setup(a => a.OnDesiredPropertiesReceivedAsync(It.IsAny<JObject>())).ReturnsAsync(false);
        engine.SetApplication(app.Object);

        var result = await engine.Init();

        Assert.False(result);
        client.Verify(c => c.SetDesiredPropertyUpdateCallbackAsync(It.IsAny<DesiredPropertyUpdateCallback>(), It.IsAny<object>()), Times.Never);
        client.Verify(c => c.SetMethodHandlerAsync(It.IsAny<string>(), It.IsAny<MethodCallback>(), It.IsAny<object>()), Times.Never);
        client.Verify(c => c.SetConnectionStatusChangedHandlerAsync(It.IsAny<IotConnectionStatusChangeHandler>()), Times.Never);
        app.Verify(a => a.StartAsync(), Times.Never);
        Assert.Empty(GetMethodRequestEventData(engine));
    }

    [Fact(DisplayName = "No693_異常系：異常動作の確認_applicationMain.StartAsync()がfalseを返す状態で、メソッドを呼び出す")]
    public async Task Case693()
    {
        using var log = new LogCapture();
        var calls = new List<string>();
        var client = NewModuleClient(calls);
        using var patch = new ModuleClientFactoryPatch(client.Object, calls);
        var engine = new ApplicationEngine();
        var app = NewApplicationMain(calls);
        app.Setup(a => a.StartAsync()).ReturnsAsync(false);
        engine.SetApplication(app.Object);

        var result = await engine.Init();

        Assert.False(result);
        app.Verify(a => a.StartAsync(), Times.Once);
    }

    private static Mock<IApplicationMain> NewApplicationMain(List<string> calls)
    {
        var app = new Mock<IApplicationMain>();
        app.Setup(a => a.InitializeAsync()).Callback(() => calls.Add("InitializeAsync")).ReturnsAsync(true);
        app.Setup(a => a.StartAsync()).Callback(() => calls.Add("StartAsync")).ReturnsAsync(true);
        app.Setup(a => a.TerminateAsync()).ReturnsAsync(true);
        app.Setup(a => a.OnDesiredPropertiesReceivedAsync(It.IsAny<JObject>()))
            .Callback(() => calls.Add("OnDesiredPropertiesReceivedAsync"))
            .ReturnsAsync(true);
        return app;
    }

    private static Mock<IModuleClient> NewModuleClient(List<string> calls)
    {
        var client = new Mock<IModuleClient>();
        client.Setup(c => c.OpenAsync()).Callback(() => calls.Add("OpenAsync")).Returns(Task.CompletedTask);
        client.Setup(c => c.GetTwinAsync())
            .Callback(() =>
            {
                if (ReferenceEquals(LogCapture.GetLoggerModuleClient(), client.Object))
                {
                    calls.Add("SetModuleClient");
                }
                calls.Add("GetTwinAsync");
            })
            .ReturnsAsync(new Twin());
        client.Setup(c => c.SetDesiredPropertyUpdateCallbackAsync(It.IsAny<DesiredPropertyUpdateCallback>(), It.IsAny<object>()))
            .Callback(() => calls.Add("SetDesiredPropertyUpdateCallbackAsync"))
            .Returns(Task.CompletedTask);
        client.Setup(c => c.SetMethodHandlerAsync(It.IsAny<string>(), It.IsAny<MethodCallback>(), It.IsAny<object>()))
            .Callback<string, MethodCallback, object>((name, _, _) => calls.Add($"SetMethodHandlerAsync:{name}"))
            .Returns(Task.CompletedTask);
        client.Setup(c => c.SetConnectionStatusChangedHandlerAsync(It.IsAny<IotConnectionStatusChangeHandler>()))
            .Callback(() => calls.Add("SetConnectionStatusChangedHandlerAsync"))
            .Returns(Task.CompletedTask);
        return client;
    }

    private static List<TimeSpan> SetupOpenAsyncFailures(Mock<IModuleClient> client, int failureCount)
    {
        var times = new List<TimeSpan>();
        var stopwatch = Stopwatch.StartNew();
        client.Setup(c => c.OpenAsync()).Returns(() =>
        {
            times.Add(stopwatch.Elapsed);
            return times.Count <= failureCount
                ? Task.FromException(new InvalidOperationException("open error"))
                : Task.CompletedTask;
        });
        return times;
    }

    private static int CountOccurrences(string text, string value)
    {
        var count = 0;
        for (var index = text.IndexOf(value, StringComparison.Ordinal); index >= 0; index = text.IndexOf(value, index + value.Length, StringComparison.Ordinal))
        {
            count++;
        }
        return count;
    }

    private static Dictionary<string, MethodEventData> GetMethodRequestEventData(ApplicationEngine engine)
    {
        return (Dictionary<string, MethodEventData>)typeof(ApplicationEngine)
            .GetField("methodRequestEventData", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
    }

    private static void SetCurrentState(ApplicationEngine engine, ApplicationState state)
    {
        var stateController = typeof(ApplicationEngine)
            .GetField("stateController", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(engine)!;
        typeof(StateController)
            .GetField("currentState", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(stateController, state);
    }

    private sealed class LogCapture : IDisposable
    {
        private static readonly Type LoggerType = typeof(ILogger).Assembly.GetType("TICO.GAUDI.Commons.Logger")!;
        private readonly StringWriter writer = new();
        private readonly object? originalWriter;
        private readonly object? originalLogLevel;
        private readonly object? originalPermanent;
        private readonly object? originalClient;

        public string Output => writer.ToString();

        public LogCapture()
        {
            originalWriter = GetStatic("outTextWriter");
            originalLogLevel = GetStatic("outputLogLevel");
            originalPermanent = GetStatic("isMandatoryLoglevelPermanent");
            originalClient = GetStatic("myClient");

            SetStatic("outTextWriter", TextWriter.Synchronized(writer));
            SetStatic("outputLogLevel", ILogger.LogLevel.INFO);
            SetStatic("isMandatoryLoglevelPermanent", false);
            SetStatic("myClient", null);
        }

        public static object? GetLoggerModuleClient() => GetStatic("myClient");

        public void Dispose()
        {
            SetStatic("outTextWriter", originalWriter);
            SetStatic("outputLogLevel", originalLogLevel);
            SetStatic("isMandatoryLoglevelPermanent", originalPermanent);
            SetStatic("myClient", originalClient);
            writer.Dispose();
        }

        private static object? GetStatic(string name)
        {
            return LoggerType.GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        }

        private static void SetStatic(string name, object? value)
        {
            LoggerType.GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, value);
        }
    }

    private sealed class ModuleClientFactoryPatch : IDisposable
    {
        private readonly Harmony harmony;

        private static IModuleClient? moduleClient;

        private static List<string>? calls;

        public ModuleClientFactoryPatch(IModuleClient? client, List<string> callList)
        {
            moduleClient = client;
            calls = callList;

            harmony = new Harmony($"{nameof(ApplicationEngine_Init)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ModuleClientFactory), nameof(ModuleClientFactory.CreateAsync)),
                prefix: new HarmonyMethod(typeof(ModuleClientFactoryPatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
            moduleClient = null;
            calls = null;
        }

        // moduleClient が null の場合は生成失敗として例外を返す
        private static bool Prefix(ref Task<IModuleClient> __result)
        {
            calls?.Add("CreateAsync");
            __result = moduleClient != null
                ? Task.FromResult(moduleClient)
                : Task.FromException<IModuleClient>(new InvalidOperationException("create error"));
            return false;
        }
    }

    private sealed class DesiredPropertiesUpdatePatch : IDisposable
    {
        private readonly Harmony harmony;

        public DesiredPropertiesUpdatePatch()
        {
            harmony = new Harmony($"{nameof(ApplicationEngine_Init)}.{Guid.NewGuid()}");
            harmony.Patch(
                AccessTools.Method(typeof(ApplicationEngine), "DesiredPropertiesUpdate"),
                prefix: new HarmonyMethod(typeof(DesiredPropertiesUpdatePatch), nameof(Prefix)));
        }

        public void Dispose()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static bool Prefix(ref Task<bool> __result)
        {
            __result = Task.FromException<bool>(new InvalidOperationException("desired update error"));
            return false;
        }
    }
}
