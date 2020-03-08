# SteroidsDI

Advanced Dependency Injection to use every day.

**The documentation is not complete and is under development.**

Commonly used APIs:
- `AddFunc` for `Func<T>` injection
- `AddDefer` for `Defer<T>` injection
- `AddFactory` for a named factory injection; implementation type is generated in runtime

These APIs solve the same problem, approaching the design of their API from different angles.
The challenge is to provide a dependency T through some intermediary object X where an explicit
dependency on T is either not possible or not desirable.

An example of impossibility is a dependency on a scoped lifetime in an object with a singleton lifetime.

An example of non-desirability is creating dependency is expensive and not always required.

Examples:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddDefer(options => options.ValidateParallelScopes = true)
}
```