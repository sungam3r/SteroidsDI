namespace System; // yes, this is exactly the namespace by analogy with Func<T>

/// <summary>
/// An analogue of <see cref="Func{TResult}" /> for deferred resolving of the required object
/// from a DI container. Has a more explicit/simple API compared to Func. This type must be
/// used for those constructor arguments that require deferred calculation of T.
/// </summary>
/// <typeparam name="T">Type of dependency.</typeparam>
public abstract class Defer<T> : IDefer<T>
{
    /// <summary>Gets required dependency.</summary>
    public abstract T Value { get; }
}
