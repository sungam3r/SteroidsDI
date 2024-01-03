using Microsoft.Extensions.Options;

namespace SteroidsDI.Tests;

/// <summary>This is an example of a generated factory.</summary>
internal sealed class IMegaFactory_Generated : IMegaFactory
{
    private readonly IServiceProvider _provider;
    private readonly List<NamedBinding> _bindings; // all bindings
    private readonly ServiceProviderAdvancedOptions _options;

    public IMegaFactory_Generated(IServiceProvider provider, IEnumerable<NamedBinding> bindings, IOptions<ServiceProviderAdvancedOptions> options)
    {
        _provider = provider;
        _bindings = bindings.ToList();
        _options = options.Value;
    }

    public IBuilder AAA() => _provider.Resolve<IBuilder>(_options);

    public INotifier BBB() => _provider.Resolve<INotifier>(_options);

    public IBuilder CCC(string name) => _provider.ResolveByNamedBinding<IBuilder>(name, _bindings, _options);

    public IBuilder DDD(ManagerType type) => _provider.ResolveByNamedBinding<IBuilder>(type, _bindings, _options);
}
