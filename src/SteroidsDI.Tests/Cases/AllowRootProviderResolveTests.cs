using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace SteroidsDI.Tests.Cases;

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

        using (var rootProvider = services.BuildServiceProvider())
        {
            using (var scope = rootProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<Service>()!;
                service.Scoped.Value.ShouldNotBeNull();
            }
        }
    }

    [Test]
    [Category("Throw")]
    public void Should_Throw_When_No_Scopes_And_AllowRootProviderResolve_Disabled()
    {
        var services = new ServiceCollection()
           //.Configure<ServiceProviderAdvancedOptions>(opt => opt.AllowRootProviderResolve = true) // false by default
           .AddDefer()
           .AddSingleton<ScopedAsSingleton>()
           .AddSingleton<Service>();

        using (var rootProvider = services.BuildServiceProvider())
        {
            using (var scope = rootProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<Service>()!;
                Should.Throw<InvalidOperationException>(() => service.Scoped.Value).Message.ShouldBe(@"The current scope is missing. Unable to resolve service 'ScopedAsSingleton' from the root service provider.
Be sure to add the required provider (IScopeProvider) to the DI container by using appropriate extension method.
Note that a service can be obtained from the root service provider only if it has a non-scoped lifetime (i.e. singleton or transient) and 'ServiceProviderAdvancedOptions.AllowRootProviderResolve' option is enabled.");
            }
        }
    }

    [Test]
    [Category("Throw")]
    public void Should_Throw_When_No_Scopes_And_No_Service_Registered_And_AllowRootProviderResolve_Enabled()
    {
        var services = new ServiceCollection()
           .Configure<ServiceProviderAdvancedOptions>(opt => opt.AllowRootProviderResolve = true)
           .AddDefer()
           .AddSingleton<Service>();

        using (var rootProvider = services.BuildServiceProvider())
        {
            using (var scope = rootProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<Service>()!;
                Should.Throw<InvalidOperationException>(() => service.Scoped.Value).Message.ShouldBe("No service for type 'SteroidsDI.Tests.Cases.AllowRootProviderResolveTests+ScopedAsSingleton' has been registered.");
            }
        }
    }
}
