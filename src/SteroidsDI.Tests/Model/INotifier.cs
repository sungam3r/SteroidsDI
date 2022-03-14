namespace SteroidsDI.Tests;

public interface INotifier
{
    void Notify();
}

internal class Notifier : INotifier
{
    public void Notify() => Console.WriteLine("Notifier");
}
