using System;
using Microsoft.Extensions.DependencyInjection;
using SteroidsDI.Core;

namespace SteroidsDI.Tests.Cases
{
    /// <summary>
    /// Base test that provides ability to work with scopes. Each test method in
    /// derived classes can get query DI container via <see cref="GetService"/> method,
    /// see <see cref="ScopedTestDerived"/> class.
    /// </summary>
    public class ScopedTestBase : IDisposable
    {
        private readonly ServiceProvider _rootProvider;
        private readonly Scoped<ScopedTestBase> _scoped;

        public ScopedTestBase()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            _rootProvider = services.BuildServiceProvider();
            _scoped = new Scoped<ScopedTestBase>(_rootProvider.GetRequiredService<IScopeFactory>());
        }

        // override this method to specify required services for unit tests
        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddDefer();
            services.AddGenericScope<ScopedTestBase>();
        }

        protected T GetService<T>() => ((IServiceScope)_scoped.Scope).ServiceProvider.GetService<T>();

        protected T GetRequiredService<T>() => ((IServiceScope)_scoped.Scope).ServiceProvider.GetRequiredService<T>();

        public void Dispose()
        {
            _scoped.Dispose();
            _rootProvider.Dispose();
        }
    }
}
