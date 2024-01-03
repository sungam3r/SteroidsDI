namespace SteroidsDI.Tests;

public interface IBuilder
{
    void Build();
}

internal class Builder : IBuilder
{
    public void Build() => Console.WriteLine("Builder");
}

internal class SpecialBuilder : IBuilder
{
    public void Build() => Console.WriteLine("SpecialBuilder!");
}

internal class SpecialBuilderOver9000Level : IBuilder
{
    public void Build() => Console.WriteLine("!!!!!!!SpecialBuilderOver9000Level!!!!!!!");
}

internal class DefaultBuilder : IBuilder
{
    public void Build() => Console.WriteLine("Default");
}
