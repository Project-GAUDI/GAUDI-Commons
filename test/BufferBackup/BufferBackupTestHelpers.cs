namespace TICO.GAUDI.Commons.Test;

/// <summary>テスト専用の一時ディレクトリを作成し、破棄時に削除する。</summary>
internal sealed class TempDirectory : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"BufferBackupStoreTest_{Guid.NewGuid():N}");

    public TempDirectory()
    {
        Directory.CreateDirectory(Path);
    }

    public void Dispose()
    {
        if (Directory.Exists(Path))
        {
            Directory.Delete(Path, true);
        }
    }
}

/// <summary>環境変数を一時的に変更し、破棄時に元の値へ戻す。</summary>
internal sealed class EnvironmentVariableScope : IDisposable
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
