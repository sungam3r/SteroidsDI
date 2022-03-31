namespace System;

/// <summary>
/// Covariant interface for <see cref="Defer{T}" /> which is an analogue of <see cref="Func{TResult}" />
/// for deferred resolving of the required object from a DI container.
/// Has a more explicit/simple API compared to Func. This type must be used for
/// those constructor arguments that require deferred calculation of T.
/// </summary>
/// <typeparam name="T"> Type of dependency. </typeparam>
public interface IDefer<out T>
{
    /// <summary> Get required dependency. </summary>
    T Value { get; }
}
