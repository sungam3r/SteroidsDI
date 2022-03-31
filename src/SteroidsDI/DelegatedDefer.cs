using Microsoft.Extensions.Options;

namespace SteroidsDI;

internal sealed class DelegatedDefer<T> : Defer<T>
{
    private readonly IServiceProvider _rootProvider;
    private readonly ServiceProviderAdvancedOptions _options;

    public DelegatedDefer(IServiceProvider rootProvider, IOptions<ServiceProviderAdvancedOptions> options)
    {
        _rootProvider = rootProvider;
        _options = options.Value;
    }

    /// <inheritdoc/>
    public override T Value => _rootProvider.Resolve<T>(_options);
}
