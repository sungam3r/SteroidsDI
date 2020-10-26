using System;
using System.Diagnostics;
using System.Threading;

namespace SteroidsDI.Tests
{
    /// <summary> It was required to test transient dependencies in singleton objects through Func. </summary>
    [DebuggerDisplay("TransientService {Index}")]
    internal class TransientService : IDisposable
    {
        private static int _counter;

        public TransientService()
        {
            Index = Interlocked.Increment(ref _counter);
            Console.WriteLine($"TransientService {Index} created");
        }

        public int Index { get; }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            Disposed = true;
            Console.WriteLine($"TransientService {Index} disposed");
        }
    }
}
