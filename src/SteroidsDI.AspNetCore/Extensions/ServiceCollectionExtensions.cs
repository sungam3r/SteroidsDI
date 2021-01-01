using Microsoft.Extensions.DependencyInjection.Extensions;
using SteroidsDI.AspNetCore;
using SteroidsDI.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary> Extension methods for <see cref="IServiceCollection"/>. </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary> Register <see cref="AspNetCoreHttpScopeProvider" /> in DI as one of the possible implementations for <see cref="IScopeProvider" />. </summary>
        /// <param name="services"> A collection of DI container services. </param>
        /// <returns> Reference to the passed object <paramref name="services" /> to be able to call methods in a chain. </returns>
        public static IServiceCollection AddHttpScope(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IScopeProvider, AspNetCoreHttpScopeProvider>());
            return services;
        }
    }
}
