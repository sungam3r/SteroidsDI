using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using SteroidsDI.Core;

namespace SteroidsDI.Tests.Cases;

[TestFixture]
public class ScopedTests
{
    [Test]
    [Category("Throw")]
    public void Should_Throw_If_Null()
    {
        Should.Throw<ArgumentNullException>(() => new Scoped<int>(null!)).ParamName.ShouldBe("scopeFactory");
        Should.Throw<ArgumentNullException>(() => new Scoped(null!, null!)).ParamName.ShouldBe("scopeFactory");
    }

    [Test]
    [Category("Throw")]
    public void Should_Throw_If_Current_Scope_Exist()
    {
        GenericScope<int>.CurrentScope = new NoopScope();
        Should.Throw<InvalidOperationException>(() => new Scoped<int>(new NoopScopeFactory())).Message.ShouldBe($"The current scope of GenericScope<Int32> is not null when trying to initialize it.");
        Should.Throw<InvalidOperationException>(() => new Scoped(typeof(int), new NoopScopeFactory())).Message.ShouldBe($"The current scope of GenericScope<Int32> is not null when trying to initialize it.");
    }

    [Test]
    public async Task Should_Not_Throw_If_Null_Scope()
    {
        using var s1 = new Scoped<int>(new NullScopeFactory());
        s1.Scope.ShouldBeNull();

        using var s2 = new Scoped(typeof(int), new NullScopeFactory());
        s2.Scope.ShouldBeNull();

        await using var s3 = new Scoped<int>(new NullScopeFactory());
        s1.Scope.ShouldBeNull();

        await using var s4 = new Scoped(typeof(int), new NullScopeFactory());
        s2.Scope.ShouldBeNull();
    }

    [Test]
    public async Task Should_Support_Custom_Scope_That_Does_Not_Implement_IAsyncDisposable()
    {
        await using (new Scoped<bool>(new NoopScopeFactory()))
        {
        }

        await using (new Scoped(typeof(bool), new NoopScopeFactory()))
        {
        }
    }

    [Test]
    public async Task Should_Support_IAsyncDisposable()
    {
        var services = new ServiceCollection()
            .AddGenericScope<bool>()
            .AddMicrosoftScopeFactory()
            .AddDefer()
            .AddSingleton<Singleton>()
            .AddScoped<IFoo, Foo>();

        await using var provider = services.BuildServiceProvider();

        var singleton = provider.GetRequiredService<Singleton>();

        await using (new Scoped<bool>(provider.GetRequiredService<IScopeFactory>()))
        {
            var foo = singleton.Foo.Value;

            foo.Count.ShouldBe(0);
            foo.Count++;
            foo.Count.ShouldBe(1);

            foo = singleton.Foo.Value;

            foo.Count.ShouldBe(1); // the same instance with counter eq 1
        }

        await using (new Scoped<bool>(provider.GetRequiredService<IScopeFactory>()))
        {
            var foo = singleton.Foo.Value;

            foo.Count.ShouldBe(0); // counter eq 0 because we are in a new scope
            foo.Count++;
            foo.Count.ShouldBe(1);

            foo = singleton.Foo.Value;

            foo.Count.ShouldBe(1); // the same instance with counter eq 1
        }
    }

    [Test]
    public async Task Should_Support_IAsyncDisposable_NonGeneric()
    {
        var services = new ServiceCollection()
            .AddGenericScope<bool>()
            .AddMicrosoftScopeFactory()
            .AddDefer()
            .AddSingleton<Singleton>()
            .AddScoped<IFoo, Foo>();

        await using var provider = services.BuildServiceProvider();

        var singleton = provider.GetRequiredService<Singleton>();

        await using (new Scoped(typeof(bool), provider.GetRequiredService<IScopeFactory>()))
        {
            var foo = singleton.Foo.Value;

            foo.Count.ShouldBe(0);
            foo.Count++;
            foo.Count.ShouldBe(1);

            foo = singleton.Foo.Value;

            foo.Count.ShouldBe(1); // the same instance with counter eq 1
        }

        await using (new Scoped(typeof(bool), provider.GetRequiredService<IScopeFactory>()))
        {
            var foo = singleton.Foo.Value;

            foo.Count.ShouldBe(0); // counter eq 0 because we are in a new scope
            foo.Count++;
            foo.Count.ShouldBe(1);

            foo = singleton.Foo.Value;

            foo.Count.ShouldBe(1); // the same instance with counter eq 1
        }
    }

    private sealed class NoopScope : IDisposable
    {
        public void Dispose()
        {
        }
    }

    private sealed class NoopScopeFactory : IScopeFactory
    {
        public IDisposable CreateScope() => new NoopScope();
    }

    private sealed class NullScopeFactory : IScopeFactory
    {
        public IDisposable CreateScope() => null!;
    }

    private class Singleton
    {
        public Singleton(Defer<IFoo> foo)
        {
            Foo = foo;
        }

        public Defer<IFoo> Foo { get; }
    }

    private interface IFoo
    {
        int Count { get; set; }
    }

    private class Foo : IFoo, IAsyncDisposable
    {
        public int Count { get; set; }

        public ValueTask DisposeAsync() => default;
    }
}
