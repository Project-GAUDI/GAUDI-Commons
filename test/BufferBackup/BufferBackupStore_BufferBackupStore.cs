using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class BufferBackupStore_BufferBackupStore
{
    [Fact(DisplayName = "No764_正常系：正常動作の確認_baseDirectoryに任意のディレクトリ、preserveDerivedTypesにfalseを指定して、コンストラクタを呼び出す")]
    public void Case764()
    {
        var baseDirectory = Path.Combine(Path.GetTempPath(), "backup");

        var store = new BufferBackupStore(baseDirectory, false);

        Assert.NotNull(store);
        Assert.Equal(baseDirectory, typeof(BufferBackupStore)
            .GetField("baseDirectory", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(store));
    }

    [Fact(DisplayName = "No765_異常系：異常動作の確認_baseDirectoryにnull、preserveDerivedTypesにfalseを指定して、コンストラクタを呼び出す")]
    public void Case765()
    {
        Assert.Throws<ArgumentException>(() => new BufferBackupStore(null!, false));
    }

    [Fact(DisplayName = "No766_異常系：異常動作の確認_baseDirectoryに\"\"、preserveDerivedTypesにfalseを指定して、コンストラクタを呼び出す")]
    public void Case766()
    {
        Assert.Throws<ArgumentException>(() => new BufferBackupStore("", false));
    }

    [Fact(DisplayName = "No767_異常系：異常動作の確認_baseDirectoryに\" \"、preserveDerivedTypesにfalseを指定して、コンストラクタを呼び出す")]
    public void Case767()
    {
        Assert.Throws<ArgumentException>(() => new BufferBackupStore(" ", false));
    }
}
