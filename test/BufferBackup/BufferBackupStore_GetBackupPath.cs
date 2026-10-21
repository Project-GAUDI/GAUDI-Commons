using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// IOTEDGE_MODULEIDを操作する他のテスト（MqttModuleClient系）と直列化する
[Collection(nameof(MqttModuleClientHarmonyCollection))]
public sealed class BufferBackupStore_GetBackupPath
{
    private const string ModuleIdEnvName = "IOTEDGE_MODULEID";

    private static readonly string BaseDirectory = Path.Combine(Path.GetTempPath(), "backup");

    [Fact(DisplayName = "No768_正常系：正常動作の確認_ENV:IOTEDGE_MODULEID=Module01の状態で、keyに\"queue1\"を指定して、メソッドを呼び出す")]
    public void Case768()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        var store = new BufferBackupStore(BaseDirectory, false);

        var result = store.GetBackupPath("queue1");

        Assert.Equal(Path.Combine(BaseDirectory, "Module01_queue1_backup.json"), result);
    }

    [Fact(DisplayName = "No769_正常系：正常動作の確認_ENV:IOTEDGE_MODULEID=Module01の状態で、keyにnullを指定して、メソッドを呼び出す")]
    public void Case769()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        var store = new BufferBackupStore(BaseDirectory, false);

        var result = store.GetBackupPath(null!);

        Assert.Equal(Path.Combine(BaseDirectory, "Module01_backup.json"), result);
    }

    [Fact(DisplayName = "No770_正常系：正常動作の確認_ENV:IOTEDGE_MODULEID=未指定の状態で、keyに\"queue1\"を指定して、メソッドを呼び出す")]
    public void Case770()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, null);
        var store = new BufferBackupStore(BaseDirectory, false);

        var result = store.GetBackupPath("queue1");

        Assert.Equal(Path.Combine(BaseDirectory, "default_queue1_backup.json"), result);
    }

    [Fact(DisplayName = "No771_異常系：異常動作の確認_ENV:IOTEDGE_MODULEID=Module01の状態で、keyに\"../queue1\"を指定して、メソッドを呼び出す")]
    public void Case771()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        var store = new BufferBackupStore(BaseDirectory, false);

        Assert.Throws<ArgumentException>(() => store.GetBackupPath("../queue1"));
    }

    [Fact(DisplayName = "No772_異常系：異常動作の確認_ENV:IOTEDGE_MODULEID=Invalid/Moduleの状態で、keyに\"queue1\"を指定して、メソッドを呼び出す")]
    public void Case772()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Invalid/Module");
        var store = new BufferBackupStore(BaseDirectory, false);

        Assert.Throws<ArgumentException>(() => store.GetBackupPath("queue1"));
    }

    private sealed class EnvironmentVariableScope : IDisposable
    {
        private readonly string name;
        private readonly string? originalValue;

        public EnvironmentVariableScope(string name, string? value)
        {
            this.name = name;
            originalValue = Environment.GetEnvironmentVariable(name);
            Environment.SetEnvironmentVariable(name, value);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(name, originalValue);
        }
    }
}
