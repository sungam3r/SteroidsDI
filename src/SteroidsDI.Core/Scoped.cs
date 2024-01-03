using System.Reflection;

namespace SteroidsDI.Core;

/// <summary>
/// Auxiliary class for creating and destroying scopes. Scopes are sections of code within which
/// scoped dependencies are resolved. This class controls the state of <see cref="GenericScope{T}" />,
/// initializing its initial state and cleaning/destroying it "at the exit", i.e when calling
/// <see cref="Dispose"/> or <see cref="DisposeAsync"/>.
/// </summary>
/// <typeparam name="T">
/// An arbitrary type that is used to create various static AsyncLocal fields. The caller may
/// set unique closed type, thereby providing its own storage, to which only he will have access.
/// </typeparam>
public readonly struct Scoped<T> : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Scoped{T}"/>.
    /// </summary>
    /// <param name="scopeFactory">A factory for creating scopes.</param>
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

    /// <inheritdoc />
    public void Dispose()
    {
        GenericScope<T>.CurrentScope = null;
        Scope?.Dispose(); // in some cases scopeFactory.CreateScope() MAY return null
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        GenericScope<T>.CurrentScope = null;
        if (Scope is IAsyncDisposable ad)
            return ad.DisposeAsync();
        Scope?.Dispose(); // in some cases scopeFactory.CreateScope() MAY return null
        // ValueTask.CompletedTask is only available in net5.0 and later.
        return default;
    }
}

/// <summary>
/// Auxiliary class for creating and destroying scopes. Scopes are sections of code within which
/// scoped dependencies are resolved. This class controls the state of <see cref="GenericScope{T}" />,
/// initializing its initial state and cleaning/destroying it "at the exit", i.e when calling
/// <see cref="Dispose"/> or <see cref="DisposeAsync"/>.
/// <br/><br/>
/// Non-generic version of <see cref="Scoped{T}"/>.
/// </summary>
public readonly struct Scoped : IDisposable, IAsyncDisposable
{
    private readonly PropertyInfo _currentScopeProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="Scoped"/>.
    /// </summary>
    /// <param name="type">
    /// An arbitrary type that is used to create various static AsyncLocal fields. The caller may
    /// set unique closed type, thereby providing its own storage, to which only he will have access.
    /// </param>
    /// <param name="scopeFactory">A factory for creating scopes.</param>
    public Scoped(Type type, IScopeFactory scopeFactory)
    {
        if (scopeFactory == null)
            throw new ArgumentNullException(nameof(scopeFactory));

        _currentScopeProperty = typeof(GenericScope<>).MakeGenericType(type).GetProperty(nameof(GenericScope<int>.CurrentScope));
        if (_currentScopeProperty.GetValue(null) != null)
            throw new InvalidOperationException($"The current scope of GenericScope<{type.Name}> is not null when trying to initialize it.");

        Scope = scopeFactory.CreateScope();
        _currentScopeProperty.SetValue(null, Scope);
    }

    /// <summary>
    /// Gets current scope.
    /// </summary>
    public IDisposable Scope { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        _currentScopeProperty.SetValue(null, null);
        Scope?.Dispose(); // in some cases scopeFactory.CreateScope() MAY return null
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _currentScopeProperty.SetValue(null, null);
        if (Scope is IAsyncDisposable ad)
            return ad.DisposeAsync();
        Scope?.Dispose(); // in some cases scopeFactory.CreateScope() MAY return null
        // ValueTask.CompletedTask is only available in net5.0 and later.
        return default;
    }
}
