using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SteroidsDI;
using SteroidsDI.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary> Extension methods for <see cref="IServiceCollection"/>. </summary>
    public static class ServiceCollectionExtensions
    {
        private static IServiceCollection AddAdvancedOptions(this IServiceCollection services)
            => services.Configure<ServiceProviderAdvancedOptions>(opt => opt.Services = services);

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

            services.AddAdvancedOptions();
            services.TryAddSingleton(factoryType, FactoryGenerator.Generate(factoryType));
            return services;
        }

        /// <summary> Add the specified type <typeparamref name="TFactory"/> to the DI container as a factory that performs factory methods for creating objects. </summary>
        /// <typeparam name="TFactory"> Factory type. </typeparam>
        /// <param name="services"> A collection of DI container services. </param>
        /// <returns> Reference to the passed object <paramref name="services" /> to be able to call methods in a chain. </returns>
        public static IServiceCollection AddFactory<TFactory>(this IServiceCollection services)
            => services.AddAdvancedOptions().AddFactory(typeof(TFactory));

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
            => services.AddAdvancedOptions().AddSingleton(provider =>
                {
                    var options = provider.GetRequiredService<IOptions<ServiceProviderAdvancedOptions>>();
                    return new Func<TService>(() => provider.Resolve<TService>(options.Value));
                });

        /// <summary>
        /// Adds support for <see cref="IDefer{T}" /> and <see cref="Defer{T}" /> - deferring resolving
        /// the object in the desired scope. The effect is completely similar to one from
        /// <see cref="AddFunc{TService}(IServiceCollection)">AddFunc</see> with the difference
        /// that this method works immediately for all objects registered in the DI container.
        /// </summary>
        /// <param name="services"> A collection of DI container services. </param>
        /// <returns> Reference to the passed object <paramref name="services" /> to be able to call methods in a chain. </returns>
        public static IServiceCollection AddDefer(this IServiceCollection services)
            => services.AddAdvancedOptions()
                       .AddSingleton(typeof(Defer<>), typeof(DelegatedDefer<>))
                       .AddSingleton(typeof(IDefer<>), typeof(DelegatedDefer<>));

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
    }
}
