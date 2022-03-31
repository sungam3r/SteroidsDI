using System.Diagnostics;

namespace SteroidsDI;

/// <summary>
/// Named binding, binds the type <see cref="ServiceType" /> to the
/// type <see cref="ImplementationType" /> in the required context.
/// </summary>
[DebuggerDisplay("{Name}: {ServiceType.Name} -> {ImplementationType.Name}")]
internal sealed class NamedBinding
{
    public NamedBinding(object name, Type serviceType, Type implementationType)
    {
        Name = name;
        ServiceType = serviceType;
        ImplementationType = implementationType;
    }

    /// <summary> Gets the name of the binding. An arbitrary object, not just a string. </summary>
    public object Name { get; }

    /// <summary> Gets the service type. </summary>
    public Type ServiceType { get; }

    /// <summary> Gets the service implementation type. </summary>
    public Type ImplementationType { get; }
}
