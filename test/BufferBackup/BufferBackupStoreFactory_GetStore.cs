using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class BufferBackupStoreFactory_GetStore
{
    private const string DefaultDirectory = "/app/backupdata";

    [Fact(DisplayName = "No798_正常系：正常動作の確認_preserveDerivedTypesを指定せず、メソッドを呼び出す")]
    public void Case798()
    {
        var result = BufferBackupStoreFactory.GetStore();

        AssertStore(result, DefaultDirectory, JsonTypeNameHandling.None);
    }

    [Fact(DisplayName = "No799_正常系：正常動作の確認_preserveDerivedTypes=falseを指定して、メソッドを呼び出す")]
    public void Case799()
    {
        var result = BufferBackupStoreFactory.GetStore(false);

        AssertStore(result, DefaultDirectory, JsonTypeNameHandling.None);
    }

    [Fact(DisplayName = "No800_正常系：正常動作の確認_preserveDerivedTypes=trueを指定して、メソッドを呼び出す")]
    public void Case800()
    {
        var result = BufferBackupStoreFactory.GetStore(true);

        AssertStore(result, DefaultDirectory, JsonTypeNameHandling.Auto);
    }

    [Fact(DisplayName = "No801_正常系：正常動作の確認_baseDirectoryに存在する任意のディレクトリ、preserveDerivedTypesを指定せず、メソッドを呼び出す")]
    public void Case801()
    {
        using var temp = new TempDirectory();

        var result = BufferBackupStoreFactory.GetStore(temp.Path);

        AssertStore(result, temp.Path, JsonTypeNameHandling.None);
    }

    [Fact(DisplayName = "No802_正常系：正常動作の確認_baseDirectoryに存在する任意のディレクトリ、preserveDerivedTypes=falseを指定してメソッドを呼び出す")]
    public void Case802()
    {
        using var temp = new TempDirectory();

        var result = BufferBackupStoreFactory.GetStore(temp.Path, false);

        AssertStore(result, temp.Path, JsonTypeNameHandling.None);
    }

    [Fact(DisplayName = "No803_正常系：正常動作の確認_baseDirectoryに存在する任意のディレクトリ、preserveDerivedTypes=trueを指定してメソッドを呼び出す")]
    public void Case803()
    {
        using var temp = new TempDirectory();

        var result = BufferBackupStoreFactory.GetStore(temp.Path, true);

        AssertStore(result, temp.Path, JsonTypeNameHandling.Auto);
    }

    [Fact(DisplayName = "No804_異常系：異常動作の確認_baseDirectoryにnullを指定して、メソッドを呼び出す")]
    public void Case804()
    {
        Assert.Throws<ArgumentException>(() => BufferBackupStoreFactory.GetStore(null!, false));
    }

    [Fact(DisplayName = "No805_異常系：異常動作の確認_baseDirectoryに空文字を指定して、メソッドを呼び出す")]
    public void Case805()
    {
        Assert.Throws<ArgumentException>(() => BufferBackupStoreFactory.GetStore("", false));
    }

    [Fact(DisplayName = "No806_異常系：異常動作の確認_baseDirectoryに空白文字を指定して、メソッドを呼び出す")]
    public void Case806()
    {
        Assert.Throws<ArgumentException>(() => BufferBackupStoreFactory.GetStore(" ", false));
    }

    // preserveDerivedTypes は TypeNameHandling（true: Auto / false: None）として保持される
    private static void AssertStore(IBufferBackupStore store, string expectedDirectory, JsonTypeNameHandling expectedTypeNameHandling)
    {
        var backupStore = Assert.IsType<BufferBackupStore>(store);
        var baseDirectory = typeof(BufferBackupStore)
            .GetField("baseDirectory", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(backupStore);
        var settings = (JsonSerializerSettings)typeof(BufferBackupStore)
            .GetField("serializerSettings", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(backupStore)!;

        Assert.Equal(expectedDirectory, baseDirectory);
        Assert.Equal(expectedTypeNameHandling, settings.TypeNameHandling);
    }
}
