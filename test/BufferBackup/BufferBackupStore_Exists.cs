using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// IOTEDGE_MODULEIDを操作するため、他テストと並列実行されないコレクションに含める
[Collection(nameof(SharedLoggerState))]
public sealed class BufferBackupStore_Exists
{
    private const string ModuleIdEnvName = "IOTEDGE_MODULEID";

    [Fact(DisplayName = "No787_正常系：正常動作の確認_Saveでkeyに\"queue1\"を保存後、同じkeyを指定して、メソッドを呼び出す")]
    public void Case787()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);
        store.Save("queue1", new[] { 1, 2, 3 });

        var result = store.Exists("queue1");

        Assert.True(result);
    }

    [Fact(DisplayName = "No788_正常系：正常動作の確認_退避ファイルが存在しない状態で、keyに\"queue1\"を指定して、メソッドを呼び出す")]
    public void Case788()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);

        var result = store.Exists("queue1");

        Assert.False(result);
    }

    [Fact(DisplayName = "No789_異常系：異常動作の確認_keyに\"queue\\\\1\"を指定して、メソッドを呼び出す")]
    public void Case789()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);

        Assert.Throws<ArgumentException>(() => store.Exists("queue\\1"));
    }
}
