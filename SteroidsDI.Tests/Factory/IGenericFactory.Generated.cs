using System;
using System.Collections.Generic;
using System.Linq;

namespace SteroidsDI.Tests
{
    /// <summary> This is an example of a generated generic factory. </summary>
    internal sealed class IGenericFactory_Generated<TBuilder, TNotifier> : IGenericFactory<TBuilder, TNotifier>
    {
        private readonly IServiceProvider _provider;
        private readonly List<NamedBinding> _bindings; // all bindings
        private readonly ServiceProviderAdvancedOptions _options;

        public IGenericFactory_Generated(IServiceProvider provider, IEnumerable<NamedBinding> bindings, ServiceProviderAdvancedOptions options)
        {
            _provider = provider;
            _bindings = bindings.ToList();
            _options = options;
        }

        public TBuilder AAA() => _provider.Resolve<TBuilder>(_options);

        public TNotifier BBB() => _provider.Resolve<TNotifier>(_options);

        public TBuilder CCC(string name) => _provider.ResolveByNamedBinding<TBuilder>(name, _bindings, _options);

        public TBuilder DDD(ManagerType type) => _provider.ResolveByNamedBinding<TBuilder>(type, _bindings, _options);
    }
}
