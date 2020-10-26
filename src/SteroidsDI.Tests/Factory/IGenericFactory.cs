namespace SteroidsDI.Tests
{
    /// <summary> An generic factory for which implementation is generated in runtime. </summary>
    /// <typeparam name="TBuilder"> The first generic parameter. </typeparam>
    /// <typeparam name="TNotifier"> The second generic parameter. </typeparam>
    public interface IGenericFactory<TBuilder, TNotifier>
    {
        TBuilder AAA();

        TNotifier BBB();

        TBuilder CCC(string name);

        TBuilder DDD(ManagerType type);
    }
}
