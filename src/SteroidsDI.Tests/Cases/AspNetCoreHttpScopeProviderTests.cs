using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using SteroidsDI.AspNetCore;

namespace SteroidsDI.Tests.Cases
{
    [TestFixture]
    internal class AspNetCoreHttpScopeProviderTests
    {
        [Test]
        [Category("Throw")]
        public void Should_Throw_If_Called_With_Null_Provider()
        {
            Should.Throw<ArgumentNullException>(() => new AspNetCoreHttpScopeProvider().GetScopedServiceProvider(null!).ShouldBe(null));
        }

        [Test]
        public void Should_Return_Null_If_Called_Out_Of_HttpContext()
        {
            var provider = new ServiceCollection().AddHttpScope().BuildServiceProvider();
            new AspNetCoreHttpScopeProvider().GetScopedServiceProvider(provider).ShouldBe(null);
        }
    }
}
