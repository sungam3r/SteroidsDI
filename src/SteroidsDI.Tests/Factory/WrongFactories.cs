using System;

namespace SteroidsDI.Tests
{
    public interface IFactoryWithEvent
    {
        event EventHandler Click;
    }

    public interface IFactoryWithProperty
    {
        int Age { get; }
    }

    public interface IFactoryWithMethodWithManyArgs
    {
        IBuilder XXX(int a, string b, DateTime c);
    }
}
