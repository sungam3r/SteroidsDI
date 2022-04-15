using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using SteroidsDI.Core;

namespace SteroidsDI.Tests.Cases;

internal class UseFriendlyObjectDisposedExceptionTests
{
    [Test]
    public void ObjectDisposedException_UseFriendlyObjectDisposedException_Enabled()
    {
        var services = new ServiceCollection()
            .AddDefer()
            .AddScoped<ScopedService>()
            .AddSingleton<SingletonService>()
            .AddGenericScope<UseFriendlyObjectDisposedExceptionTests>();

        using (var rootProvider = services.BuildServiceProvider())
        {
            var service = rootProvider.GetRequiredService<SingletonService>();
            var outerScope = new Scoped<UseFriendlyObjectDisposedExceptionTests>(rootProvider.GetRequiredService<IScopeFactory>());

            var cts1 = new CancellationTokenSource();
            var cts2 = new CancellationTokenSource();

            var t = Task.Run(() =>
            {
                cts1.Cancel();
                cts2.Token.WaitHandle.WaitOne();

                var ex = Should.Throw<ObjectDisposedException>(() => service.Scoped.Value);
                ex.Message.ShouldBe(@"ObjectDisposedException occurred while resolving service 'ScopedService' by scoped service provider obtained from 'SteroidsDI.GenericScopeProvider`1[[SteroidsDI.Tests.Cases.UseFriendlyObjectDisposedExceptionTests, SteroidsDI.Tests, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null]]'.
Most likely this happened because the scope and scoped provider were disposed BEFORE the actual completion of the user code.
Often, this is due to a forgotten 'await' operator somewhere in the user code. Make sure you await all the created tasks correctly.
You see this message because 'ServiceProviderAdvancedOptions.UseFriendlyObjectDisposedException' option is enabled.");
                ex.InnerException.ShouldBeOfType<ObjectDisposedException>().Message.ShouldBe(@"Cannot access a disposed object.
Object name: 'IServiceProvider'.");
            });

            cts1.Token.WaitHandle.WaitOne();
            outerScope.Dispose();
            cts2.Cancel();
            t.Wait();
        }
    }

    [Test]
    public void ObjectDisposedException_UseFriendlyObjectDisposedException_Disabled()
    {
        var services = new ServiceCollection()
            .Configure<ServiceProviderAdvancedOptions>(opt => opt.UseFriendlyObjectDisposedException = false)
            .AddDefer()
            .AddScoped<ScopedService>()
            .AddSingleton<SingletonService>()
            .AddGenericScope<UseFriendlyObjectDisposedExceptionTests>();

        using (var rootProvider = services.BuildServiceProvider())
        {
            var service = rootProvider.GetRequiredService<SingletonService>();
            var outerScope = new Scoped<UseFriendlyObjectDisposedExceptionTests>(rootProvider.GetRequiredService<IScopeFactory>());

            var cts1 = new CancellationTokenSource();
            var cts2 = new CancellationTokenSource();

            var t = Task.Run(() =>
            {
                cts1.Cancel();
                cts2.Token.WaitHandle.WaitOne();

                var ex = Should.Throw<ObjectDisposedException>(() => service.Scoped.Value);
                ex.Message.ShouldBe(@"Cannot access a disposed object.
Object name: 'IServiceProvider'.");
                ex.InnerException.ShouldBeNull();
            });

            cts1.Token.WaitHandle.WaitOne();
            outerScope.Dispose();
            cts2.Cancel();
            t.Wait();
        }
    }

    private sealed class ScopedService { }

    private class SingletonService
    {
        public SingletonService(IDefer<ScopedService> scoped)
        {
            Scoped = scoped;
        }

        public IDefer<ScopedService> Scoped { get; }
    }
}
