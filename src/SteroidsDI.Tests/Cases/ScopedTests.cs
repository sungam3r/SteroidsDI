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
    }

    [Test]
    [Category("Throw")]
    public void Should_Throw_If_Current_Scope_Exist()
    {
        GenericScope<int>.CurrentScope = new NoopScope();
        Should.Throw<InvalidOperationException>(() => new Scoped<int>(new NoopScopeFactory())).Message.ShouldBe($"The current scope of GenericScope<Int32> is not null when trying to initialize it.");
    }

    [Test]
    public void Should_Not_Throw_If_Null_Scope()
    {
        using var s = new Scoped<int>(new NullScopeFactory());
        s.Scope.ShouldBeNull();
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
