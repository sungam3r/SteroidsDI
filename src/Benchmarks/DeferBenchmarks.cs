using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SteroidsDI;

namespace Benchmarks;

public class DeferBenchmarks
{
    private sealed class Dependency
    {
    }

    private IServiceProvider _provider = null!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services
            .Configure<ServiceProviderAdvancedOptions>(opt => opt.AllowRootProviderResolve = true)
            .AddSingleton<Dependency>()
            .AddDefer();

        _provider = services.BuildServiceProvider();
    }

    [Benchmark]
    public void ResolveDefer()
    {
        var defer = _provider.GetRequiredService<Defer<Dependency>>();
        _ = defer.Value;
    }

    [Benchmark]
    public void ResolveIDefer()
    {
        var defer = _provider.GetRequiredService<IDefer<Dependency>>();
        _ = defer.Value;
    }
}
