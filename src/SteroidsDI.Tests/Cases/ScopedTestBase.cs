using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SteroidsDI.Core;

namespace SteroidsDI.Tests.Cases;

/// <summary>
/// Base test that provides ability to work with scopes. Each test method in
/// derived classes can get query DI container via <see cref="GetService"/> method,
/// see <see cref="ScopedTestDerived"/> class.
/// <br/>
/// Note that you should add [FixtureLifeCycle(LifeCycle.InstancePerTestCase)] to your
/// test class to make each test method work with its own unique scope created in ctor.
/// </summary>
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class ScopedTestBase : IDisposable
{
    private readonly ServiceProvider _rootProvider;
    private readonly Scoped _scoped;

    public ScopedTestBase()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        _rootProvider = services.BuildServiceProvider();
        _scoped = new Scoped(GetType(), _rootProvider.GetRequiredService<IScopeFactory>()); // GetType instead of <ScopedTestBase> to do not mix the same scope in case of parallel tests
    }

    // override this method to specify required services for unit tests
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        _ = services
            .AddDefer()
            .AddGenericScope(GetType()); // GetType instead of <ScopedTestBase> to do not mix the same scope in case of parallel tests
    }

    protected T? GetService<T>() => ((IServiceScope)_scoped.Scope).ServiceProvider.GetService<T>();

    protected T GetRequiredService<T>() where T : notnull
        => ((IServiceScope)_scoped.Scope).ServiceProvider.GetRequiredService<T>();

    public void Dispose()
    {
        _scoped.Dispose();
        _rootProvider.Dispose();
        GC.SuppressFinalize(this);
    }
}
