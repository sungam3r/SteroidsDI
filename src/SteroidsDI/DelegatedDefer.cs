using System;
using Microsoft.Extensions.Options;

namespace SteroidsDI
{
    internal sealed class DelegatedDefer<T> : Defer<T>
    {
        private readonly IServiceProvider _provider;
        private readonly ServiceProviderAdvancedOptions _options;

        public DelegatedDefer(IServiceProvider provider, IOptions<ServiceProviderAdvancedOptions> options)
        {
            _provider = provider;
            _options = options.Value;
        }

        public override T Value => _provider.Resolve<T>(_options);
    }
}
