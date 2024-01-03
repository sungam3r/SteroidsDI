namespace SteroidsDI.Core;

/// <summary>
/// A factory for creating scopes. Works in conjunction with <see cref="IScopeProvider" />.
/// </summary>
public interface IScopeFactory
{
    /// <summary>Create scope.</summary>
    /// <returns>
    /// Scope object which should be destroyed at the end of scope.
    /// It SHOULD NOT be null, but MAY be null.
    /// </returns>
    IDisposable CreateScope();
}
