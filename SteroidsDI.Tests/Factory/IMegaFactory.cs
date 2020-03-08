namespace SteroidsDI.Tests
{
    /// <summary> An factory for which implementation is generated in runtime. </summary>
    public interface IMegaFactory
    {
        IBuilder AAA();

        INotifier BBB();

        IBuilder CCC(string name);

        IBuilder DDD(ManagerType type);
    }
}
