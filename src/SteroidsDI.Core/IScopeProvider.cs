namespace SteroidsDI.Core;

/// <summary>
/// Provider of scoped <see cref="IServiceProvider" />. The mission of this interface is
/// not to CREATE something, but to provide the required object, which was created one way
/// or another before that. That is, it is an interface for STORAGE of objects. This is its
/// difference from <see cref = "IScopeFactory" />.
/// </summary>
public interface IScopeProvider
{
    /// <summary>
    /// Gets scoped <see cref="IServiceProvider" />, that is,
    /// one in which scoped dependencies can be resolved.
    /// </summary>
    /// <param name="rootProvider">
    /// The root <see cref="IServiceProvider" /> object, which the
    /// provider may need in order to calculate the current scope.
    /// </param>
    /// <returns>
    /// The scoped <see cref="IServiceProvider" /> object or <c>null</c>
    /// if the provider does not have the current scope.
    /// </returns>
    IServiceProvider? GetScopedServiceProvider(IServiceProvider rootProvider);
}
