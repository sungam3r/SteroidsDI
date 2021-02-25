using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace SteroidsDI.Tests.Cases
{
    [TestFixture]
    public class AllowRootProviderResolveTests
    {
        private sealed class ScopedAsSingleton { }

        private class Service
        {
            public Service(Defer<ScopedAsSingleton> scoped)
            {
                Scoped = scoped;
            }

            public Defer<ScopedAsSingleton> Scoped { get; }
        }

        [Test]
        public void Should_Work_When_No_Scopes_And_AllowRootProviderResolve_Enabled()
        {
            var services = new ServiceCollection()
               .Configure<ServiceProviderAdvancedOptions>(opt => opt.AllowRootProviderResolve = true)
               .AddDefer()
               .AddSingleton<ScopedAsSingleton>()
               .AddSingleton<Service>();

            using (var provider = services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<Service>();
                    service.Scoped.Value.ShouldNotBeNull();
                }
            }
        }
    }
}
