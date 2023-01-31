using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using SteroidsDI.Core;

namespace SteroidsDI.Tests.Cases;

[TestFixture]
public class ValidateParallelScopesTests
{
    private sealed class A { }

    private sealed class B { }

    private sealed class Scoped { }

    private class Service
    {
        public Service(Defer<Scoped> scoped)
        {
            Scoped = scoped;
        }

        public Defer<Scoped> Scoped { get; }
    }

    private sealed class TestScope : IDisposable, IServiceScope, IServiceProvider
    {
        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
        }

        public object GetService(Type serviceType) => Activator.CreateInstance(serviceType)!;
    }

    [Test]
    [Category("Throw")]
    public void Should_Throw_When_No_Scopes()
    {
        var services = new ServiceCollection()
           .AddDefer()
           .AddGenericScope<A>()
           .AddGenericScope<B>()
           .AddScoped<Scoped>()
           .AddSingleton<Service>();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetService<Service>()!;

        Should.Throw<InvalidOperationException>(() => service.Scoped.Value).Message.ShouldBe(@"An error occurred while resolving service 'Scoped'.
The service was registered as scoped, but an attempt to resolve this service is made outside of any scope.
An application can simultaneously have several entry points that initialize their scopes.
Be sure to add the required provider (IScopeProvider) to the DI container by using appropriate extension method.");
    }

    [Test]
    public void Should_Work_When_More_Than_One_Scope_And_ValidateParallelScopes_Disabled()
    {
        var services = new ServiceCollection()
           //.Configure<ServiceProviderAdvancedOptions>(opt => opt.ValidateParallelScopes = false) // false by default
           .AddDefer()
           .AddGenericScope<A>()
           .AddGenericScope<B>()
           .AddScoped<Scoped>()
           .AddSingleton<Service>();

        GenericScope<A>.CurrentScope = new TestScope();
        GenericScope<B>.CurrentScope = new TestScope();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetService<Service>()!;
        service.Scoped.Value.ShouldNotBeNull();
    }

    [Test]
    public void Should_Work_When_Only_One_Scope_And_ValidateParallelScopes_Enabled()
    {
        var services = new ServiceCollection()
           .Configure<ServiceProviderAdvancedOptions>(opt => opt.ValidateParallelScopes = true)
           .AddDefer()
           .AddGenericScope<A>()
           .AddGenericScope<B>()
           .AddScoped<Scoped>()
           .AddSingleton<Service>();

        GenericScope<A>.CurrentScope = new TestScope();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetService<Service>()!;
        service.Scoped.Value.ShouldNotBeNull();
    }

    [Test]
    [Category("Throw")]
    public void Should_Throw_When_More_Than_One_Scope_And_ValidateParallelScopes_Enabled()
    {
        var services = new ServiceCollection()
           .Configure<ServiceProviderAdvancedOptions>(opt => opt.ValidateParallelScopes = true)
           .AddDefer()
           .AddGenericScope<A>()
           .AddGenericScope<B>()
           .AddScoped<Scoped>()
           .AddSingleton<Service>();

        GenericScope<A>.CurrentScope = new TestScope();
        GenericScope<B>.CurrentScope = new TestScope();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetService<Service>()!;
        Should.Throw<InvalidOperationException>(() => service.Scoped.Value).Message.ShouldBe(@"'ServiceProviderAdvancedOptions.ValidateParallelScopes' option is ON. The simultaneous existence of several scopes from different providers was detected.
Scopes were obtained from the following providers: SteroidsDI.GenericScopeProvider`1[SteroidsDI.Tests.Cases.ValidateParallelScopesTests+B], SteroidsDI.GenericScopeProvider`1[SteroidsDI.Tests.Cases.ValidateParallelScopesTests+A]");
    }
}
