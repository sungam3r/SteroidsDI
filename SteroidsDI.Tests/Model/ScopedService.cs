using System;
using System.Diagnostics;
using System.Threading;

namespace SteroidsDI.Tests
{
    /// <summary> It was required to test scoped dependencies in singleton objects through Func. </summary>
    [DebuggerDisplay("ScopedService {Index}")]
    internal class ScopedService : IDisposable
    {
        private static int _counter;

        public ScopedService()
        {
            Index = Interlocked.Increment(ref _counter);
            Console.WriteLine($"ScopedService {Index} created");
        }

        public int Index { get; }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            Disposed = true;
            Console.WriteLine($"ScopedService {Index} disposed");
        }

        public int Sum(int first, int second) => first + second;
    }
}
