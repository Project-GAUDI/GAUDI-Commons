using System;
using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test.DirectMethodTest;

// Loggerのテストと競合するため、Collectionを指定
[Trait("LoggerTest", "true")]
[Collection(nameof(LoggerTest))]
public class DirectMethod_SetLogLevel_RequestData
{
    private readonly ITestOutputHelper _output;

    public DirectMethod_SetLogLevel_RequestData(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "DirectMethod Case025_初期化したインスタンスに対してメソッドを呼び出す")]
    public void DirectMethod_Case025()
    {
        DirectMethod_SetLogLevel setLogLevel = new();

        var requestDataField = setLogLevel.GetType().GetField("<requestData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var requestData = requestDataField!.GetValue(setLogLevel);
        requestData = Activator.CreateInstance(requestDataField.FieldType);
        requestDataField.SetValue(setLogLevel, requestData);

        var validateMethod = requestData!.GetType().GetMethod("Validate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        var ret = validateMethod!.Invoke(requestData, null) as bool?;

        Assert.False(ret);
    }

    [Fact(DisplayName = "DirectMethod Case026_初期化したインスタンスに対し、EnableSecに 0 を設定して、メソッドを呼び出す")]
    public void DirectMethod_Case026()
    {
        DirectMethod_SetLogLevel setLogLevel = new();

        var requestDataField = setLogLevel.GetType().GetField("<requestData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var requestData = requestDataField!.GetValue(setLogLevel);
        requestData = Activator.CreateInstance(requestDataField.FieldType);

        var enableSecProperty = requestData!.GetType().GetProperty("EnableSec", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        enableSecProperty!.SetValue(requestData, 0);
        requestDataField.SetValue(setLogLevel, requestData);

        var validateMethod = requestData!.GetType().GetMethod("Validate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        var ret = validateMethod!.Invoke(requestData, null) as bool?;

        Assert.True(ret);
    }

    [Fact(DisplayName = "DirectMethod Case027_初期化したインスタンスに対し、LogLevelに DEBUG を設定して、メソッドを呼び出す")]
    public void DirectMethod_Case027()
    {
        DirectMethod_SetLogLevel setLogLevel = new();

        var requestDataField = setLogLevel.GetType().GetField("<requestData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var requestData = requestDataField!.GetValue(setLogLevel);
        requestData = Activator.CreateInstance(requestDataField.FieldType);

        var logLevelProperty = requestData!.GetType().GetField("LogLevel", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        logLevelProperty!.SetValue(requestData, "debug");
        requestDataField.SetValue(setLogLevel, requestData);

        var validateMethod = requestData!.GetType().GetMethod("Validate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        var ret = validateMethod!.Invoke(requestData, null) as bool?;

        Assert.False(ret);
    }

    [Fact(DisplayName = "DirectMethod Case028_初期化したインスタンスに対し、EnableSecに 1、LogLevelに DEBUG を設定して、メソッドを呼び出す")]
    public void DirectMethod_Case028()
    {
        DirectMethod_SetLogLevel setLogLevel = new();

        var requestDataField = setLogLevel.GetType().GetField("<requestData>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var requestData = requestDataField!.GetValue(setLogLevel);
        requestData = Activator.CreateInstance(requestDataField.FieldType);

        var enableSecProperty = requestData!.GetType().GetProperty("EnableSec", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        enableSecProperty!.SetValue(requestData, 1);
        requestDataField.SetValue(setLogLevel, requestData);

        var logLevelProperty = requestData!.GetType().GetField("LogLevel", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        logLevelProperty!.SetValue(requestData, "debug");
        requestDataField.SetValue(setLogLevel, requestData);

        var validateMethod = requestData!.GetType().GetMethod("Validate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        var ret = validateMethod!.Invoke(requestData, null) as bool?;

        Assert.True(ret);
    }
}