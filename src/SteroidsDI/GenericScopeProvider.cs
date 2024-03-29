using Microsoft.Extensions.DependencyInjection;
using SteroidsDI.Core;

namespace SteroidsDI;

/// <summary>
/// A generic <see cref="IScopeProvider" /> that works with an instance of <see cref="GenericScope{T}" />.
/// </summary>
/// <typeparam name="T">
/// An arbitrary type that is used to create various static AsyncLocal fields. The caller may
/// set unique closed type, thereby providing its own storage, to which only he will have access.
/// </typeparam>
public sealed class GenericScopeProvider<T> : IScopeProvider
{
    /// <inheritdoc/>
    public IServiceProvider? GetScopedServiceProvider(IServiceProvider root)
        => (GenericScope<T>.CurrentScope as IServiceScope)?.ServiceProvider;
}
