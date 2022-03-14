namespace SteroidsDI.Core;

/// <summary>
/// A helper class that provides access to the current scope stored in the AsyncLocal field.
/// Works in conjunction with <see cref="IScopeProvider" />.
/// </summary>
/// <typeparam name="T">
/// An arbitrary type that is used to create various static AsyncLocal fields. The caller may set unique
/// closed type, thereby providing its own storage, to which only it will have access.
/// </typeparam>
public static class GenericScope<T>
{
    private static readonly AsyncLocal<IDisposable?> _currentScope = new AsyncLocal<IDisposable?>();

    /// <summary> Gets or sets the current scope. </summary>
    public static IDisposable? CurrentScope
    {
        get => _currentScope.Value;
        set => _currentScope.Value = value;
    }
}
