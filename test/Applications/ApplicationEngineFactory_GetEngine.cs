using System.Reflection;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngineFactory_GetEngine
{
    [Fact(DisplayName = "No762_正常系：正常動作の確認_メソッドを呼び出す")]
    public void Case762()
    {
        var expected = typeof(ApplicationEngineFactory)
            .GetField("applicationEngine", BindingFlags.Static | BindingFlags.NonPublic)!
            .GetValue(null);

        var result = ApplicationEngineFactory.GetEngine();

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IApplicationEngine>(result);
        Assert.Same(expected, result);
    }

    [Fact(DisplayName = "No763_正常系：正常動作の確認_メソッドを2回連続で呼び出す")]
    public void Case763()
    {
        var first = ApplicationEngineFactory.GetEngine();
        var second = ApplicationEngineFactory.GetEngine();

        Assert.Same(first, second);
    }
}
