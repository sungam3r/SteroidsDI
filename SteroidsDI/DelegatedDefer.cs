using System;

namespace SteroidsDI
{
    internal sealed class DelegatedDefer<T> : Defer<T>
    {
        private readonly IServiceProvider _provider;
        private readonly ServiceProviderAdvancedOptions _options;

        public DelegatedDefer(IServiceProvider provider, ServiceProviderAdvancedOptions options)
        {
            _provider = provider;
            _options = options;
        }

        public override T Value => _provider.Resolve<T>(_options);
    }
}
