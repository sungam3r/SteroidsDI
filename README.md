# SteroidsDI

![Activity](https://img.shields.io/github/commit-activity/w/sungam3r/SteroidsDI)
![Activity](https://img.shields.io/github/commit-activity/m/sungam3r/SteroidsDI)
![Activity](https://img.shields.io/github/commit-activity/y/sungam3r/SteroidsDI)

![Size](https://img.shields.io/github/repo-size/sungam3r/SteroidsDI)

Advanced Dependency Injection to use every day.

**The documentation is not complete and is under development.**

## Installation

This repository provides the following packages:

| Package | Downloads | Nuget Latest | Description |
|---------|-----------|--------------|-------------|
| SteroidsDI | [![Nuget](https://img.shields.io/nuget/dt/SteroidsDI)](https://www.nuget.org/packages/SteroidsDI) | [![Nuget](https://img.shields.io/nuget/v/SteroidsDI)](https://www.nuget.org/packages/SteroidsDI) | Advanced Dependency Injection for Microsoft.Extensions.DependencyInjection: AddDefer, AddFunc, AddFactory |
| SteroidsDI.Core | [![Nuget](https://img.shields.io/nuget/dt/SteroidsDI.Core)](https://www.nuget.org/packages/SteroidsDI.Core) | [![Nuget](https://img.shields.io/nuget/v/SteroidsDI.Core)](https://www.nuget.org/packages/SteroidsDI.Core) | Dependency Injection primitives |
| SteroidsDI.AspNetCore | [![Nuget](https://img.shields.io/nuget/dt/SteroidsDI.AspNetCore)](https://www.nuget.org/packages/SteroidsDI.AspNetCore) | [![Nuget](https://img.shields.io/nuget/v/SteroidsDI.AspNetCore)](https://www.nuget.org/packages/SteroidsDI.AspNetCore) | Scope Provider for ASP.NET Core |

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
1. `Defer<T>` abstraction which looks like `Lazy<T>` but has a significant difference - `Defer<T>` **does not cache the value**.
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

```c#
class MyObject
{
    private IRepository _repo;

    public MyObject(IRepository repo) { _repo = repo; }
    
    public void DoSomething() { _repo.DoMagic(); }
}
```

After:

```c#
class MyObject
{
    private Func<IRepository> _repo;

    public MyObject(Func<IRepository> repo) { _repo = repo; }
    
    public void DoSomething() { _repo().DoMagic(); }
}
```

How to configure in DI:

```c#
public void ConfigureServices(IServiceCollection services)
{
    // First register your IRepository and then call
    services.AddFunc<IRepository>();
}
```

Note that you should call `AddFunc` for each dependency which you want to inject as `Func<T>`.

## Defer\<T>

This method suggests more explicit API - inject `Defer<T>` instead of `T`:

Before:

```c#
class MyObject
{
    private IRepository _repo;

    public MyObject(IRepository repo) { _repo = repo; }
    
    public void DoSomething() { _repo.DoMagic(); }
}
```

After:

```c#
class MyObject
{
    private Defer<IRepository> _repo;

    public MyObject(Defer<IRepository> repo) { _repo = repo; }
    
    public void DoSomething() { _repo.Value.DoMagic(); }
}
```

How to configure in DI:

```c#
public void ConfigureServices(IServiceCollection services)
{
    // First register your IRepository and then call
    services.AddDefer();
}
```

Note that unlike `AddFunc<T>`, the `AddDefer` method needs to be called only once.

## Named factory

This method is the most difficult to implement, but from the public API point of view it is just as simple as previous two.
It assumes that you declare a factory interface with one or more methods without parameters. Method name does not matter.
Each factory method should return some dependency type configured in DI container:

```c#
public interface IMyRepositoryFactory
{
    IRepository GetPersonsRepo();
}
```

And inject this factory into your "parent" type:

```c#
class MyObject
{
    private IMyRepositoryFactory _factory;

    public MyObject(IMyRepositoryFactory factory) { _factory = factory; }
    
    public void DoSomething() { _factory.GetPersonsRepo().DoMagic(); }
}
```

How to configure in DI:

```c#
public void ConfigureServices(IServiceCollection services)
{
    // First register your IRepository and then call
    services.AddFactory<IMyRepositoryFactory>();
}
```

Implementation for `IMyRepositoryFactory` will be generated at runtime.

_In fact, a factory method can take one parameter of an arbitrary type. In this case, a named binding should be specified.
This feature will be documented later._

## How it works?

[Everything is simple here](SteroidsDI/Resolver.cs). All three methods come down to delegating dependency resolution to
the _appropriate_ `IServiceProvider`. What does _appropriate_ mean? As a rule, in a ASP.NET Core application, everyone
is used to working with one (scoped) provider obtained from `IHttpContextAccessor` - `HttpContext.RequestServices`.
But in the general case, there can be many such providers. In addition, dependency-consuming code is not aware of their
existence. This code may be a general purpose library no tightly coupled with application specific environment. Therefore
abstraction for obtaining the _appropriate_ `IServiceProvider` is [introduced](SteroidsDI.Core/IScopeProvider.cs). Yes,
one more abstraction again!

This project provides two built-in providers:
1. [`AspNetCoreHttpScopeProvider`](SteroidsDI.AspNetCore/AspNetCoreHttpScopeProvider.cs) for ASP.NET Core apps.
1. [`GenericScopeProvider<T>`](SteroidsDI/GenericScopeProvider.cs) for general purpose libraries.

How to configure in DI:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpScope();
    services.AddGenericScope<SomeClass>();
}
```

And of course you can always write your own provider.

## Advanced behavior

You can customize the behavior of `AddFunc`/`AddDefer`/`AddFactory` APIs via [ServiceProviderAdvancedOptions](SteroidsDI/ServiceProviderAdvancedOptions.cs):

```c#
public void ConfigureServices(IServiceCollection services)
{
    // using optional delegate parameter in all methods
    services.AddDefer(options => options.ValidateParallelScopes = true);
    services.AddFunc<IRepository>(options => options.ValidateParallelScopes = true);
    services.AddFactory<IRepositoryFactory>(options => options.ValidateParallelScopes = true);

    // or using extensions from Microsoft.Extensions.Options/Microsoft.Extensions.Options.ConfigurationExtensions packages
    services.Configure<ServiceProviderAdvancedOptions>(options => options.AllowRootProviderResolve = true)
    services.Configure<ServiceProviderAdvancedOptions>(Configuration.GetSection("Steroids"));
}
```

All delegates applied will be called sequentially, modifying the same instance of options.

## Examples

You can see how to use all the aforementioned APIs in the [example project](/Example).

## FAQ

**Q**. Wait a moment. Doesn't `Microsoft.Extensions.DependencyInjection` have support for this out of the box?

**A**. Unfortunately no. I myself would rather be able to use the existing feature than to write my own package.

<br/>

**Q**. Isn't what you offer is a **ServiceLocator**? I heard that the ServiceLocator is antipattern.

**A**. Yes, ServiceLocator is a [known antipattern](https://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/).
The fundamental difference with the proposed solutions is that ServiceLocator allows you to resolve
any dependency in runtime while `Func<T>`, `Defer<T>` and Named Factory are designed to resolve
only specific dependencies specified at the compile-time.

<br/>

**Q**. Is this some kind of new dependency injection approach?

**A**. Actually not. A description of this approach can be found in articles/blogs many years ago, for example
[here](https://www.planetgeek.ch/2011/12/31/ninject-extensions-factory-introduction/).