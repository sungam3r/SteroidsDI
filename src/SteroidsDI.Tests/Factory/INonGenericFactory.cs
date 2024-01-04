namespace SteroidsDI.Tests;

/// <summary>A factory for which implementation is generated in runtime.</summary>
public interface INonGenericFactory
{
    IBuilder AAA();

    INotifier BBB();

    IBuilder CCC(string name);

    IBuilder DDD(ManagerType type);
}
