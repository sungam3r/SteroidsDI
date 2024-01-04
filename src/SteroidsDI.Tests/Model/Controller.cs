using SteroidsDI.Core;

namespace SteroidsDI.Tests;

internal class Controller : IDisposable
{
    public Controller(
        IScopeFactory scopeFactory,
        INonGenericFactory nonGenericFactory,
        IGenericFactory<IBuilder, INotifier> genericFactory,
        Func<ScopedService> scopedFunc,
        Func<TransientService> transientFunc,
        Defer<ScopedService> scopedDefer,
        Defer<TransientService> transientDefer)
    {
        NonGenericFactory = nonGenericFactory;
        GenericFactory = genericFactory;
        ScopedFunc = scopedFunc;
        ScopedDefer = scopedDefer;
        TransientFunc = transientFunc;
        TransientDefer = transientDefer;

        Console.WriteLine("Controller created");

        using (new Scoped<Controller>(scopeFactory))
        {
        }
    }

    public INonGenericFactory NonGenericFactory { get; }

    public IGenericFactory<IBuilder, INotifier> GenericFactory { get; set; }

    public Func<ScopedService> ScopedFunc { get; set; }

    public Defer<ScopedService> ScopedDefer { get; set; }

    public Func<TransientService> TransientFunc { get; set; }

    public Defer<TransientService> TransientDefer { get; set; }

    public void Dispose()
    {
        Console.WriteLine("Controller disposed");
    }
}
