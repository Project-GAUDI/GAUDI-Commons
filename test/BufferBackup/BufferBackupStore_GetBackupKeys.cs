using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// IOTEDGE_MODULEIDを操作するため、他テストと並列実行されないコレクションに含める
[Collection(nameof(SharedLoggerState))]
public sealed class BufferBackupStore_GetBackupKeys
{
    private const string ModuleIdEnvName = "IOTEDGE_MODULEID";

    [Fact(DisplayName = "No793_正常系：正常動作の確認_baseDirectoryが存在しないディレクトリの状態で、メソッドを呼び出す")]
    public void Case793()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(Path.Combine(temp.Path, "notexist"), false);

        var result = store.GetBackupKeys();

        Assert.Empty(result);
    }

    [Fact(DisplayName = "No794_正常系：正常動作の確認_ENV:IOTEDGE_MODULEID=Module01、Module01_backup.jsonが存在する状態で、メソッドを呼び出す")]
    public void Case794()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        CreateFiles(temp, "Module01_backup.json");
        var store = new BufferBackupStore(temp.Path, false);

        var result = store.GetBackupKeys();

        Assert.Equal(new[] { string.Empty }, result);
    }

    [Fact(DisplayName = "No795_正常系：正常動作の確認_ENV:IOTEDGE_MODULEID=Module01の状態で、Module01_queue1_backup.json、Module01_queue2_backup.json、Other_queue3_backup.jsonが存在する状態で、メソッドを呼び出す")]
    public void Case795()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        CreateFiles(temp, "Module01_queue1_backup.json", "Module01_queue2_backup.json", "Other_queue3_backup.json");
        var store = new BufferBackupStore(temp.Path, false);

        var result = store.GetBackupKeys();

        Assert.Equal(new[] { "queue1", "queue2" }, result.OrderBy(k => k, StringComparer.Ordinal));
        Assert.DoesNotContain("queue3", result);
    }

    [Fact(DisplayName = "No796_正常系：正常動作の確認_ENV:IOTEDGE_MODULEID=Module01の状態で、Module01_queue1_backup.json.tmp、Module01_queue1.txt、Other_queue1_backup.jsonが存在する状態で、メソッドを呼び出す")]
    public void Case796()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        CreateFiles(temp, "Module01_queue1_backup.json.tmp", "Module01_queue1.txt", "Other_queue1_backup.json");
        var store = new BufferBackupStore(temp.Path, false);

        var result = store.GetBackupKeys();

        Assert.Empty(result);
    }

    [Fact(DisplayName = "No797_異常系：異常動作の確認_ENV:IOTEDGE_MODULEID=Invalid/Moduleの状態で、メソッドを呼び出す")]
    public void Case797()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Invalid/Module");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);

        Assert.Throws<ArgumentException>(() => store.GetBackupKeys());
    }

    private static void CreateFiles(TempDirectory temp, params string[] fileNames)
    {
        foreach (var fileName in fileNames)
        {
            File.WriteAllText(Path.Combine(temp.Path, fileName), "[]");
        }
    }
}
