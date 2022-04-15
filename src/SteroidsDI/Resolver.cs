using Microsoft.Extensions.DependencyInjection;
using SteroidsDI.Core;

namespace SteroidsDI;

internal static class Resolver
{
    internal static TService ResolveByNamedBinding<TService>(this IServiceProvider provider, object name, IEnumerable<NamedBinding> bindings, ServiceProviderAdvancedOptions options)
    {
        var binding = bindings.Where(b => b.ServiceType == typeof(TService)).SingleOrDefault(b => b.Name.Equals(name));
        return binding == null
            ? throw new InvalidOperationException($"Destination type not found for named binding '{name}' to type '{typeof(TService)}'. Verify that a named binding is specified in the DI container.")
            : (TService)provider.Resolve(binding.ImplementationType, options);
    }

    internal static TService Resolve<TService>(this IServiceProvider rootProvider, ServiceProviderAdvancedOptions options)
        => (TService)rootProvider.Resolve(typeof(TService), options);

    // The core part of the whole project that resolves services from appropriate provider.
    internal static object Resolve(this IServiceProvider rootProvider, Type type, ServiceProviderAdvancedOptions options)
    {
        // An application can have several entry points that initialize their scopes.
        var scopeProviders = rootProvider.GetRequiredService<IEnumerable<IScopeProvider>>();

        // If at any given time there is one of the scopes, then we use it.
        foreach (var scopeProvider in scopeProviders)
        {
            var scopedServiceProvider = scopeProvider.GetScopedServiceProvider(rootProvider);

            // The first scope found is used. Generally speaking, a situation where several providers can return
            // a non null scope at the same time is unlikely (but may arise). Providers characterize some entry
            // point of the application, which processes requests from the outside. It can be, for example,
            // an HTTP server or a message broker listener (RabbitMQ, Kafka, etc). Such entry points, by the
            // nature are isolated from each other and do not interfere. Nevertheless, theoretically, it’s possible
            // (intentionally or by mistake) to create such a situation, when more than 1 scope will exist
            // simultaneously. In this case, the possibility of validation is added.
            if (scopedServiceProvider != null)
            {
                if (options.ValidateParallelScopes)
                {
                    // The exclusion of scopeProvider and then adding it to the list is made in order not to
                    // call GetScopedServiceProvider() on it again, which theoretically can be costly and/or
                    // cause side effects (in practice this should not be, but there are concerns).
                    var parallelScopeProviders = scopeProviders.Except(new[] { scopeProvider }).Where(p => p.GetScopedServiceProvider(rootProvider) != null).ToList();
                    if (parallelScopeProviders.Count > 0)
                    {
                        parallelScopeProviders.Add(scopeProvider);
                        throw new InvalidOperationException($@"'{nameof(ServiceProviderAdvancedOptions)}.{nameof(ServiceProviderAdvancedOptions.ValidateParallelScopes)}' option is ON. The simultaneous existence of several scopes from different providers was detected.
Scopes were obtained from the following providers: {string.Join(", ", parallelScopeProviders.Select(p => p.ToString()))}");
                    }
                }

                try
                {
                    return scopedServiceProvider.GetRequiredService(type);
                }
                catch (ObjectDisposedException e) when (options.UseFriendlyObjectDisposedException)
                {
                    throw new ObjectDisposedException($@"ObjectDisposedException occurred while resolving service '{type.Name}' by scoped service provider obtained from '{scopeProvider.GetType().FullName}'.
Most likely this happened because the scope and scoped provider were disposed BEFORE the actual completion of the user code.
Often, this is due to the forgotten 'await' operator somewhere in the user code. Make sure you await all the created tasks correctly.
You see this message because 'ServiceProviderAdvancedOptions.UseFriendlyObjectDisposedException' option is enabled.", e);
                }
            }
        }

        // If we got here, then no provider could provide a scope. Either no provider
        // is registered or resolve is called out the context of each provider.

        // There can be several descriptors with the same service type in the DI container (or none, of course).
        // In this case we check the latter. MSDI has such behavior of Last Win when resolving dependencies, do the same here.
        bool scopeRequired = options.Services.LastOrDefault(s => s.ServiceType == type)?.Lifetime == ServiceLifetime.Scoped;
        if (scopeRequired)
        {
            throw new InvalidOperationException($@"An error occurred while resolving service '{type.Name}'.
The service was registered as scoped, but an attempt to resolve this service is made outside of any scope.
An application can simultaneously have several entry points that initialize their scopes.
Be sure to add the required provider (IScopeProvider) to the DI container by using appropriate extension method.");
        }

        // In the absence of any available scope use root service provider to resolve
        // transient or singleton service, if this is allowed by the options.
        return options.AllowRootProviderResolve
            ? rootProvider.GetRequiredService(type)
            : throw new InvalidOperationException($@"The current scope is missing. Unable to resolve service '{type.Name}' from the root service provider.
Be sure to add the required provider (IScopeProvider) to the DI container by using appropriate extension method.
Note that a service can be obtained from the root service provider only if it has a non-scoped lifetime (i.e. singleton or transient) and '{nameof(ServiceProviderAdvancedOptions)}.{nameof(ServiceProviderAdvancedOptions.AllowRootProviderResolve)}' option is enabled.");
    }
}
