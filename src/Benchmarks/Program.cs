using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using Benchmarks;

new DeferBenchmarks().Setup();
var config = ManualConfig
  .Create(DefaultConfig.Instance)
  .AddDiagnoser(MemoryDiagnoser.Default);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
