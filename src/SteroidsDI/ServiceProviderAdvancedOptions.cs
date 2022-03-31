using Microsoft.Extensions.DependencyInjection;

namespace SteroidsDI;

/// <summary>
/// Options to customize the behavior of <see cref="IServiceProvider" /> when used in Func / [I]Defer / Factory.
/// </summary>
public sealed class ServiceProviderAdvancedOptions
{
    /// <summary>
    /// <see langword="true"/> to validate the situation with the presence of parallel
    /// scopes from different providers. The situation is unlikely but may arise.
    /// </summary>
    public bool ValidateParallelScopes { get; set; }

    /// <summary>
    /// Allows resolving objects through the root provider if the current scope is missing.
    /// The object must have lifetime different from scoped. Getting scoped objects through
    /// the root provider is ALWAYS FORBIDDEN. Defaults to <see langword="false"/>.
    /// </summary>
    public bool AllowRootProviderResolve { get; set; }

    /// <summary> For internal use only. </summary>
    internal IServiceCollection? Services { get; set; }
}
