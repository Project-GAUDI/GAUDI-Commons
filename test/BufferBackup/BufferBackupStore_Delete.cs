using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// IOTEDGE_MODULEIDを操作するため、他テストと並列実行されないコレクションに含める
[Collection(nameof(SharedLoggerState))]
public sealed class BufferBackupStore_Delete
{
    private const string ModuleIdEnvName = "IOTEDGE_MODULEID";

    [Fact(DisplayName = "No790_正常系：正常動作の確認_Saveでkeyに\"queue1\"を保存後、同じkeyを指定して、メソッドを呼び出す")]
    public void Case790()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);
        store.Save("queue1", new[] { 1, 2, 3 });
        var path = Path.Combine(temp.Path, "Module01_queue1_backup.json");
        Assert.True(File.Exists(path));

        var result = store.Delete("queue1");

        Assert.True(result);
        Assert.False(File.Exists(path));
    }

    [Fact(DisplayName = "No791_正常系：正常動作の確認_退避ファイルが存在しない状態で、keyに\"queue1\"を指定して、メソッドを呼び出す")]
    public void Case791()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);

        var result = store.Delete("queue1");

        Assert.True(result);
    }

    [Fact(DisplayName = "No792_異常系：異常動作の確認_keyに\"queue/1\"を指定して、メソッドを呼び出す")]
    public void Case792()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);

        Assert.Throws<ArgumentException>(() => store.Delete("queue/1"));
    }
}
