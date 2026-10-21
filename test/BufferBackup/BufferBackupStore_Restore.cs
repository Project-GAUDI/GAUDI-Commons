using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// IOTEDGE_MODULEIDとLogger出力先（静的状態）を操作するため、他テストと並列実行されないコレクションに含める
[Collection(nameof(SharedLoggerState))]
public sealed class BufferBackupStore_Restore
{
    private const string ModuleIdEnvName = "IOTEDGE_MODULEID";

    [Fact(DisplayName = "No780_正常系：正常動作の確認_Saveでkeyに\"queue1\"、itemsに[1,2,3]を保存後、deleteAfterRestore=trueを指定して、メソッドを呼び出す")]
    public void Case780()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);
        store.Save("queue1", new[] { 1, 2, 3 });

        var result = store.Restore<int>("queue1", true);

        Assert.Equal(new[] { 1, 2, 3 }, result);
        Assert.False(File.Exists(BackupPath(temp)));
    }

    [Fact(DisplayName = "No781_正常系：正常動作の確認_Saveでkeyに\"queue1\"、itemsに[1,2,3]を保存後、deleteAfterRestore=falseを指定して、メソッドを呼び出す")]
    public void Case781()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);
        store.Save("queue1", new[] { 1, 2, 3 });

        var result = store.Restore<int>("queue1", false);

        Assert.Equal(new[] { 1, 2, 3 }, result);
        Assert.True(File.Exists(BackupPath(temp)));
    }

    [Fact(DisplayName = "No782_正常系：正常動作の確認_退避ファイルが存在しない状態で、メソッドを呼び出す")]
    public void Case782()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);

        var result = store.Restore<int>("queue1");

        Assert.Null(result);
    }

    [Fact(DisplayName = "No783_異常系：異常動作の確認_keyに\"../queue1\"を指定して、メソッドを呼び出す")]
    public void Case783()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);

        Assert.Throws<ArgumentException>(() => store.Restore<int>("../queue1"));
    }

    [Fact(DisplayName = "No784_異常系：異常動作の確認_退避ファイルに\"null\"を書き込んだ状態で、deleteAfterRestore=trueを指定して、メソッドを呼び出す")]
    public void Case784()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        using var log = new LoggerOutputCapture();
        var store = new BufferBackupStore(temp.Path, false);
        File.WriteAllText(BackupPath(temp), "null");

        var result = store.Restore<int>("queue1", true);

        Assert.Null(result);
        Assert.False(File.Exists(BackupPath(temp)));
        Assert.Contains("Failed to deserialize backup file.", log.Output);
    }

    [Fact(DisplayName = "No785_異常系：異常動作の確認_退避ファイルに不正なJSONを書き込んだ状態で、メソッドを呼び出す")]
    public void Case785()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        using var log = new LoggerOutputCapture();
        var store = new BufferBackupStore(temp.Path, false);
        File.WriteAllText(BackupPath(temp), "{ invalid json");

        var result = store.Restore<int>("queue1");

        // シリアライザがデシリアライズ例外を内部で捕捉してnullを返すため、catch節ではなくnull判定の分岐に入る
        Assert.Null(result);
        Assert.False(File.Exists(BackupPath(temp)));
        Assert.Contains("Failed to deserialize backup file.", log.Output);
        Assert.DoesNotContain("Failed to restore buffer from disk.", log.Output);
    }

    [Fact(DisplayName = "No786_異常系：異常動作の確認_退避ファイルが排他ロックされ読み込みに失敗する状態で、メソッドを呼び出す")]
    public void Case786()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        using var log = new LoggerOutputCapture();
        var store = new BufferBackupStore(temp.Path, false);
        store.Save("queue1", new[] { 1, 2, 3 });
        using var lockStream = new FileStream(BackupPath(temp), FileMode.Open, FileAccess.ReadWrite, FileShare.None);

        var result = store.Restore<int>("queue1", true);

        Assert.Null(result);
        Assert.True(File.Exists(BackupPath(temp)));
        Assert.Contains("Failed to restore buffer from disk.", log.Output);
    }

    private static string BackupPath(TempDirectory temp)
    {
        return Path.Combine(temp.Path, "Module01_queue1_backup.json");
    }
}
