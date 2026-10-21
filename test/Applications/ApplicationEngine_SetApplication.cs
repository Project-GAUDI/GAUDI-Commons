using System.Reflection;
using Moq;
using TICO.GAUDI.Commons;
using Xunit;

namespace TICO.GAUDI.Commons.Test;

public sealed class ApplicationEngine_SetApplication
{
    [Fact(DisplayName = "No649_正常系：正常動作の確認_applicationMainに任意のIApplicationMain実装を指定して、メソッドを呼び出す")]
    public void Case649()
    {
        var engine = new ApplicationEngine();
        var app = new Mock<IApplicationMain>().Object;

        engine.SetApplication(app);

        Assert.Same(app, GetApplicationMain(engine));
    }

    [Fact(DisplayName = "No650_正常系：正常動作の確認_applicationMainにnullを指定して、メソッドを呼び出す")]
    public void Case650()
    {
        var engine = new ApplicationEngine();
        engine.SetApplication(new Mock<IApplicationMain>().Object);

        engine.SetApplication(null!);

        Assert.Null(GetApplicationMain(engine));
    }

    private static object? GetApplicationMain(ApplicationEngine engine)
    {
        return typeof(ApplicationEngine).GetField("applicationMain", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(engine);
    }
}
