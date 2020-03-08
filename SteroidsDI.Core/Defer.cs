namespace System // yes, this is exactly the namespace by analogy with Func<T>
{
    /// <summary>
    /// An analogue of <see cref="Func{TResult}" /> for deferred resolve of the required object from a DI container.
    /// Has a more explicit/simple API compared to Func. This type must be used for those constructor arguments 
    /// that requires deferred calculation of T.
    /// </summary>
    /// <typeparam name="T"> Dependency type. </typeparam>
    public abstract class Defer<T>
    {
        /// <summary> Get required dependency. </summary>
        public abstract T Value { get; }
    }
}
