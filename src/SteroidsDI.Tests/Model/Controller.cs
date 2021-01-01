using System;
using SteroidsDI.Core;

namespace SteroidsDI.Tests
{
    internal class Controller : IDisposable
    {
        public Controller(
            IScopeFactory scopeFactory,
            IMegaFactory factory,
            IGenericFactory<IBuilder, INotifier> generic,
            Func<ScopedService> scopedFunc,
            Func<TransientService> transientFunc,
            Defer<ScopedService> scopedDefer,
            Defer<TransientService> transientDefer)
        {
            Factory = factory;
            Generic = generic;
            ScopedFunc = scopedFunc;
            ScopedDefer = scopedDefer;
            TransientFunc = transientFunc;
            TransientDefer = transientDefer;

            Console.WriteLine("Controller created");

            using (new Scoped<Controller>(scopeFactory))
            {
            }
        }

        public IMegaFactory Factory { get; }

        public IGenericFactory<IBuilder, INotifier> Generic { get; set; }

        public Func<ScopedService> ScopedFunc { get; set; }

        public Defer<ScopedService> ScopedDefer { get; set; }

        public Func<TransientService> TransientFunc { get; set; }

        public Defer<TransientService> TransientDefer { get; set; }

        public void Dispose()
        {
            Console.WriteLine("Controller disposed");
        }
    }
}
