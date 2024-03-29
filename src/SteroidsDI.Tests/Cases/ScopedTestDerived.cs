using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace SteroidsDI.Tests.Cases;

public class ScopedTestDerived : ScopedTestBase
{
    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddSingleton<IRepo, Repo>();
        services.AddScoped<Dependency>();
    }

    /// <summary>
    /// This method works in context of prepared unique DI scope like ASP.NET Core app does.
    /// </summary>
    [Test]
    public void Scoped_Should_Work_1()
    {
        var repo = GetRequiredService<IRepo>();
        repo.Name.ShouldBe("12345");
    }

    /// <summary>
    /// This method works in context of prepared unique DI scope like ASP.NET Core app does.
    /// </summary>
    [Test]
    public void Scoped_Should_Work_2()
    {
        var repo = GetRequiredService<IRepo>();
        repo.Name.ShouldBe("12345");
    }

    /// <summary>
    /// This method works in context of prepared unique scope like ASP.NET Core app does.
    /// </summary>
    [Test]
    public void Scoped_Should_Work_3()
    {
        var repo = GetRequiredService<IRepo>();
        repo.Name.ShouldBe("12345");
    }
}

public interface IRepo
{
    string Name { get; }
}

public class Repo : IRepo
{
    private readonly Defer<Dependency> _dep;

    public Repo(Defer<Dependency> dep)
    {
        _dep = dep;
    }

    public string Name => _dep.Value.Name;
}

public class Dependency
{
    public virtual string Name => "12345";
}
