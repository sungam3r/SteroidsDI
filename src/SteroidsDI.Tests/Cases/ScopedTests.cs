using System;
using NUnit.Framework;
using Shouldly;
using SteroidsDI.Core;

namespace SteroidsDI.Tests.Cases
{
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
    }
}
