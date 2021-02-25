using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SteroidsDI.Core;

namespace SteroidsDI
{
    internal static class Resolver
    {
        internal static TService ResolveByNamedBinding<TService>(this IServiceProvider provider, object name, IEnumerable<NamedBinding> bindings, ServiceProviderAdvancedOptions options)
        {
            var binding = bindings.Where(b => b.ServiceType == typeof(TService)).SingleOrDefault(b => b.Name.Equals(name));
            if (binding == null)
                throw new InvalidOperationException($"Destination type not found for named binding '{name}' to type '{typeof(TService)}'. Verify that a named binding is specified in the DI container.");

            return (TService)provider.Resolve(binding.ImplementationType, options);
        }

        internal static TService Resolve<TService>(this IServiceProvider provider, ServiceProviderAdvancedOptions options) => (TService)provider.Resolve(typeof(TService), options);

        internal static object Resolve(this IServiceProvider provider, Type type, ServiceProviderAdvancedOptions options)
        {
            // An application can have several entry points that initialize their scopes.
            var scopeProviders = provider.GetRequiredService<IEnumerable<IScopeProvider>>();

            // If at any given time there is one of the scopes, then we use it.
            foreach (var scopeProvider in scopeProviders)
            {
                var scopedServiceProvider = scopeProvider.GetScopedServiceProvider(provider);

                // The first scope found is used. Generally speaking, a situation where several providers can return a non null scope at the same time is unlikely.
                // Providers characterize some entry point of the application, which processes requests from the outside. It can be, for example, an HTTP server or
                // a message broker listener (RabbitMQ, Kafka, etc). Such entry points, by the nature are isolated from each other and do not interfere. Theoretically,
                // it’s possible (intentionally or by mistake) to create such a situation, when nevertheless more than 1 scope will exist simultaneously. In this case,
                // the possibility of validation is added.
                if (scopedServiceProvider != null)
                {
                    if (options.ValidateParallelScopes)
                    {
                        // The exception of scopeProvider and then adding it to the set is made in order not to call GetScopedServiceProvider() on it again, which
                        // theoretically, can be costly and/or cause side effects (in practice this should not be, but there are concerns).
                        var parallelScopeProviders = scopeProviders.Except(new[] { scopeProvider }).Where(p => p.GetScopedServiceProvider(provider) != null).ToList();
                        if (parallelScopeProviders.Count > 0)
                        {
                            parallelScopeProviders.Add(scopeProvider);
                            throw new InvalidOperationException($@"When '{nameof(ServiceProviderAdvancedOptions)}.{nameof(ServiceProviderAdvancedOptions.ValidateParallelScopes)}' option is turned on, the simultaneous existence of several scopes from different providers was detected.
Scopes obtained from the following providers: {string.Join(", ", parallelScopeProviders.Select(p => p.ToString()))}");
                        }
                    }

                    return scopedServiceProvider.GetRequiredService(type);
                }
            }

            // With this type, there can be several descriptors in the container (or none), in this case we check the latter.
            // MSDI has such behavior of Last Win when resolving dependencies, do the same here.
            bool scopeRequired = options.Services.LastOrDefault(s => s.ServiceType == type)?.Lifetime == ServiceLifetime.Scoped;
            if (scopeRequired)
            {
                throw new InvalidOperationException($@"An error occurred while resolving the type '{type.Name}'
The type is declared as scoped within the context of the request, but an attempt to resolve the type is made outside the context of the request.
An application can simultaneously have several entry points that initialize their request contexts.
Be sure to add the required provider (IScopeProvider) to the container using the TryAddEnumerable method.");
            }

            // In the absence of scopes and the possibility of obtaining TService from the root provider, use it to resolve the dependency
            if (options.AllowRootProviderResolve)
                return provider.GetRequiredService(type);

            throw new InvalidOperationException($@"The current scope is missing. Unable to get object of type '{type.Name}' from the root provider.
Be sure to add the required provider (IScopeProvider) to the container using the TryAddEnumerable method or a special method from your transport library.
An object can be obtained from the root provider if it has a non-scoped lifetime and the parameter '{nameof(ServiceProviderAdvancedOptions)}.{nameof(ServiceProviderAdvancedOptions.AllowRootProviderResolve)}' = true.");
        }
    }
}
