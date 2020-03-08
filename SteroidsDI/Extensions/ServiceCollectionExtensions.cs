using Microsoft.Extensions.DependencyInjection.Extensions;
using SteroidsDI;
using SteroidsDI.Core;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary> Extension methods for <see cref="IServiceCollection"/>. </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary> Gets the binding context for the type <typeparamref name="TService" />. </summary>
        /// <typeparam name="TService"> The service type which context is customized. </typeparam>
        /// <param name="services"> A collection of DI container services. </param>
        /// <returns> Binding context. </returns>
        public static BindingContext<TService> For<TService>(this IServiceCollection services)
            where TService : class => new BindingContext<TService>(services);

        /// <summary> Add the specified type <paramref name="factoryType" /> to the DI container as a factory that performs factory methods for creating objects. </summary>
        /// <param name="services"> A collection of DI container services. </param>
        /// <param name="factoryType"> Factory type. </param>
        /// <returns> Reference to the passed object <paramref name="services" /> to be able to call methods in a chain. </returns>
        public static IServiceCollection AddFactory(this IServiceCollection services, Type factoryType)
        {
            if (factoryType == null)
                throw new ArgumentNullException(nameof(factoryType));

            services.AddServiceProviderAdvancedOptions(_ => { });
            services.TryAddSingleton(factoryType, FactoryGenerator.Generate(factoryType));
            return services;
        }

        /// <summary> Add the specified type <typeparamref name="TFactory"/> to the DI container as a factory that performs factory methods for creating objects. </summary>
        /// <typeparam name="TFactory"> Factory type. </typeparam>
        /// <param name="services"> A collection of DI container services. </param>
        /// <returns> Reference to the passed object <paramref name="services" /> to be able to call methods in a chain. </returns>
        public static IServiceCollection AddFactory<TFactory>(this IServiceCollection services) => services.AddFactory(typeof(TFactory));

        /// <summary>
        /// Register the factory <see cref="Func {TService}" /> to create an object of type <typeparamref name="TService" />.
        /// This factory can find/select the correct scope (if one exists at all) through which you need to get the required object.
        /// Differs from <see cref="AddDefer(IServiceCollection)" >AddDefer</see > by the fact that it works for only one specified
        /// type, that is, this method may need to be called several times.
        /// </summary>
        /// <typeparam name="TService"> Service type. </typeparam>
        /// <param name="services"> A collection of DI container services. </param>
        /// <returns> Reference to the passed object <paramref name="services" /> to be able to call methods in a chain. </returns>
        public static IServiceCollection AddFunc<TService>(this IServiceCollection services)
            => services.AddServiceProviderAdvancedOptions(_ => { })
                       .AddSingleton(provider =>
                        {
                            var options = provider.GetRequiredService<ServiceProviderAdvancedOptions>();
                            return new Func<TService>(() => provider.Resolve<TService>(options));
                        });

        /// <summary>
        /// Adds support for <see cref="Defer{T}" /> - deferring resolving the object in the desired scope. The effect is completely
        /// similar to one from <see cref="AddFunc{TService}(IServiceCollection)">AddFunc</see> with the difference that this method
        /// works immediately for all objects registered in the DI container.
        /// </summary>
        /// <param name="services"> A collection of DI container services. </param>
        /// <returns> Reference to the passed object <paramref name="services" /> to be able to call methods in a chain. </returns>
        public static IServiceCollection AddDefer(this IServiceCollection services) => services.AddDefer(_ => { });

        /// <summary>
        /// Adds support for <see cref="Defer{T}" /> - deferring resolving the object in the desired scope. The effect is completely
        /// similar to one from <see cref="AddFunc{TService}(IServiceCollection)">AddFunc</see> with the difference that this method
        /// works immediately for all objects registered in the DI container.
        /// </summary>
        /// <param name="services"> A collection of DI container services. </param>
        /// <param name="configure"> Delegate to configure DI options. </param>
        /// <returns> Reference to the passed object <paramref name="services" /> to be able to call methods in a chain. </returns>
        public static IServiceCollection AddDefer(this IServiceCollection services, Action<ServiceProviderAdvancedOptions> configure)
            => services.AddServiceProviderAdvancedOptions(configure)
                       .AddSingleton(typeof(Defer<>), typeof(DelegatedDefer<>));

        /// <summary> Register <see cref="GenericScopeProvider{T}" />  in DI as one of the possible implementations of <see cref="IScopeProvider" />. </summary>
        /// <typeparam name="T">
        /// An arbitrary type that is used to create various static AsyncLocal fields. The caller may set unique
        /// closed type, thereby providing its own storage, to which only it will have access.
        /// </typeparam>
        /// <param name="services"> A collection of DI container services. </param>
        /// <returns> Reference to the passed object <paramref name="services" /> to be able to call methods in a chain. </returns>
        public static IServiceCollection AddGenericScope<T>(this IServiceCollection services)
        {
            services.AddMicrosoftScopeFactory(); // this is not necessary, but in 99.99% of cases this is exactly what you need
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IScopeProvider, GenericScopeProvider<T>>());
            return services;
        }

        /// <summary> Register <see cref="MicrosoftScopeFactory" /> in DI as one of the possible the implementations of <see cref="IScopeFactory" />. </summary>
        /// <param name="services"> A collection of DI container services. </param>
        /// <returns> Reference to the passed object <paramref name="services" /> to be able to call methods in a chain. </returns>
        public static IServiceCollection AddMicrosoftScopeFactory(this IServiceCollection services)
        {
            services.TryAddSingleton<IScopeFactory, MicrosoftScopeFactory>();
            return services;
        }

        /// <summary>
        /// Setting options to customize the behavior of <see cref="IServiceProvider" /> when used in Func / Defer / Factory.
        /// </summary>
        /// <param name="services"> A collection of DI container services. </param>
        /// <param name="configure"> Delegate to configure DI options. </param>
        /// <returns> Reference to the passed object <paramref name="services" /> to be able to call methods in a chain. </returns>
        public static IServiceCollection AddServiceProviderAdvancedOptions(this IServiceCollection services, Action<ServiceProviderAdvancedOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var descriptor = services.LastOrDefault(s => s.ServiceType == typeof(ServiceProviderAdvancedOptions));

            if (descriptor == null)
            {
                var options = new ServiceProviderAdvancedOptions();
                configure(options);
                options.Services = services;

                services.AddSingleton(options);
            }
            else if (descriptor.ImplementationInstance is ServiceProviderAdvancedOptions options)
            {
                if (options.Services != services)
                    throw new InvalidOperationException("Unknown configuration");

                configure(options);
            }
            else
            {
                throw new InvalidOperationException("Unknown configuration");
            }

            return services;
        }
    }
}
