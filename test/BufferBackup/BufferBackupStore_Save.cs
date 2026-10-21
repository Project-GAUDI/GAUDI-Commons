using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

// IOTEDGE_MODULEIDとLogger出力先（静的状態）を操作するため、他テストと並列実行されないコレクションに含める
[Collection(nameof(SharedLoggerState))]
public sealed class BufferBackupStore_Save
{
    private const string ModuleIdEnvName = "IOTEDGE_MODULEID";

    [Fact(DisplayName = "No773_正常系：正常動作の確認_ENV:IOTEDGE_MODULEID=Module01、baseDirectory=存在しないディレクトリの状態で、keyに\"queue1\"、itemsに[1,2,3]を指定して、メソッドを呼び出す")]
    public void Case773()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var baseDirectory = Path.Combine(temp.Path, "notexist");
        var store = new BufferBackupStore(baseDirectory, false);

        var result = store.Save("queue1", new[] { 1, 2, 3 });

        Assert.True(result);
        Assert.True(Directory.Exists(baseDirectory));
        var path = Path.Combine(baseDirectory, "Module01_queue1_backup.json");
        Assert.True(File.Exists(path));
        Assert.Equal("[1,2,3]", File.ReadAllText(path));
    }

    [Fact(DisplayName = "No774_正常系：正常動作の確認_ENV:IOTEDGE_MODULEID=Module01、baseDirectory=任意のディレクトリの状態で、同じkeyで異なるitemsを2回指定して、メソッドを呼び出す")]
    public void Case774()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);

        var first = store.Save("queue1", new[] { 1, 2, 3 });
        var second = store.Save("queue1", new[] { 4, 5 });

        Assert.True(first);
        Assert.True(second);
        Assert.Equal("[4,5]", File.ReadAllText(Path.Combine(temp.Path, "Module01_queue1_backup.json")));
    }

    [Fact(DisplayName = "No775_正常系：正常動作の確認_preserveDerivedTypes=trueの状態で、基底クラス型のリストに派生クラスのインスタンスを含めて、itemsに派生型を含むリストを指定して、メソッドを呼び出す")]
    public void Case775()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, true);
        var items = new List<BaseItem> { new BaseItem { Name = "base" }, new DerivedItem { Name = "derived", Extra = "x" } };

        var result = store.Save("queue1", items);

        Assert.True(result);
        var json = File.ReadAllText(Path.Combine(temp.Path, "Module01_queue1_backup.json"));
        Assert.Contains("\"$type\"", json);
        Assert.Contains(nameof(DerivedItem), json);
    }

    [Fact(DisplayName = "No776_正常系：正常動作の確認_preserveDerivedTypes=falseの状態で、基底クラス型のリストに派生クラスのインスタンスを含めて、itemsに派生型を含むリストを指定して、メソッドを呼び出す")]
    public void Case776()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);
        var items = new List<BaseItem> { new BaseItem { Name = "base" }, new DerivedItem { Name = "derived", Extra = "x" } };

        var result = store.Save("queue1", items);

        Assert.True(result);
        var json = File.ReadAllText(Path.Combine(temp.Path, "Module01_queue1_backup.json"));
        Assert.DoesNotContain("\"$type\"", json);
    }

    [Fact(DisplayName = "No777_異常系：異常動作の確認_keyに\"../queue1\"を指定して、メソッドを呼び出す")]
    public void Case777()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        var store = new BufferBackupStore(temp.Path, false);

        Assert.Throws<ArgumentException>(() => store.Save("../queue1", new[] { 1, 2, 3 }));
    }

    [Fact(DisplayName = "No778_異常系：異常動作の確認_itemsにnullを指定して、メソッドを呼び出す")]
    public void Case778()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        using var log = new LoggerOutputCapture();
        var store = new BufferBackupStore(temp.Path, false);

        var result = store.Save<int>("queue1", null!);

        Assert.False(result);
        Assert.False(File.Exists(Path.Combine(temp.Path, "Module01_queue1_backup.json")));
        Assert.Contains("Backup skipped because items is null.", log.Output);
    }

    [Fact(DisplayName = "No779_異常系：異常動作の確認_baseDirectoryがファイルとして存在するパスの状態で、メソッドを呼び出す")]
    public void Case779()
    {
        using var env = new EnvironmentVariableScope(ModuleIdEnvName, "Module01");
        using var temp = new TempDirectory();
        using var log = new LoggerOutputCapture();
        var baseDirectory = Path.Combine(temp.Path, "file");
        File.WriteAllText(baseDirectory, "not a directory");
        var store = new BufferBackupStore(baseDirectory, false);

        var result = store.Save("queue1", new[] { 1, 2, 3 });

        Assert.False(result);
        Assert.False(File.Exists(store.GetBackupPath("queue1") + ".tmp"));
        Assert.Contains("Failed to save buffer to disk.", log.Output);
    }

    public class BaseItem
    {
        public string Name { get; set; } = "";
    }

    public sealed class DerivedItem : BaseItem
    {
        public string Extra { get; set; } = "";
    }
}
