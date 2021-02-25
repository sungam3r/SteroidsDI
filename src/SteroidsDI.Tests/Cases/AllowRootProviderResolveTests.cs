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
                    var service = scope.ServiceProvider.GetService<Service>()!;
                    service.Scoped.Value.ShouldNotBeNull();
                }
            }
        }

        [Test]
        public void Should_Throw_When_No_Scopes_And_AllowRootProviderResolve_Disabled()
        {
            var services = new ServiceCollection()
               //.Configure<ServiceProviderAdvancedOptions>(opt => opt.AllowRootProviderResolve = true) // false by default
               .AddDefer()
               .AddSingleton<ScopedAsSingleton>()
               .AddSingleton<Service>();

            using (var provider = services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<Service>()!;
                    Should.Throw<InvalidOperationException>(() => service.Scoped.Value).Message.ShouldBe(@"The current scope is missing. Unable to get object of type 'ScopedAsSingleton' from the root provider.
Be sure to add the required provider (IScopeProvider) to the container using the TryAddEnumerable method or a special method from your transport library.
An object can be obtained from the root provider if it has a non-scoped lifetime and the parameter 'ServiceProviderAdvancedOptions.AllowRootProviderResolve' = true.");
                }
            }
        }
    }
}
