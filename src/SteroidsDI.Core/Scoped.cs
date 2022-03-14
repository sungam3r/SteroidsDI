namespace SteroidsDI.Core;

/// <summary>
/// Auxiliary class for creating and destroying scopes. These are sections of code within which
/// scoped dependencies are resolved. This class controls the state of <see cref="GenericScope{T}" />,
/// initializing its initial state and cleaning/destroying it at the exit.
/// </summary>
/// <typeparam name="T">
/// An arbitrary type that is used to create various static AsyncLocal fields. The caller may set unique
/// closed type, thereby providing its own storage, to which only it will have access.
/// </typeparam>
public struct Scoped<T> : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Scoped{T}"/>.
    /// </summary>
    /// <param name="scopeFactory"> A factory for creating scopes. </param>
    public Scoped(IScopeFactory scopeFactory)
    {
        if (scopeFactory == null)
            throw new ArgumentNullException(nameof(scopeFactory));

        if (GenericScope<T>.CurrentScope != null)
            throw new InvalidOperationException($"The current scope of GenericScope<{typeof(T).Name}> is not null when trying to initialize it.");

        Scope = scopeFactory.CreateScope();
        GenericScope<T>.CurrentScope = Scope;
    }

    /// <summary>
    /// Gets current scope.
    /// </summary>
    public IDisposable Scope { get; }

    /// <summary> Dispose scope. </summary>
    public void Dispose()
    {
        GenericScope<T>.CurrentScope = null;
        Scope?.Dispose(); // in some cases scopeFactory.CreateScope() MAY return null
    }
}
