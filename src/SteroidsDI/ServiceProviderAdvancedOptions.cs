using Microsoft.Extensions.DependencyInjection;

namespace SteroidsDI;

/// <summary>
/// Options to customize the behavior of <see cref="IServiceProvider" /> when used in Func / [I]Defer / Factory.
/// </summary>
public sealed class ServiceProviderAdvancedOptions
{
    /// <summary>
    /// Set this option to <see langword="false"/> to disable wrapping original <see cref="ObjectDisposedException"/>
    /// thrown from <see cref="IServiceProvider.GetService(Type)"/>. In most cases, the initial error message is in
    /// little informative and the developer does not understand what is happening and what he should do to fix the error.
    /// <br/>
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool UseFriendlyObjectDisposedException { get; set; } = true;

    /// <summary>
    /// Set this option to <see langword="true"/> to validate the situation with the presence
    /// of parallel scopes from different providers. The situation is unlikely but may arise.
    /// <br/>
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool ValidateParallelScopes { get; set; }

    /// <summary>
    /// Set this option to <see langword="true"/> to allow resolving objects through the root
    /// provider if the current scope is missing. The object must have lifetime different from
    /// scoped. Getting scoped objects through the root provider is ALWAYS FORBIDDEN.
    /// <br/>
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool AllowRootProviderResolve { get; set; }

    /// <summary>For internal use only.</summary>
    internal IServiceCollection? Services { get; set; }
}
