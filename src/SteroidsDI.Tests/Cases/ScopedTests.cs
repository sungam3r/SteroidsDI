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
    public void Should_Not_Throw_If_Null_Scope()
    {
        using var s1 = new Scoped<int>(new NullScopeFactory());
        s1.Scope.ShouldBeNull();

        using var s2 = new Scoped(typeof(int), new NullScopeFactory());
        s2.Scope.ShouldBeNull();
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
}
