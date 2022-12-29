# SteroidsDI

<a href="https://www.buymeacoffee.com/sungam3r" target="_blank"><img src="https://bmc-cdn.nyc3.digitaloceanspaces.com/BMC-button-images/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>

![License](https://img.shields.io/github/license/sungam3r/SteroidsDI)

[![codecov](https://codecov.io/gh/sungam3r/SteroidsDI/branch/master/graph/badge.svg?token=0ZRHIUEQM4)](https://codecov.io/gh/sungam3r/SteroidsDI)
[![Nuget](https://img.shields.io/nuget/dt/SteroidsDI.Core)](https://www.nuget.org/packages/SteroidsDI.Core)
[![Nuget](https://img.shields.io/nuget/v/SteroidsDI.Core)](https://www.nuget.org/packages/SteroidsDI.Core)

[![GitHub Release Date](https://img.shields.io/github/release-date/sungam3r/SteroidsDI?label=released)](https://github.com/sungam3r/SteroidsDI/releases)
[![GitHub commits since latest release (by date)](https://img.shields.io/github/commits-since/sungam3r/SteroidsDI/latest?label=new+commits)](https://github.com/sungam3r/SteroidsDI/commits/master)
![Size](https://img.shields.io/github/repo-size/sungam3r/SteroidsDI)

[![GitHub contributors](https://img.shields.io/github/contributors/sungam3r/SteroidsDI)](https://github.com/sungam3r/SteroidsDI/graphs/contributors)
![Activity](https://img.shields.io/github/commit-activity/w/sungam3r/SteroidsDI)
![Activity](https://img.shields.io/github/commit-activity/m/sungam3r/SteroidsDI)
![Activity](https://img.shields.io/github/commit-activity/y/sungam3r/SteroidsDI)

[![Run unit tests](https://github.com/sungam3r/SteroidsDI/actions/workflows/test.yml/badge.svg)](https://github.com/sungam3r/SteroidsDI/actions/workflows/test.yml)
[![Publish preview to GitHub registry](https://github.com/sungam3r/SteroidsDI/actions/workflows/publish-preview.yml/badge.svg)](https://github.com/sungam3r/SteroidsDI/actions/workflows/publish-preview.yml)
[![Publish release to Nuget registry](https://github.com/sungam3r/SteroidsDI/actions/workflows/publish-release.yml/badge.svg)](https://github.com/sungam3r/SteroidsDI/actions/workflows/publish-release.yml)
[![CodeQL analysis](https://github.com/sungam3r/SteroidsDI/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/sungam3r/SteroidsDI/actions/workflows/codeql-analysis.yml)

Advanced Dependency Injection to use every day.

## Installation

This repository provides the following packages:

| Package | Downloads | Nuget Latest | Description |
|---------|-----------|--------------|-------------|
| SteroidsDI.Core | [![Nuget](https://img.shields.io/nuget/dt/SteroidsDI.Core)](https://www.nuget.org/packages/SteroidsDI.Core) | [![Nuget](https://img.shields.io/nuget/v/SteroidsDI.Core)](https://www.nuget.org/packages/SteroidsDI.Core) | Dependency Injection primitives |
| SteroidsDI | [![Nuget](https://img.shields.io/nuget/dt/SteroidsDI)](https://www.nuget.org/packages/SteroidsDI) | [![Nuget](https://img.shields.io/nuget/v/SteroidsDI)](https://www.nuget.org/packages/SteroidsDI) | Advanced Dependency Injection for Microsoft.Extensions.DependencyInjection: AddDefer, AddFunc, AddFactory; depends on SteroidsDI.Core |
| SteroidsDI.AspNetCore | [![Nuget](https://img.shields.io/nuget/dt/SteroidsDI.AspNetCore)](https://www.nuget.org/packages/SteroidsDI.AspNetCore) | [![Nuget](https://img.shields.io/nuget/v/SteroidsDI.AspNetCore)](https://www.nuget.org/packages/SteroidsDI.AspNetCore) | Scope Provider for ASP.NET Core; depends on SteroidsDI.Core |

You can install the latest stable version via NuGet:
```
> dotnet add package SteroidsDI
> dotnet add package SteroidsDI.Core
> dotnet add package SteroidsDI.AspNetCore
```

## What is it ? Why do I need it ?

.NET Core has built-in support for [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection).
It works and works quite well. We can use the dependencies of the three main lifetimes: singleton, scoped, transient. There are rules that
specify possible combinations of passing objects with one lifetime in objects with another lifetime. For example, you may encounter such
an error message:
> Error while validating the service descriptor 'ServiceType: I_XXX Lifetime: Singleton ImplementationType: XXX':
> Cannot consume scoped service 'YYY' from singleton 'I_XXX'.

The error says that you cannot pass an object with a shorter lifetime to the constructor of a long-living object. Well that's right!
The problem is clear. But how to solve it? Obviously, when using **constructor dependency injection** (.NET Core has built-in support 
only for constructor DI), we must follow these rules. So we have at least 3 options:

1. Lengthen lifetime for injected object.
1. Shorten lifetime for an object that injects a dependency.
1. Remove such a dependency.
1. Change the design of dependencies so as to satisfy the rules.

The first method is far from always possible. The second method is much easier to implement, although this will lead to a decrease
in performance due to the repeated creation of objects that were previously created once. The third way... well, you understand,
life will not become easier. So it remains to somehow change the design. This project just offers such a way to solve the problem,
introducing a number of auxiliary abstractions. As many already know

> Any programming problem can be solved by introducing an additional level of abstraction with the exception of the problem of an
> excessive number of abstractions.

The project provides three such abstractions:
1. Well known `Func<T>` delegate.
1. `Defer<T>`/`IDefer<T>` abstractions which look like `Lazy<T>` but have a significant difference - `Defer<T>`/`IDefer<T>`
**do not cache the value**.
1. A named factory interface, when implementation type is generated at runtime.

All these abstractions **solve the same problem**, approaching the design of their API from different angles. The challenge is to
provide a dependency T through some intermediary object X where an explicit dependency on T is either not possible or not desirable.
**Important! No implementation in this package caches dependency T.**

As mentioned above an example of impossibility is a dependency on a scoped lifetime in an object with a singleton lifetime. And an
example of non-desirability is creating dependency is expensive and not always required.

**There is one important point to make - injecting dependency is not the same as using dependency.** In fact, in the case of
constructor injection, the injection of the dependency in the constructor leads (in most cases) to storing a reference to the
passed value in the some field. The dependency will be used later when calling the methods of "parent" object.

## Func\<T>

This method is the easiest and offers to inject `Func<T>` instead of `T`:

Before:

```csharp
class MyObject
{
    private IRepository _repo;

    public MyObject(IRepository repo) { _repo = repo; }
    
    public void DoSomething() { _repo.DoMagic(); }
}
```

After:

```csharp
class MyObject
{
    private Func<IRepository> _repo;

    public MyObject(Func<IRepository> repo) { _repo = repo; }
    
    public void DoSomething() { _repo().DoMagic(); }
}
```

How to configure in DI:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // First register your IRepository and then call
    services.AddFunc<IRepository>();
}
```

Note that you should call `AddFunc` for each dependency `T` which you want to inject as `Func<T>`.

## IDefer\<T> and Defer\<T>

This method suggests more explicit API - inject `IDefer<T>` or `Defer<T>` instead of `T`:

Before:

```csharp
class MyObject
{
    private IRepository _repo;

    public MyObject(IRepository repo) { _repo = repo; }
    
    public void DoSomething() { _repo.DoMagic(); }
}
```

After:

```csharp
class MyObject
{
    private Defer<IRepository> _repo;

    public MyObject(Defer<IRepository> repo) { _repo = repo; }
    
    public void DoSomething() { _repo.Value.DoMagic(); }
}
```

How to configure in DI:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // First register your IRepository and then call
    services.AddDefer();
}
```

Note that unlike `AddFunc<T>`, the `AddDefer` method needs to be called only once. Use `IDefer<T>` interface if you
need covariance. 

## Named factory

This method is the most difficult to implement, but from the public API point of view it is just as simple as previous two.
It assumes that you declare a factory interface with one or more methods without parameters. Method name does not matter.
Each factory method should return some dependency type configured in DI container:

```csharp
public interface IRepositoryFactory
{
    IRepository GetPersonsRepo();
}
```

And inject this factory into your "parent" type:

```csharp
class MyObject
{
    private IRepositoryFactory _factory;

    public MyObject(IRepositoryFactory factory) { _factory = factory; }
    
    public void DoSomething() { _factory.GetPersonsRepo().DoMagic(); }
}
```

How to configure in DI:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // First register your IRepository and then call
    services.AddFactory<IRepositoryFactory>();
}
```

Implementation for `IRepositoryFactory` will be generated at runtime.

In fact, each factory method can take one parameter of an arbitrary type - string, enum, custom class, whatever.
In this case, a _named binding_ should be specified.

```csharp
public interface IRepositoryFactory
{
    IRepository GetPersonsRepo(string mode);
}

public interface IRepository
{
    void Save(Person person); 
}

public class DemoRepository : IRepository
{
...
}

public class ProductionRepository : IRepository
{
...
}

public class RandomRepository : IRepository
{
...
}

public void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<IRepository, DemoRepository>()
            .AddTransient<IRepository, ProductionRepository>()
            .AddTransient<IRepository, RandomRepository>()
            .AddFactory<IRepositoryFactory>()
                .For<IRepository>()
                    .Named<DemoRepository>("demo")
                    .Named<ProductionRepository>("prod")
                    .Named<RandomRepository>("rnd");
}

public class Person
{
    public string Name { get; set; }
}

public class SomeClassWithDependency
{
    private readonly IRepositoryFactory _factory;

    public SomeClassWithDependency(IRepositoryFactory factory)
    {
        _factory = factory;
    }

    public void DoSomething(Person person)
    {
        if (person.Name == "demoUser")
            _factory.GetPersonsRepo("demo").Save(person);
        else if (person.Name.StartsWith("tester"))
            _factory.GetPersonsRepo("rnd").Save(person);
        else
            _factory.GetPersonsRepo("prod").Save(person);
    }
}
```

In the example above, the `GetPersonsRepo` method will return the corresponding implementation of the `IRepository`
interface, configured for the provided name.

## How it works?

[Everything is simple here](src/SteroidsDI/Resolver.cs). All three methods come down to delegating dependency resolution to
the _appropriate_ `IServiceProvider`. What does _appropriate_ mean? As a rule, in a ASP.NET Core application, everyone
is used to working with one (scoped) provider obtained from `IHttpContextAccessor` - `HttpContext.RequestServices`.
But in the general case, there can be many such providers. In addition, dependency-consuming code is not aware of their
existence. This code may be a general purpose library no tightly coupled with application specific environment. Therefore
abstraction for obtaining the _appropriate_ `IServiceProvider` is [introduced](src/SteroidsDI.Core/IScopeProvider.cs). Yes,
one more abstraction again!

This project provides two built-in providers:
1. [`AspNetCoreHttpScopeProvider`](src/SteroidsDI.AspNetCore/AspNetCoreHttpScopeProvider.cs) for ASP.NET Core apps.
1. [`GenericScopeProvider<T>`](src/SteroidsDI/GenericScopeProvider.cs) for general purpose libraries.

How to configure in DI:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpScope();
    services.AddGenericScope<SomeClass>();
}
```

And of course you can always write your own provider:

```csharp
public class MyScopeProvider : IScopeProvider
{
    public IServiceProvider? GetScopedServiceProvider(IServiceProvider rootProvider) => rootProvider.ReturnSomeMagic();
}
```

And provide its registration in DI:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyScope(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IScopeProvider, MyScopeProvider>());
        return services;
    }
}
```

## Advanced behavior

You can customize the behavior of `AddFunc`/`AddDefer`/`AddFactory` APIs via [ServiceProviderAdvancedOptions](src/SteroidsDI/ServiceProviderAdvancedOptions.cs):
Just use standard extension methods from `Microsoft.Extensions.Options/Microsoft.Extensions.Options.ConfigurationExtensions` packages.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<ServiceProviderAdvancedOptions>(options => options.AllowRootProviderResolve = true)
    services.Configure<ServiceProviderAdvancedOptions>(Configuration.GetSection("Steroids"));
}
```

## Examples

You can see how to use all the aforementioned APIs in the [example project](https://github.com/sungam3r/SteroidsDI/tree/master/src/Example).

## FAQ

**Q**. Wait a moment. Doesn't `Microsoft.Extensions.DependencyInjection` have support for this out of the box?

**A**. Unfortunately no. I myself would rather be able to use the existing feature than to write my own package.

<br/>

**Q**. Isn't what you offer is a **ServiceLocator**? I heard that the ServiceLocator is anti-pattern.

**A**. Yes, ServiceLocator is a [known antipattern](https://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/).
The fundamental difference with the proposed solutions is that ServiceLocator allows you to resolve
**any** dependency in runtime while `Func<T>`, `Defer<T>` and Named Factory are designed to resolve
only **known** dependencies specified at the compile-time. Thus, the principal difference is that all
the dependences of the class are declared explicitly and are injected into it. The class itself **does
not pull** these dependencies secretly within its implementation. This is so called [Explicit Dependencies Principle](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles#explicit-dependencies).

<br/>

**Q**. Is this some kind of new dependency injection approach?

**A**. Actually not. A description of this approach can be found in articles/blogs many years ago, for example
[here](https://www.planetgeek.ch/2011/12/31/ninject-extensions-factory-introduction/).

<br/>

**Q**. What should I prefer - `Func<>` or `[I]Defer<>`?

**A**. The main thing is they all work equally under the hood. The difference is in which context you are
going to use these APIs.

There are two main differences:

1) The advantage of `Func<>` is that the code in which you inject `Func<>` does not require any new
dependency, it is well-known .NET delegate type. On the contrary `[I]Defer<>` requires a reference to
`SteroidsDI.Core` package.

2) You should call `AddFunc` for each dependency `T` which you want to inject as `Func<T>`. On the
contrary the `AddDefer` method needs to be called only once.

<br/>

**Q**. What if I want to create my own scope to work with, i.e. not only consume it but also provide?

**A**. First you should somehow get an instance of root `IServiceProvider`.
Then create scope by calling `CreateScope()` method on it and set it into `GenericScope`:

```csharp
var rootProvider = ...;
using var scope = rootProvider.CreateScope();
GenericScope<SomeClass>.CurrentScope = scope;
...
... Some code here that works with scopes.
... All registered Func<T>, [I]Defer<T> and
... factories use created scope.
...
GenericScope<T>.CurrentScope = null;
```

Or you can use a bit simpler approach with [`Scoped<T>`](src/SteroidsDI.Core/Scoped.cs) struct.

```csharp
IScopeFactory scopeFactory = ...; // can be obtained from DI, see AddMicrosoftScopeFactory extension method
using (new Scoped<SomeClass>(scopeFactory))
or
await using (new Scoped<SomeClass>(scopeFactory)) // Scoped class supports IAsyncDisposable as well
{
...
... Some code here that works with scopes.
... All registered Func<T>, [I]Defer<T> and
... factories use created scope.
...
} 
```

Also see [ScopedTestBase](src/SteroidsDI.Tests/Cases/ScopedTestBase.cs) and [ScopedTestDerived](src/SteroidsDI.Tests/Cases/ScopedTestDerived.cs)
for more info. This example shows how you can add scope support to all unit tests.
