using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace SteroidsDI
{
    /// <summary>
    /// Binding context for type <typeparamref name="TService" />. Currently serving named bindings, i.e. the context is
    /// only the binding name, but may add additional context in the future.
    /// </summary>
    /// <typeparam name="TService"> The service type which context is customized. </typeparam>
    public sealed class BindingContext<TService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingContext{TService}" /> class to bind to the
        /// type <typeparamref name="TService" /> inside the given <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services"> A collection of DI container services. </param>
        public BindingContext(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary> A collection of DI container services. </summary>
        public IServiceCollection Services { get; }

        /// <summary> Register a named binding from type <typeparamref name="TService" /> to type <typeparamref name="TImplementation" />.</summary>
        /// <typeparam name="TImplementation"> Implementation type. </typeparam>
        /// <param name="name">
        /// The name of the binding. The name of the binding can be not only a string, but an arbitrary object.
        /// This object selects the required binding in the place where there is a binding context.
        /// </param>
        /// <param name="lifetime"> The lifetime of the dependency object. </param>
        /// <returns> Reference to <c>this</c> to be able to call methods in a chain. </returns>
        public BindingContext<TService> Named<TImplementation>(object name, ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TImplementation : TService
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "No binding name specified.");

            var existing = Services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(TImplementation));
            if (existing != null && (existing.ImplementationType != typeof(TImplementation) || existing.Lifetime != lifetime || existing.ImplementationFactory != null || existing.ImplementationInstance != null))
                throw new InvalidOperationException($@"It is not possible to add a named binding '{name}' for type {typeof(TService)}, because the DI container
already has a binding on type {typeof(TImplementation)} with different characteristics. This is a limitation of the current implementation.");

            if (existing == null)
                Services.Add(new ServiceDescriptor(typeof(TImplementation), typeof(TImplementation), lifetime));

            Services.AddSingleton(new NamedBinding(name, typeof(TService), typeof(TImplementation)));

            return this;
        }

        /// <summary>
        /// Register a named binding from type <typeparamref name="TService" /> to type <typeparamref name="TImplementation" />
        /// with a lifetime equal to the lifetime of the object from the default binding.
        /// </summary>
        /// <typeparam name="TImplementation">Implementation type. </typeparam>
        /// <param name="name">
        /// The name of the binding. The name of the binding can be not only a string, but an arbitrary object.
        /// This object selects the required binding in the place where there is a binding context.
        /// </param>
        /// <returns> Reference to <c>this</c> to be able to call methods in a chain. </returns>
        public BindingContext<TService> Named<TImplementation>(object name)
            where TImplementation : TService
            => Named<TImplementation>(name, GetServiceLifetime());

        private ServiceLifetime GetServiceLifetime()
        {
            return Services.FirstOrDefault(s => s.ServiceType == typeof(TService))?.Lifetime
                ?? throw new InvalidOperationException($@"The DI container does not have a default binding for the type '{typeof(TService)}', so it is not possible to determine the value of Lifetime.
Use the 'Named' overload with explicit Lifetime or first set the default binding in the DI container.");
        }
    }
}
